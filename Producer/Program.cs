using System;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "";

            var sbClient = new ServiceBusClient(connectionString);
            var sender = sbClient.CreateSender(queueOrTopicName: "demoqueue");

            try
            {
                
                for (int i = 0; i < 10; i++)
                {
                    var content = $"Message: {i}";

                    var message = new ServiceBusMessage(content);
                    await sender.SendMessageAsync(message);

                    Console.WriteLine($"Sent: {i}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }
    }
}
