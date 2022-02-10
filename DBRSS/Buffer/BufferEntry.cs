using DBRSS.Events;

namespace DBRSS.Buffer; 

public class CollectorBufferEntry {
  public long Id { get; set; }
  public ConnectorMessageEvent Payload { get; set; }
}

public class SenderBufferEntry {
  public long Id { get; set; }
  public ConnectorMessageEvent Payload { get; set; }
}

public class HashPayload {
  public string Ek { get; set; }
  public SenderBufferEntry[] Package { get; set; }

  public HashPayload(string ek, SenderBufferEntry[] package) {
    this.Ek = ek;
    this.Package = package;
  }
  
}

public class BufferSendPackage {
  public string Hash { get; set; }
  public SenderBufferEntry[] Package { get; set; }
}