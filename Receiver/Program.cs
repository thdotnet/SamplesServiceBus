using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "";
            var sbClient = new ServiceBusClient(connectionString);
            var receiver = sbClient.CreateProcessor(queueName: "demoqueue");

            try
            {
                receiver.ProcessMessageAsync += Receiver_ProcessMessageAsync;
                receiver.ProcessErrorAsync += Receiver_ProcessErrorAsync;

                await receiver.StartProcessingAsync();

                Console.WriteLine("wait for a minute and then press any key to end the processing");
                Console.ReadLine();

                Console.WriteLine("stopping the received...");
                
                await receiver.StopProcessingAsync();

                Console.WriteLine("stopped receiving messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                await receiver.DisposeAsync();
                await sbClient.DisposeAsync();
            }
        }

        private static async Task Receiver_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async Task Receiver_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            Console.WriteLine($"Received: {arg.Message.Body}");
            return;
        }
    }
}
