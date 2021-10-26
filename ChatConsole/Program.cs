using System;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Threading.Tasks;

namespace ChatConsole
{
    class Program
    {
        static string _connectionString = "";
        static string _topicPath = "chattopic";
        static async Task Main(string[] args)
        {
            Console.WriteLine("enter your name:");
            var name = Console.ReadLine();

            var client = new ServiceBusClient(_connectionString);
            var adm = new ServiceBusAdministrationClient(_connectionString);

            var topicExists = await adm.TopicExistsAsync(_topicPath);
            if (!topicExists)
            {
                await adm.CreateTopicAsync(_topicPath);
            }

            await adm.CreateSubscriptionAsync(new CreateSubscriptionOptions(_topicPath, name) {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            });

            var processor = client.CreateProcessor(_topicPath, name);
            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor.StartProcessingAsync();

            var helloMessage = new ServiceBusMessage($"{name} has entered the room");

            var sender = client.CreateSender(_topicPath);
            await sender.SendMessageAsync(helloMessage);

            while (true)
            {
                var text = Console.ReadLine();
                if (text.Equals("exit")) break;

                var chatMessage = new ServiceBusMessage($"{name}> {text}");
                await sender.SendMessageAsync(chatMessage);
            }

            var goodbyeMessage = new ServiceBusMessage($"{name} has exited the room");
            await sender.SendMessageAsync(goodbyeMessage);

            await client.DisposeAsync();
            await sender.DisposeAsync();
        }

        private static async Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            Console.WriteLine(arg.Message.Body);
        }
    }
}
