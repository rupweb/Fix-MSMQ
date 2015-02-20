/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 10/12/2014
 * Time: 12:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using QuickFix;
using QuickFix.Fields;

namespace Usrtec
{
	public class Engine : MessageCracker, QuickFix.IApplication
	{	
        // There's only 1 FIX session here, so global this to be used outside this class...
		public Session _market_data_session = null;
		public Session _trading_session = null;
		
		// Set up the activemq layer to publish FIX messages to activeMQ
		Publisher fixin = new Publisher();
		Publisher fixout = new Publisher();
		Publisher price = new Publisher();
		Publisher trade = new Publisher();		
		
		// Constructor sets up activeMQ topics
		public Engine()
		{
			// FIX messages from providers are published on these topics
			// General message dump
			fixin.NewTopic("fixin");
			fixout.NewTopic("fixout");
			// Specific topics
			price.NewTopic("fixtypeS");
			trade.NewTopic("fixtype8");
		}
		
		#region QuickFix.Application Methods

        public void FromApp(Message msg, SessionID s)
        {
        	Console.WriteLine("IN: " + msg.ToString());
        	
        	// Publish all raw FIX messages to message dump topic
        	// Message dump includes heartbeats, logon messages, admin messages etc
        	// Specific message types have to be cracked first (see line after next)
        	fixin.Publish(msg.ToString());
            
        	// Crack FIX message any way you want to
        	Crack(msg, s);
        }

        public void ToApp(Message msg, SessionID s)
        {
            Console.WriteLine("OUT: " + msg.ToString());     	
        	fixout.Publish(msg.ToString());
        	
        	// Don't send out unsupported message type messages to providers
            if (msg.Header.GetField(QuickFix.Fields.Tags.MsgType) == MsgType.BUSINESS_MESSAGE_REJECT)
            {
            	Console.WriteLine("ERRO ToApp BMR: " + msg.ToString());
            	return;
            }
            
            // Don't send out reject messages to providers            
            if (msg.Header.GetField(QuickFix.Fields.Tags.MsgType) == MsgType.REJECT)
            {
            	Console.WriteLine("ERRO ToApp R: " + msg.ToString());
            	return;
            }              
        }

        public void FromAdmin(Message msg, SessionID s) 
        { 
        	Console.WriteLine("Called FromAdmin: " + s.ToString());
        	Console.WriteLine("IN: " + msg.ToString());
        	fixin.Publish(msg.ToString());
        }
        public void OnCreate(SessionID s) 
        {
			Console.WriteLine("Called OnCreate: " + s.ToString());
        }
        public void OnLogout(SessionID s)        
        {
			Console.WriteLine("Called OnLogout: " + s.ToString());		
        }
        public void OnLogon(SessionID s)
        {
			Console.WriteLine("Called OnLogon: " + s.ToString());   			
        }
        public void ToAdmin(Message msg, SessionID s)
        {               	
        	Console.WriteLine("Called ToAdmin: " + s.ToString());
        	Console.WriteLine("OUT: " + msg.ToString());    
        	fixout.Publish(msg.ToString());
        	
        	// Don't send out unsupported message type messages to providers
            if (msg.Header.GetField(QuickFix.Fields.Tags.MsgType) == MsgType.BUSINESS_MESSAGE_REJECT)
            {
            	Console.WriteLine("ERRO ToAdmin BMR: " + msg.ToString());
            	return;
            }
            
            // Don't send out reject messages to providers            
            if (msg.Header.GetField(QuickFix.Fields.Tags.MsgType) == MsgType.REJECT)
            {
            	Console.WriteLine("ERRO ToAdmin R: " + msg.ToString());
            	return;
            }
        	     	
        	if (msg.Header.GetField(QuickFix.Fields.Tags.MsgType) == MsgType.LOGON)
        	{
        		if (s.SenderCompID.Equals("demo417_md"))
        		{
        			// Market Data session
        			Console.WriteLine("Market data session");
        			msg.Header.SetField(new QuickFix.Fields.Username("demo417_md"));
         			msg.Header.SetField(new QuickFix.Fields.Password("webster7"));
         			_market_data_session = Session.LookupSession(s);
        		}
        		if (s.SenderCompID.Equals("demo417_om"))
        		{
        			// Trading session
        			Console.WriteLine("Trading session");
        			msg.Header.SetField(new QuickFix.Fields.Username("demo417_om"));
         			msg.Header.SetField(new QuickFix.Fields.Password("webster7"));
         			_trading_session = Session.LookupSession(s);
        		}        		     		
        	}   	
        }
        #endregion
        
		#region MessageCracker overloads
		public void OnMessage(QuickFix.FIX44.QuoteRequestReject msg, SessionID s)
		{
			Console.WriteLine("QuoteRequestReject from: " + s.ToString());
		}
		
		public void OnMessage(QuickFix.FIX44.Quote msg, SessionID s) 
		{
			Console.WriteLine("Quote from: " + s.ToString());
			// Publish price snapshot message to fixtypeS
			price.Publish(msg.ToString());
		}		
		
		public void OnMessage(QuickFix.FIX44.QuoteCancel msg, SessionID s) 
		{
			Console.WriteLine("QuoteCancel from: " + s.ToString());
			// price.Publish(msg.ToString());
		}	
		
		public void OnMessage(QuickFix.FIX44.MarketDataSnapshotFullRefresh msg, SessionID s) 
		{
			Console.WriteLine("MarketDataSnapshot from: " + s.ToString());
			// Publish price snapshot message to fixtypeS
			price.Publish(msg.ToString());
		}
		
		public void OnMessage(QuickFix.FIX44.MarketDataIncrementalRefresh msg, SessionID s) 
		{
			Console.WriteLine("MarketDataRefresh from: " + s.ToString());
		}
		
		public void OnMessage(QuickFix.FIX44.MarketDataRequestReject msg, SessionID s) 
		{
			Console.WriteLine("MarketDataReject from: " + s.ToString());
		}
			
		public void OnMessage(QuickFix.FIX44.BusinessMessageReject msg, SessionID s) 
		{
			Console.WriteLine("BusinessMessageReject from: " + s.ToString());
		}
		
		public void OnMessage(QuickFix.FIX44.TradingSessionStatus msg, SessionID s) 
		{
			Console.WriteLine("TradingSessionStatus from: " + s.ToString());
		}		
		
        public void OnMessage(QuickFix.FIX44.OrderCancelReject msg, SessionID s) 
        { 
			Console.WriteLine("OrderCancelReject from: " + s.ToString());
        }
        
        public void OnMessage(QuickFix.FIX44.ExecutionReport msg, SessionID s) 
        {
			Console.WriteLine("ExecutionReport from: " + s.ToString());
			// Publish all execution reports to fixtype8
			trade.Publish(msg.ToString());
        }

        public void OnMessage(QuickFix.FIX44.News msg, SessionID s)
        {
			Console.WriteLine("News from: " + s.ToString());  
        }
        #endregion     
	}
}