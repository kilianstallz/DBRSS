using DBRSS.Events;
using DBRSS.Tpm;
using System.Text.Json;
using PubSub;

namespace DBRSS.Buffer;

public class BufferService : IBufferService {
  private readonly Hub _hub;
  private readonly ThresholdService _thresholdService;

  private readonly List<CollectorBufferEntry> _collectorBuffer;
  private readonly List<SenderBufferEntry> _senderBuffer;
  private readonly System.Timers.Timer _senderTimer;


  public BufferService(Hub hub, ThresholdService thresholdService) {
    Console.WriteLine("BufferService");
    _hub = hub;
    _thresholdService = thresholdService;
    _collectorBuffer = new List<CollectorBufferEntry>();
    _senderBuffer = new List<SenderBufferEntry>();
    _senderTimer = new System.Timers.Timer();
    this.Initialize();
  }

  private void Initialize() {
    _hub.Subscribe<AppStartedEvent>(this, e => { Console.WriteLine("Buffer service initialized"); });
    
    _thresholdService.OnTrigger += this.OnCollectorThresholdTrigger;

    _hub.Subscribe<ConnectorMessageEvent>(this, e => {
      Console.WriteLine("New message from device connector");
      this.AddToCollectorBuffer(new CollectorBufferEntry {
        Id = e.Timestamp.Ticks,
        Payload = e,
      });
    });
  }

  private void OnCollectorThresholdTrigger(object? sender, EventArgs eventArgs) {
    var packetId = DateTime.Now.Ticks;
    Console.WriteLine("Moving to sender buffer");
    _senderTimer.Stop();
    _senderTimer.Start();
    foreach (var collectorBufferEntry in _collectorBuffer) {
      _senderBuffer.Add(new SenderBufferEntry() {
        Id = packetId,
        Payload = collectorBufferEntry.Payload,
      });
    }
    _collectorBuffer.Clear();
  }

  private string HashPackage(HashPayload payload) {
    return TpmHelper.HashString(JsonSerializer.Serialize(payload));
  }

  private void AddToCollectorBuffer(CollectorBufferEntry entry) {
    _collectorBuffer.Add(entry);
    _thresholdService.Increment();
    Console.WriteLine("New Collector buffer size: " + _collectorBuffer.Count);
  }
  
  // GetFirstBufferEntry - returns the packet in the sendingBuffer
  public BufferSendPackage? GetFirstBufferPacket() {
    // Find element with the lowest id in the array
    var lowestId = _senderBuffer.First().Id;
    // get all elements with the lowest id
    var package = _senderBuffer.Where(x => x.Id == lowestId).ToArray();
    
    var hashPayload = new HashPayload(TpmHelper.GetPublicEk(), package);

    var hash = this.HashPackage(hashPayload);
    
    return new BufferSendPackage {
      Hash = hash,
      Package = package,
    };
  }

  // AcknowledgeBufferEntry - removes an acknowledged packet from the sendingBuffer
  public void AcknowledgeBufferPacket(long packetId) {
    Console.WriteLine("Acknowledge packet id " + packetId);
    var toRemove = _senderBuffer.Where(x => x.Id == packetId).ToArray();
    foreach (var item in toRemove) {
      _senderBuffer.Remove(item);
    }
  }
}