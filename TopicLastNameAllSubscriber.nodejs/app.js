var azure = require('azure');
var serviceBusService = azure.createServiceBusService("Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[key]=");
var topicName = "LastNameChange";
var subscriptionName = "AllMessagesForNode";

console.log('***Starting NODE.JS Azure Service Bus Subscriber***');

// Create or look for the service bus message topic
serviceBusService.createTopicIfNotExists(topicName, function (error) {
    if (!error) {
        // Topic was created or exists
        console.log("Topic '" + topicName + "' created or exists...");
    }
});

// Create or look for the service bus subscription
serviceBusService.createSubscription(topicName, subscriptionName, function (error) {
    if (!error) {
        // Subscription was created or exists
    } else {
        console.log("Subscribing to '" + subscriptionName + "'...");
        // Check for messages on a recurring time interval
        setInterval(checkForMessages.bind(null, serviceBusService, topicName, subscriptionName, processMessage.bind(null, serviceBusService)), 5000);
    }
});

function checkForMessages(serviceBusService, topicName, subscriptionName, callback) {
    serviceBusService.receiveSubscriptionMessage(topicName, subscriptionName, function (error, receivedMessage) {
        if (error) {
            if (error === 'No messages to receive') {
                console.log('No messages at: ' + Date());
            } else {
                callback(error);
            }
        } else {
            callback(null, receivedMessage);
        }
    });
}

function processMessage(serviceBusService, err, receivedMessage) {
    if (err) {
        console.log('Error on Message Receive: ', err);
    } else {
        console.log('Message: ', receivedMessage);
    }
}
