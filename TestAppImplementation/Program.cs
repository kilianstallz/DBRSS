// See https://aka.ms/new-console-template for more information
using System;
using MqttTestClient;
namespace TestAppImplementation
{
    class Program
    {
        static async Task Main(string[] args) {
            var dbrss = DBRSS.DBRSS.GetApp();
            await dbrss.Run();
            
            Console.WriteLine("Hello World!");

            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");
            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");
            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");
            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");
            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");
            await MqttTestSender.SendTestMessage("DBRSS", "1", "1", "Int");

            var sendPayload = dbrss.ReadBufferPacket();
            Console.WriteLine(sendPayload.Hash);
            dbrss.AcknowledgeBufferPacket(sendPayload.Package[0].Id);

            Console.ReadKey(false);
        }
    }
}