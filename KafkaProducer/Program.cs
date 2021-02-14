using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string topicName = args[0];

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "localhost:9093" },
                { "security.protocol", "SSL" },
                { "ssl.ca.location", @"D:\Development\Repos\KafkaProducer\KafkaProducer\tmpca-root.crt" },
                { "debug", "security" }
            };

            var mutualAuthConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", "localhost:9093" },
                { "security.protocol", "SSL" },
                { "ssl.ca.location", @"D:\Development\Repos\KafkaProducer\KafkaProducer\tmp\ca-root.crt" },
                { "ssl.certificate.location", @"D:\Development\Repos\KafkaProducer\KafkaProducer\tmp\localhost_client.crt" },
                { "ssl.key.location", @"D:\Development\Repos\KafkaProducer\KafkaProducer\tmp\localhost_client.key" },
                { "debug", "security" }
            };

            using (var producer = new Producer<Null, string>(mutualAuthConfig, null, new StringSerializer(Encoding.UTF8)))
            {
                Console.WriteLine($"{producer.Name} producing on {topicName}. q to exit.");

                producer.OnError += (_, error)
                    => Console.WriteLine($"Error: {error}");

                string text;
                while ((text = Console.ReadLine()) != "q")
                {
                    var deliveryReport = producer.ProduceAsync(topicName, null, text);
                    deliveryReport.ContinueWith(task =>
                    {
                        Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                    });
                }

                
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
