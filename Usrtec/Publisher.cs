/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/12/2014
 * Time: 16:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace Usrtec
{
	class Publisher
	{
		// Class variables constructed in constructor then used outside class
		public IConnection connection = null;
		public ISession session = null;
		public IMessageProducer producer = null;
		
		// Constructor
		public Publisher ()
		{
	        String user = env("ACTIVEMQ_USER", "admin");
	        String password = env("ACTIVEMQ_PASSWORD", "password");
	        String host = env("ACTIVEMQ_HOST", "localhost");
	        int port = Int32.Parse(env("ACTIVEMQ_PORT", "61616"));
	
			String brokerUri = "activemq:tcp://" + host + ":" + port;
	        NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);
	
	        try
	        {
	        	connection = factory.CreateConnection(user, password);
	        	connection.Start();
	        }
            catch (System.Exception e)
            {
                Console.WriteLine("==ACTIVE MQ DOWN==");
                Console.WriteLine(e.ToString());
            }	        
		}
		
		public void NewTopic (string destination)
		{
			session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
	        IDestination dest = session.GetTopic(destination);
	        producer = session.CreateProducer(dest);
	        producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
		}
		
		public void Stop ()
		{
			producer.Send(session.CreateTextMessage("SHUTDOWN"));
	        connection.Close();
		}
		
		public void Publish(String data)
		{
			producer.Send(session.CreateTextMessage(data));
		}

	    private static String env(String key, String defaultValue)
		{
	        String rc = System.Environment.GetEnvironmentVariable(key);
	        if (rc == null)
			{
	            return defaultValue;
			}
	        return rc;
	    }
	}
}