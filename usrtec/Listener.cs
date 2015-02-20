/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/12/2014
 * Time: 16:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
 using System;
 using System.Messaging;
 using System.IO;

namespace Usrtec
{
	class Listener
	{
		// Class variables constructed in constructor then used outside class
		public MessageQueue msQueue = null;
		
		public void NewTopic (string destination)
		{			
			if (MessageQueue.Exists(@".\Private$\" + destination))
			{
				msQueue = new MessageQueue(@".\Private$\" + destination);
				msQueue.Label = "Testing Queue";
			}
			else
			{
				// Create the Queue
				MessageQueue.Create(@".\Private$\" + destination);
				msQueue = new MessageQueue(@".\Private$\" + destination);
				msQueue.Label = "Newly Created Queue";
			}					
		}
					
		public string Listen()
		{			
			Console.WriteLine("Listener waiting for messages...");
			while (true)
			{
				System.Messaging.Message m = msQueue.Receive();
				
				// Parse the MSMQ message
				// m.Formatter = new System.Messaging.XmlMessageFormatter(new String[] { });
				m.Formatter = new ActiveXMessageFormatter();				
				StreamReader sr = new StreamReader(m.BodyStream);
				string s = sr.ReadToEnd();
				
				// Get the string data we want out of the XML without a deserialiser. Right?
				int xmlStart = s.IndexOf("<string>") + 8;
				int xmlEnd = s.IndexOf("</string>") - (s.IndexOf("<string>") + 8);
				string command = s.Substring(xmlStart, xmlEnd);
				
				return command;
			}
		}
	}
}