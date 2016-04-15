using System;

namespace QueueSender
{
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    class Program
    {
        private static string busConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private static NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(busConnectionString);

        static void Main(string[] args)
        {
            Console.WriteLine("Service Bus Queue Sender Ready! (PRETEND THIS HANDLES QUEUING EMAILS)");
            
            while (true)
            {
                Console.WriteLine("Press any key to send a Notification message to the queue... ");
                var input = Console.ReadKey();
                SendMessageToQueue();
                Console.WriteLine($"\n ++Message Sent at: {DateTime.Now}++");
                Console.WriteLine(" ");
            }
        }

        static void SendMessageToQueue()
        {
            // set-up queue
            if (!namespaceManager.QueueExists("NoticeQueue"))
            {
                namespaceManager.CreateQueue("NoticeQueue");
            }

            // send message to queue
            QueueClient client = QueueClient.CreateFromConnectionString(busConnectionString, "NoticeQueue");
            DateTime messageDateTime = DateTime.Now;
            BrokeredMessage message = new BrokeredMessage("Notification Message " + messageDateTime);
            message.Properties["To"] = "justin.saraceno@gmail.com";
            message.Properties["Subject"] = "This is message " + messageDateTime;
            message.Properties["Body"] = "Hello world, this is a test message.";
            client.Send(message);
        }
    }
}
