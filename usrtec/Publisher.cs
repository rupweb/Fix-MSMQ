/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/12/2014
 * Time: 16:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Messaging;

namespace Usrtec
{
	class Publisher
	{
		// Class variables constructed in constructor then used outside class
		public MessageQueue messageQueue = null;
			
		public void NewTopic (string destination)
		{
			if (MessageQueue.Exists(@".\Private$\" + destination))
			{
				messageQueue = new MessageQueue(@".\Private$\" + destination);
				messageQueue.Label = "Testing Queue";
			}
			else
			{
				// Create the Queue
				MessageQueue.Create(@".\Private$\" + destination);
				messageQueue = new MessageQueue(@".\Private$\" + destination);
				messageQueue.Label = "Newly Created Queue";
			}
		}
		
		public void Publish(String data)
		{
			messageQueue.Send(data, "Title");
		}
	}
}