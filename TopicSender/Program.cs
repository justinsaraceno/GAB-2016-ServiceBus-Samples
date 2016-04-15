using System;

namespace TopicSender
{
    using System.Linq;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    class Program
    {
        private static readonly string busConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private static readonly NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(busConnectionString);

        static void Main(string[] args)
        {
            var sleepTime = 250;
            LoadStartMessage(sleepTime);

            Console.WriteLine("Service Bus Topic Sender Ready!");

            while (true)
            {
                Console.WriteLine("Let's change a user's last name.");
                Console.WriteLine(" What is the user's new last name?");
                var lastName = Console.ReadLine();

                SendMessageToLastNameTopicGroup(lastName, DetermineNameGroup(lastName));
                
                Console.WriteLine($"\n ++Message Sent for last name: {lastName}++");
                Console.WriteLine(" ");
            }
        }

        static int DetermineNameGroup(string lastName)
        {
            char[] groupOneChars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n'};
            // Put all last names a-n in group 1; the rest are group 2. This will be used for subscribers that want to filter by group.
            return !string.IsNullOrEmpty(lastName) && groupOneChars.Any(c => c == lastName.ToLower()[0]) ? 1 : 2;
        }

        static void SendMessageToLastNameTopicGroup(string lastName, int groupNumber)
        {
            // configure the topic settings
            var topicName = "LastNameChange";
            TopicDescription topicDescription = new TopicDescription(topicName);
            topicDescription.MaxSizeInMegabytes = 5120;
            topicDescription.DefaultMessageTimeToLive = new TimeSpan(0, 1, 0);

            // create a new topic with custom settings
            if (!namespaceManager.TopicExists(topicName))
            {
                namespaceManager.CreateTopic(topicDescription);
            }

            TopicClient topicClient = TopicClient.CreateFromConnectionString(busConnectionString, topicName);
            BrokeredMessage message = new BrokeredMessage(lastName);
            Random random = new Random();
            message.Properties["UserId"] = random.Next(1, 10);
            message.Properties["AlphabetGroup"] = groupNumber; // this will be used by subscribers to filter
            topicClient.Send(message);
        }

        static void LoadStartMessage(int sleepTime)
        {
            Console.WriteLine(@" ___________ _____ _   _ _   _ _____ ");
            System.Threading.Thread.Sleep(sleepTime);
            Console.WriteLine(@"|_   _| ___ \_   _| \ | | | | |  __ \");
            System.Threading.Thread.Sleep(sleepTime);
            Console.WriteLine(@"  | | | |_/ / | | |  \| | | | | |  \/");
            System.Threading.Thread.Sleep(sleepTime);
            Console.WriteLine(@"  | | |    /  | | | . ` | | | | | __ ");
            System.Threading.Thread.Sleep(sleepTime);
            Console.WriteLine(@"  | | | |\ \ _| |_| |\  | |_| | |_\ \");
            System.Threading.Thread.Sleep(sleepTime);
            Console.WriteLine(@"  \_/ \_| \_|\___/\_| \_/\___/ \____/");
            Console.WriteLine(@"");
        }
    }
}
