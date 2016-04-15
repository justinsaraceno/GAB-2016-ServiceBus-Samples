using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver
{
    using Microsoft.Azure;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    class Program
    {
        private static string busConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private static NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(busConnectionString);

        static void Main(string[] args)
        {
            Console.WriteLine("Service Bus Receiver Ready! (PRETEND THIS SENDS NOTIFICATIONS)");
            Console.WriteLine("Listening for incoming messages...");
            ConsumeIncomingMessages();
        }

        static void ConsumeIncomingMessages()
        {
            QueueClient client = QueueClient.CreateFromConnectionString(busConnectionString, "NoticeQueue");

            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            // Callback to handle received messages.
            client.OnMessage((message) =>
            {
                try
                {
                    // Process message from queue.
                    Console.WriteLine("Message Body: " + message.GetBody<string>());
                    Console.WriteLine("Message Id: " + message.MessageId);
                    Console.WriteLine("[Property] To: " + message.Properties["To"]);
                    Console.WriteLine("[Property] Subject: " + message.Properties["Subject"]);
                    Console.WriteLine("[Property] Body: " + message.Properties["Body"]);

                    //throw new Exception("some exception to keep things in the queue");

                    // Remove message from queue.
                    Console.WriteLine("   +++Removing message from queue+++");
                    message.Complete();
                    Console.WriteLine(" ");
                }
                catch (Exception)
                {
                    // Indicates a problem, unlock message in queue.
                    message.Abandon();
                    Console.WriteLine("failed!");
                }
            }, options);

            Console.ReadLine();
        }
    }
}
