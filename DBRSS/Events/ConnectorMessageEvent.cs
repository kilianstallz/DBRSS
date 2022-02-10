namespace DBRSS.Events;

public class ConnectorMessageEvent
{
    public string RawPayload { get; init; }
    
    public PayloadType PayloadType { get; init; }

    public DateTime Timestamp { get; init; }

    public string DeviceId { get; init; }

    public ProtocolTypes Protocol { get; init; }
}

public enum ProtocolTypes
{
    Mqtt,
    Opcua
}

public enum PayloadType {
    Int,
    Float,
    String,
    Bool,
    ByteArray
}