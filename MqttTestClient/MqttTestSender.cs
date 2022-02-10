using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;

namespace MqttTestClient;

public static class MqttTestSender {

  public static async Task SendTestMessage(string topic, string msgId, string value, string msgType) {
    var factory = new MqttFactory();
    var mqtt = factory.CreateMqttClient();
    await mqtt.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("test.mosquitto.org", 1883).Build(), CancellationToken.None);

    await mqtt.PublishAsync(new MqttApplicationMessage() {
      Payload = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(new MqttPubPayload() {
        Id = msgId,
        Timestamp = DateTime.Now.Ticks,
        Value = value,
        Type = msgType
      })),
      Topic = topic
    });
  }

  private class MqttPubPayload {
    public string Id { get; set; }
    public long Timestamp { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
  }
    
}

