using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
namespace MQTTApp
{
    internal class Publish
    {
        
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Subscriber...");
                //Creazione Client
                var factory = new MqttFactory();
                var _Client = factory.CreateMqttClient();
                string clientid = Guid.NewGuid().ToString();

                Console.WriteLine("Confiruando Opzioni...");
                //Configurazione Opzioni
                var _options = new MqttClientOptionsBuilder()
                    .WithClientId(clientid)
                    .WithTcpServer("broker.mqttdashboard.com", 1883)
                    .WithCredentials("Gabriele", "Squeo")
                    .WithCleanSession()
                    .Build();

                //Connessione
                _Client.ConnectAsync(_options).Wait();

                if (_Client.IsConnected)
                {
                    Console.WriteLine("1)Pubblica:\n2)Ricevi:");
                    string input = Console.ReadLine();
                    if (input == "1")
                    {
                        Console.WriteLine("Pubblicando Messaggio...");

                        //Pubblicazione Messaggio
                        var applicationMessage = new MqttApplicationMessageBuilder()
                           .WithTopic("panetti/test")
                           .WithPayload("Ciao a tutti")
                           .Build();

                        _Client.PublishAsync(applicationMessage, CancellationToken.None);

                        Console.WriteLine("MQTT application message is published.");
                    }
                    else
                    {
                        //Sottoscrizione

                        var mqttSubscribeOptions = factory.CreateSubscribeOptionsBuilder()
                           .WithTopicFilter(f => { f.WithTopic("panetti/test"); })
                           .Build();

                        _Client.SubscribeAsync(mqttSubscribeOptions).Wait();
                        
                        //Lettura Messaggi
                        while (true)
                        {
                            _Client.UseApplicationMessageReceivedHandler(e =>
                            {
                                try
                                {
                                    string topic = e.ApplicationMessage.Topic;

                                    if (string.IsNullOrWhiteSpace(topic) == false)
                                    {
                                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                                        Console.WriteLine($"Topic1: {topic}. Message Received: {payload}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message, ex);
                                }
                            });
                        }
                        
                    }
                    
                }
                else
                {
                    Console.WriteLine("Non connesso.");
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
