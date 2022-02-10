namespace DBRSS.Configuration; 

public class MqttConfig {
  public const string MqttConnector = "MqttConnector";
  public string Host { get; set; }
  public int Port { get; set; }
  public string ClientId { get; set; }
  public string Topic { get; set; }
}