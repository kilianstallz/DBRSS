using System.Text;
using System.Text.Json;
using DBRSS.Configuration;
using DBRSS.Events;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using PubSub;

namespace DBRSS.DataConnector;

public class MqttPayload {
  public string Id { get; set; }
  public long Timestamp { get; set; }
  
  // Type of the value in the base64 encoded string
  public string Type { get; set; }
  
  // Base64 Representation of a value
  public string Value { get; set; }
}

public class MqttConnector {
  private Hub _hub;
  private readonly IConfiguration Configuration;

  public MqttConnector(Hub hub, IConfiguration configuration) {
    _hub = hub;
    Configuration = configuration;
  }

  public async Task Initialize() {
    var factory = new MqttFactory();
    var mqttClient = factory.CreateMqttClient();
    try {
      MqttConfig config = Configuration.GetSection(MqttConfig.MqttConnector).Get<MqttConfig>();
      // Connect client
      await mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer(config.Host, config.Port).WithClientId(config.ClientId).Build(),
        CancellationToken.None);
      // Subscribe to node topic
      await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(config.Topic).Build());
      Console.WriteLine("Connection success");
      // Handle messages
      mqttClient.ApplicationMessageReceivedAsync += WriteMessage;
    }
    catch (Exception e) {
      Console.WriteLine("### RECONNECTING FAILED ###" + e.Message);
    }
  }

  private Task WriteMessage(MqttApplicationMessageReceivedEventArgs args) {
    return Task.Run(() => {
      Console.WriteLine("### RECEIVED ###");
      
      // Parse message from Byte array to string
      string message = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
      if (String.IsNullOrEmpty(message)) {
        Console.WriteLine("MQTT: Empty message");
        _hub.Publish(new InvalidPayloadEvent(){Message = args.ApplicationMessage.Payload});
      }
      // Deserialize message with System.Text.Json
      MqttPayload? payload = JsonSerializer.Deserialize<MqttPayload>(message, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
      
      if (payload != null) {
        Enum.TryParse(payload.Type, true, out PayloadType payloadType);
        _hub.Publish(new ConnectorMessageEvent {
          DeviceId = payload.Id,
          Protocol = ProtocolTypes.Mqtt,
          Timestamp = new DateTime(payload.Timestamp),
          RawPayload = payload.Value,
          PayloadType = payloadType,
        });
      }
      else {
        Console.WriteLine("Could not parse payload from MQTT Connector");
      }
    });
  }
}