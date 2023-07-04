using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;

namespace AsyncQueue
{
    /**
     * Klasse für senden und empfangen von Nachrichten
     */
    public class MessageQueueConnector
    {
        private const string ActiveMqUri = "tcp://localhost:61616";
        private const string QueueName = "TestQueue";


        /**
         * Für normale Strings
         */
        public void SendMessageToQueueAsString(string message)
        {
            // Erstellen der Verbindungs- und Sitzungsobjekte
            IConnectionFactory connectionFactory = new ConnectionFactory(ActiveMqUri);
            using (Apache.NMS.IConnection connection = connectionFactory.CreateConnection())
            using (Apache.NMS.ISession session = connection.CreateSession())
            {
                // Erstellen der Warteschlange
                IDestination destination = session.GetQueue(QueueName);

                // Erstellen des Nachrichtenproduzenten
                IMessageProducer producer = session.CreateProducer(destination);

                // Erstellen der Nachricht
                ITextMessage textMessage = session.CreateTextMessage(message);

                // Senden der Nachricht an die Warteschlange
                producer.Send(textMessage);
                Console.WriteLine("Nachricht erfolgreich in die Warteschlange gesendet.");
            }
        }

        /**
         * Für JSON-Objekte
         */
        public void SendMessageToQueue(object data)
        {
            string json = JsonConvert.SerializeObject(data);

            // Erstellen der Verbindungs- und Sitzungsobjekte
            IConnectionFactory connectionFactory = new ConnectionFactory(ActiveMqUri);
            using (Apache.NMS.IConnection connection = connectionFactory.CreateConnection())
            using (Apache.NMS.ISession session = connection.CreateSession())
            {
                // Erstellen der Warteschlange
                IDestination destination = session.GetQueue(QueueName);

                // Erstellen des Nachrichtenproduzenten
                IMessageProducer producer = session.CreateProducer(destination);

                // Erstellen der Textnachricht mit JSON-Inhalt
                ITextMessage textMessage = session.CreateTextMessage(json);

                // Senden der Nachricht an die Warteschlange
                producer.Send(textMessage);
                Console.WriteLine("Nachricht erfolgreich in die Warteschlange gesendet.");
            }
        }

        public void ReceiveMessageFromQueue()
        {
            // Erstellen der ConnectionFactory
            IConnectionFactory connectionFactory = new ConnectionFactory(ActiveMqUri);
            using (Apache.NMS.IConnection connection = connectionFactory.CreateConnection())
            using (Apache.NMS.ISession session = connection.CreateSession())
            {
                // Erstellen der Warteschlange
                IDestination destination = session.GetQueue(QueueName);

                // Erstellen des Nachrichtenempfängers
                IMessageConsumer consumer = session.CreateConsumer(destination);

                // Empfangen der Nachricht
                connection.Start();
                IMessage message = consumer.Receive();
                if (message is ITextMessage textMessage)
                {
                    string content = textMessage.Text;
                    try
                    {
                        // Überprüfen, ob der Inhalt ein gültiger JSON-String ist
                        dynamic jsonData = JsonConvert.DeserializeObject(content);
                        if (jsonData != null)
                        {
                            Console.WriteLine("Empfangener JSON-Inhalt:");
                            Console.WriteLine(jsonData.ToString());
                            return;
                        }
                    }
                    catch (JsonException)
                    {
                        // Der Inhalt ist kein gültiger JSON-String
                        Console.WriteLine("Empfangener Textinhalt:");
                        Console.WriteLine(content);
                        return;
                    }
                }

                Console.WriteLine("Keine Textnachricht in der Warteschlange gefunden.");
            }
        }


        public async Task ReceiveMessageFromQueueAsync()
        {
            // Erstellen der Verbindungs- und Sitzungsobjekte
            IConnectionFactory connectionFactory = new ConnectionFactory(ActiveMqUri);
            using (IConnection connection = connectionFactory.CreateConnection())
            using (Apache.NMS.ISession session = connection.CreateSession())
            {
                // Erstellen der Warteschlange
                IDestination destination = session.GetQueue(QueueName);

                // Erstellen des Nachrichtenempfängers
                IMessageConsumer consumer = session.CreateConsumer(destination);

                // Registrieren des MessageListeners
                consumer.Listener += async message =>
                {
                    if (message is ITextMessage textMessage)
                    {
                        string content = textMessage.Text;
                        try
                        {
                            // Überprüfen, ob der Inhalt ein gültiger JSON-String ist
                            dynamic jsonData = JsonConvert.DeserializeObject(content);
                            if (jsonData != null)
                            {
                                Console.WriteLine("Empfangener JSON-Inhalt:");
                                Console.WriteLine(jsonData.ToString());
                            }
                        }
                        catch (JsonException)
                        {
                            // Der Inhalt ist kein gültiger JSON-String
                            Console.WriteLine("Empfangener Textinhalt:");
                            Console.WriteLine(content);
                        }
                    }

                    await Task.CompletedTask;
                };

                // Starten der Verbindung
                connection.Start();

                Console.WriteLine("Warte auf Nachrichten. Drücke eine beliebige Taste, um den Empfang zu beenden.");
                Console.ReadKey();

                // Schließen der Verbindung
                await Task.Run(() => connection.Close());
            }
        }
    }

    /**
     * Für den Test startete ich einfach implizit die void Main funktion.
     * Weiters verwendete ich von Apache Activemq
     * activemq    1581       1  0 12:08 ?        00:00:28 /usr/bin/java -Djava.util.logging.config.file=logging.properties -Djava.security.auth.login.config=/opt/activemq//conf/login.config -Djava.awt.headless=true -Djava.io.tmpdir=/opt/activemq//tmp -Dactivemq.classpath=/opt/activemq//conf:/opt/activemq//../lib/: -Dactivemq.home=/opt/activemq/ -Dactivemq.base=/opt/activemq/ -Dactivemq.conf=/opt/activemq//conf -Dactivemq.data=/opt/activemq//data -jar /opt/activemq//bin/activemq.jar start
     * Läuft auf localhost:61616
     * 
     */

    //TODO: Zum Testen einen der beiden Teile einkommentieren und den anderen auskommentieren 

    //Synchron Sequentiell
    /*public class Program
    {
        public static void Main(string[] args)
        {
            MessageQueueConnector connector = new MessageQueueConnector();

            // Nachricht in die Warteschlange schreiben
            // connector.SendMessageToQueue("Testnachricht."); -> Würde sonst als JSON serialisiert werden

            connector.SendMessageToQueueAsString("Normaler Text");

            // Nachricht aus der Warteschlange lesen
            connector.ReceiveMessageFromQueue();

            // Nachricht in die Warteschlange schreiben - Diese Mal als JSON
            var data = new { Name = "Stefan Halper", Age = 99 };
            connector.SendMessageToQueue(data);

            // Nachricht aus der Warteschlange lesen
            connector.ReceiveMessageFromQueue();


            Console.ReadLine();
        }
    }
    */

    //Asynchron in die Richtung von onMessage 
    public class Program
    {
        public static async Task Main(string[] args)
        {
            MessageQueueConnector connector = new MessageQueueConnector();

            // Nachricht in die Warteschlange schreiben
            connector.SendMessageToQueue("Dies ist ein normaler Text");

            // Nachrichten aus der Warteschlange empfangen
            await connector.ReceiveMessageFromQueueAsync();

            Console.ReadLine();
        }
    }

    //Ich hoffe das passt soweit.
}