using DBRSS.Buffer;
using DBRSS.DataConnector;
using PubSub;

namespace DBRSS;
public class AppStartedEvent { }
public class Application {
 

  private readonly Hub _hub;
  private readonly MqttConnector _mqtt;
  private readonly IBufferService _bufferService;

  public Application(Hub hub, MqttConnector mqtt, IBufferService bufferService) {
    _hub = hub;
    _mqtt = mqtt;
    _bufferService = bufferService;
  }
  
  public async Task Run() {
    try {
      _hub.Subscribe<AppStartedEvent>(this, e => { Console.WriteLine("App started"); });

      await _mqtt.Initialize();

      await _hub.PublishAsync<AppStartedEvent>();

    }
    catch (Exception ex) {
      Console.WriteLine("Application failed to run");
    }
  }

  public BufferSendPackage ReadBufferPacket() {
    return _bufferService.GetFirstBufferPacket();
  }

  public void AcknowledgeBufferPacket(long packetId) {
    _bufferService.AcknowledgeBufferPacket(packetId);
  }
  
}