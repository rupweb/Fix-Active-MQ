/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/12/2014
 * Time: 16:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
 using System;

using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace Usrtec
{
	class Listener
	{
		// Class variables constructed in constructor then used outside class
		public IConnection connection = null;
		public ISession session = null;
		public IMessageConsumer consumer = null;		
		
		// Constructor
		public Listener()
		{
			Console.WriteLine("Starting up Listener.");			
						
	        String user = env("ACTIVEMQ_USER", "admin");
	        String password = env("ACTIVEMQ_PASSWORD", "password");
	        String host = env("ACTIVEMQ_HOST", "localhost");
	        int port = Int32.Parse(env("ACTIVEMQ_PORT", "61616"));
	        String destination = "fixin";
	
			String brokerUri = "activemq:tcp://" + host + ":" + port + "?transport.useLogging=true";
	        NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);
	
	        connection = factory.CreateConnection(user, password);
	        connection.Start();
	        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
	        IDestination dest = session.GetTopic(destination);
	
	        consumer = session.CreateConsumer(dest);
		}
	        
		public void Stop()
		{
			Console.WriteLine("Shutting down Listener.");			
			connection.Close();
			Console.ReadLine();
	    }
		
		public string Listen()
		{			
			Console.WriteLine("Listener waiting for messages...");
			while (true) 
			{
	            IMessage msg = consumer.Receive();
	            if (msg is ITextMessage) 
				{
					ITextMessage txtMsg = msg as ITextMessage;
	                return txtMsg.Text;
	            }
				else 
				{
	                Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
	            }
	        }
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