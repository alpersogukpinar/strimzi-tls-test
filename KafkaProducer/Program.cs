using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaConsumer
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string topicName = args[0];


            var config = new ProducerConfig
            {
                //EnableSslCertificateVerification = false,
                BootstrapServers = "bootstrap.myingress.com:443",
                SecurityProtocol = SecurityProtocol.Ssl,
                SslCaLocation = "./tmp/ca-root.crt",
                SslCertificateLocation = "./tmp/user.crt",
                SslKeyLocation = "./tmp/user.key",
                //Debug = "broker",
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                Console.WriteLine($"{producer.Name} producing on {topicName}. q to exit.");

                string text;
                while ((text = Console.ReadLine()) != "q")
                {
                    try
                    {
                        var dr = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = text });
                        Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }
                }


                //producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
