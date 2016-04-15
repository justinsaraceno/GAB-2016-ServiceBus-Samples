using System;

namespace TopicLastNameOtoZSubscriber
{
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    class Program
    {
        private static readonly string busConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private static readonly NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(busConnectionString);
        private static readonly string topicName = "LastNameChange";
        private static readonly string subscriptionName = "OToZLastNameMessages";

        static void Main(string[] args)
        {
            Console.WriteLine("Subscribed to the topic: " + topicName + " filtered by: " + subscriptionName);
            Console.WriteLine("Listening for messages for this subscription [last name changes filtered O-Z]...");
            ConsumeLastNameChangeMessages();
        }

        static void ConsumeLastNameChangeMessages()
        {

            // Create a "AToNLastNameMessages" filtered subscription.
            SqlFilter alphabetGroupFilter =
               new SqlFilter("AlphabetGroup = 2");

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                SubscriptionDescription subscriptionDescription = new SubscriptionDescription(topicName, subscriptionName);
                namespaceManager.CreateSubscription(subscriptionDescription, alphabetGroupFilter);
            }

            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(busConnectionString, topicName, subscriptionName);

            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            client.OnMessage((message) =>
            {
                try
                {
                    // Process message from subscription.
                    Console.WriteLine("\n**[O-Z] Last Name Message**");
                    Console.WriteLine("Body: " + message.GetBody<string>());
                    Console.WriteLine("MessageID: " + message.MessageId);
                    Console.WriteLine("Alphabet Group: " +
                        message.Properties["AlphabetGroup"]);

                    // Remove message from subscription.
                    message.Complete();
                }
                catch (Exception)
                {
                    // Indicates a problem, unlock message in subscription.
                    message.Abandon();
                }
            }, options);

            Console.ReadLine();
        }
    }
}
