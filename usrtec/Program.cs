/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 10/12/2014
 * Time: 12:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Threading;
using QuickFix;

namespace Usrtec
{
    class Program
    {
        [STAThread]
		public static void Main(string[] args)
		{
			Console.WriteLine("In Main()");
		
            try
            {			          	
            	SessionSettings settings = new SessionSettings(args[0]);
            	Console.Write("settings: " + settings.ToString());

				IApplication app = new Engine();
				IMessageStoreFactory storeFactory = new FileStoreFactory(settings); 
				ILogFactory logFactory = new FileLogFactory(settings); 
				// MessageFactory messageFactory = new DefaultMessageFactory(); 
				IInitiator i = new QuickFix.Transport.SocketInitiator(app, storeFactory, settings, logFactory); 
							
				i.Start();
				Thread.Sleep(3000); 
				
				Demo demo = new Demo();
				SessionID s1 = (SessionID) i.GetSessionIDs().First();
				// SessionID s2 = (SessionID) i.GetSessionIDs().Last();
				demo.go(s1);
		
				i.Stop(); 
				
				Console.Write("Out Main()");
				Console.ReadKey(true);
			}
            catch (System.Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
                Console.ReadKey(true);
            }
		}
    }
}