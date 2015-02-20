/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 10/12/2014
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using Apache.NMS;

using QuickFix;
using QuickFix.Fields;

namespace Usrtec
{
	public class Demo
	{	
		Session _market_data_session = null;
		Session _trading_session = null;
			    
		public void go(SessionID s1, SessionID s2)
		{				
			_market_data_session = Session.LookupSession(s1);
			_trading_session = Session.LookupSession(s2);
			
			// Set up the activemq layer listening to user commands
			Listener ct_to_fix = new Listener();
			ct_to_fix.NewTopic("cttofix");
						
			Console.WriteLine("");
			Console.WriteLine("USE:");
		    Console.WriteLine("md: market data snapshot in 1mio EURUSD");
		    Console.WriteLine("mr: market data stream in 1mio EURUSD");		
		    Console.WriteLine("u: unsubscribe from market data stream");				    
		    Console.WriteLine("o: place new order to buy 1mio EURUSD");	
		    Console.WriteLine("q: exit program");			    
				
		    bool x = true;
		    while (x)
		    {
		    	string inp = ct_to_fix.Listen();
				switch (inp)
				{
					case "md":
						md();
						break;
						
					case "mr":
						mr();
						break;
						
					case "u":
						mu();
						break;						
						
					case "o":
						nos();
						break;
						
					case "q":
						x = false;
						break;
						
					default:
						Console.WriteLine("Input error...");
						break;
				}
		    }
		    
		    // Shut down activeMQ listener
		    ct_to_fix.Stop();
		}
		
		public void md()
		{			
            QuickFix.FIX43.MarketDataRequest mdr = MarketDataRequest43();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}
		
		public void mr()
		{			
            QuickFix.FIX43.MarketDataRequest mdr = MarketDataRefresh43();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}
		
		public void mu()
		{			
            QuickFix.FIX43.MarketDataRequest mdr = MarketDataUnsubscribe43();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}		
		
		public void nos()
		{
            QuickFix.FIX43.NewOrderSingle order = NewOrderSingle43();

            if (order != null)
            {
                order.Header.GetField(Tags.BeginString);
                SendOrder(order);
            }
		}
		
		private void SendMDR(Message m)
        {			
			if (_market_data_session != null)
               _market_data_session.Send(m);
            else
            {
                Console.WriteLine("Can't send message: market data session not created.");
            }
        }
		
		private void SendOrder(Message m)
        {			
			if (_trading_session != null)
               _trading_session.Send(m);
            else
            {
                Console.WriteLine("Can't send message: trading session not created.");
            }
        }
		
		private QuickFix.FIX43.MarketDataRequest MarketDataRequest43()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT);
            MarketDepth marketDepth = new MarketDepth(1);
            
            QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

            QuickFix.FIX43.MarketDataRequest message = new QuickFix.FIX43.MarketDataRequest(mdReqID, subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);
            
            // Give security type for foreign exchange contract
            // SecurityType securityType = new SecurityType("FOR");            
            // message.SetField(securityType);
                       
 			// Settlement date left blank for SPOT value
            // FutSettDate futSettDate = new FutSettDate("");
 			// message.SetField(futSettDate);
 			
 			// GainGTX custom tag 7820 set to SPOT
 			message.SetField(new StringField(7820, ""));

            return message;
        }
		
		private QuickFix.FIX43.MarketDataRequest MarketDataRefresh43()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES);
            MarketDepth marketDepth = new MarketDepth(1);
            
            QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

            QuickFix.FIX43.MarketDataRequest message = new QuickFix.FIX43.MarketDataRequest(mdReqID, subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);
 			
 			// GainGTX custom tag 7820 set to SPOT
 			message.SetField(new StringField(7820, ""));

            return message;
        }

		private QuickFix.FIX43.MarketDataRequest MarketDataUnsubscribe43()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.DISABLE_PREVIOUS);
            MarketDepth marketDepth = new MarketDepth(1);
            
            QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX43.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX43.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

            QuickFix.FIX43.MarketDataRequest message = new QuickFix.FIX43.MarketDataRequest(mdReqID, subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);
 			
 			// GainGTX custom tag 7820 set to SPOT
 			message.SetField(new StringField(7820, ""));

            return message;
        }		
		
        private QuickFix.FIX43.NewOrderSingle NewOrderSingle43()
        {
            // Ensure each order has unique order id for today
        	QuickFix.Fields.ClOrdID clOrdID = new ClOrdID("USRTEC_"
        	                            					+ DateTime.Now.Hour 
        	                            					+ DateTime.Now.Minute
        	                            					+ DateTime.Now.Second);
        	
            QuickFix.Fields.HandlInst handlInst = new HandlInst('1');
            QuickFix.Fields.Symbol symbol = new Symbol("EUR/USD");
            QuickFix.Fields.Side side = new Side('1');
            QuickFix.Fields.TransactTime time = new TransactTime(DateTime.Now);
            QuickFix.Fields.OrdType ordType = new OrdType(OrdType.MARKET);
            // QuickFix.Fields.OrdType ordType = new OrdType(OrdType.LIMIT);            

            QuickFix.FIX43.NewOrderSingle newOrderSingle = new QuickFix.FIX43.NewOrderSingle(
            	clOrdID, handlInst, symbol, side, time, ordType);

            newOrderSingle.Set(new OrderQty(Convert.ToDecimal("1000000")));
                     
			// GTC order at some price. The ordType needs to be LIMIT
            // newOrderSingle.Set(new TimeInForce(TimeInForce.GOOD_TILL_CANCEL));
            // newOrderSingle.Set(new Price(Convert.ToDecimal(1.50)));
            
            // For IOC orders ordType needs to be MARKET
            newOrderSingle.Set(new TimeInForce(TimeInForce.IMMEDIATE_OR_CANCEL));
            // newOrderSingle.Set(new StopPx(Convert.ToDecimal("2.00")));
            
            return newOrderSingle;
        }		
		
	}
}