/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 10/12/2014
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using QuickFix;
using QuickFix.Fields;

namespace Usrtec
{
	public class Demo
	{	
		Session _session = null;
		
		// This is a hack to global the quote id because, well, it needs to be discovered when using QuoteCancel.
		QuoteID _rfq_id = null;
		
		// Set up the rfq quote list
		// Rfq3 rfq_and_quote = new Rfq3();
		
		public void go(SessionID s1)
		{				
			_session = Session.LookupSession(s1);
	
			// Set up the msmq layer listening to user commands
			Listener ct_to_fix = new Listener();
			ct_to_fix.NewTopic("cttofix");
						
			Console.WriteLine("");
			Console.WriteLine("USE:");
			Console.WriteLine("q: quote request for 1mio EURUSD");
			Console.WriteLine("qc: quote cancel");			
		    Console.WriteLine("md: market data snapshot in 1mio EURUSD");
		    Console.WriteLine("mr: market data stream in 1mio EURUSD");		
		    Console.WriteLine("u: unsubscribe from market data stream");				    
		    Console.WriteLine("o: place new order to buy 1mio EURUSD");	
			Console.WriteLine("logout: logout");	
			Console.WriteLine("logon: logon");			
		    Console.WriteLine("quit: exit program");			    
				
		    bool x = true;
		    while (x)
		    {
		    	string inp = ct_to_fix.Listen();
				switch (inp)
				{
						
					case "q":
						q();
						break;
						
					case "qc":
						qc();
						break;
						
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
						
					case "logon":
						logon();
						break;
						
					case "logout":
						logout();
						break;
						
					case "quit":
						x = false;
						break;
						
					default:
						Console.WriteLine("Input error...");
						break;
				}
		    }
		    
		    // Don't shut down MSMQ listener
		}
		
		public void q()
		{			
			Quote o = new Quote();
			o.id = "TEST_" + DateTime.Now.ToString("hhmmssfff");
			o.instrument = "EUR/USD";
			o.side = "BUY";
			o.size = 1000000;
			o.account = "357647";
			// Get spot settlement date without including holidays. This could be a nitemare across all countries... 
			// TODO: A holiday checker?
			o.value_date = Utils.AddBusinessDays(DateTime.Today, 2).ToString("yyyyMMdd");
			
			QuickFix.FIX44.QuoteRequest qr = QuoteRequest44(o);

            if (qr != null)
            {
                qr.Header.GetField(Tags.BeginString);
                SendMDR(qr);
            }
		}
		
		public void qc()
		{			
			QuickFix.FIX44.QuoteCancel qcl = QuoteCancel44();

            if (qcl != null)
            {
                qcl.Header.GetField(Tags.BeginString);
                SendMDR(qcl);
            }
		}
		
		public void md()
		{			
            QuickFix.FIX44.MarketDataRequest mdr = MarketDataRequest44();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}
		
		public void mr()
		{			
            QuickFix.FIX44.MarketDataRequest mdr = MarketDataRefresh44();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}
		
		public void mu()
		{			
            QuickFix.FIX44.MarketDataRequest mdr = MarketDataUnsubscribe44();

            if (mdr != null)
            {
                mdr.Header.GetField(Tags.BeginString);
                SendMDR(mdr);
            }
		}		
		
		public void nos()
		{
			Order o = new Order();
			
			o.id = _rfq_id.ToString();
			// Need to look up the latest quote_id for this rfq from the RFQ QUOTE struct
			o.quote_id = Rfq.get_rfq_quote(o.id);
			o.instrument = "EUR/USD";
			o.side = "BUY";
			o.size = 1000000;
			o.account = "357647";
			o.value_date = Utils.AddBusinessDays(DateTime.Today, 2).ToString("yyyyMMdd");			
			
            // A new order requires a PREVIOUSLY_QUOTED quote id. Get that from the quote array.
            			
			QuickFix.FIX44.NewOrderSingle order = NewOrderSingle44(o);

            if (order != null)
            {
                order.Header.GetField(Tags.BeginString);
                SendOrder(order);
            }
		}
		
		public void logon()
		{
			if (_session != null)
               _session.Logon();
            else
            {
                Console.WriteLine("Can't logon: session not created.");
            }
		}
		
		public void logout()
		{
			if (_session != null)
               _session.Logout();
            else
            {
                Console.WriteLine("Can't logout: session not created.");
            }
		}
		
		private void SendRFQ(Message m)
        {			
			if (_session != null)
               _session.Send(m);
            else
            {
                Console.WriteLine("Can't send RFQ: session not created.");
            }
        }
		
		private void SendMDR(Message m)
        {			
			if (_session != null)
               _session.Send(m);
            else
            {
                Console.WriteLine("Can't send request: market data session not created.");
            }
        }
		
		private void SendOrder(Message m)
        {			
			if (_session != null)
               _session.Send(m);
            else
            {
                Console.WriteLine("Can't send order: trading session not created.");
            }
        }
		
		private QuickFix.FIX44.QuoteRequest QuoteRequest44(Quote o)
		{
			QuoteReqID id = new QuoteReqID(o.id);
			_rfq_id = new QuoteID(id.ToString());
			
			QuickFix.FIX44.QuoteRequest qr = new QuickFix.FIX44.QuoteRequest(id);
						
			qr.SetField(new QuickFix.Fields.Symbol(o.instrument));
			
			QuickFix.FIX44.QuoteRequest.NoRelatedSymGroup group = new QuickFix.FIX44.QuoteRequest.NoRelatedSymGroup();
			group.Set (new QuoteRequestType(102));
			group.Set (new OrderQty(o.size));
			group.Set (new SettlType("B"));	
			group.Set (new Account(o.account));
			group.Set (new Currency(o.instrument.Substring(0,3)));
			group.Set (new SettlDate(o.value_date));
			
        	// add group to request
        	qr.AddGroup(group);
			
			return qr;
		}
		
		private QuickFix.FIX44.QuoteCancel QuoteCancel44()
		{		
			QuoteCancelType qct = new QuoteCancelType(QuoteCancelType.CANCEL_FOR_ONE_OR_MORE_SECURITIES);
			QuickFix.FIX44.QuoteCancel qcl = new QuickFix.FIX44.QuoteCancel(_rfq_id, qct);
			
			// Eiger needs tag 131 included as well
			qcl.SetField(new QuickFix.Fields.QuoteReqID(_rfq_id.ToString()));
						
			return qcl;
		}
		
		private QuickFix.FIX44.OrderCancelRequest OrderCancelRequest44()
		{		
			QuickFix.FIX44.OrderCancelRequest ocl = new QuickFix.FIX44.OrderCancelRequest();
						
			return ocl;
		}
		
		private QuickFix.FIX44.MarketDataRequest MarketDataRequest44()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT);
            MarketDepth marketDepth = new MarketDepth(1);
			
            QuickFix.FIX44.MarketDataRequest message = new QuickFix.FIX44.MarketDataRequest(mdReqID, subType, marketDepth);
            
            QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

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
		
		private QuickFix.FIX44.MarketDataRequest MarketDataRefresh44()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES);
            MarketDepth marketDepth = new MarketDepth(1);
            
            QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

            QuickFix.FIX44.MarketDataRequest message = new QuickFix.FIX44.MarketDataRequest(mdReqID, subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);
 			
 			// GainGTX custom tag 7820 set to SPOT
 			message.SetField(new StringField(7820, ""));

            return message;
        }

		private QuickFix.FIX44.MarketDataRequest MarketDataUnsubscribe44()
        {
            // You can reuse the same market data request id reference
            // So it's recommeneded to use the symbol, i.e. EURUSD to avoid duplicate streams
			MDReqID mdReqID = new MDReqID("EURUSD");
            SubscriptionRequestType subType = new SubscriptionRequestType(SubscriptionRequestType.DISABLE_PREVIOUS);
            MarketDepth marketDepth = new MarketDepth(1);
            
            QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup marketDataEntryGroup = new QuickFix.FIX44.MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            // GainGTX requires the Symbol to be in format like: EUR/USD
            QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup symbolGroup = new QuickFix.FIX44.MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol("EUR/USD"));

            QuickFix.FIX44.MarketDataRequest message = new QuickFix.FIX44.MarketDataRequest(mdReqID, subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);
 			
 			// GainGTX custom tag 7820 set to SPOT
 			message.SetField(new StringField(7820, ""));

            return message;
        }		
			
        private QuickFix.FIX44.NewOrderSingle NewOrderSingle44(Order o)
        {          
            // TODO: Raise an exception if _rfq_id is not supplied or is null
            
            QuickFix.Fields.ClOrdID clOrdID = new ClOrdID(o.id);      	
            QuickFix.Fields.Symbol symbol = new Symbol(o.instrument);
            QuickFix.Fields.TransactTime time = new TransactTime(DateTime.Now);
            QuickFix.Fields.OrdType ordType = new OrdType(OrdType.PREVIOUSLY_QUOTED);
            // QuickFix.Fields.OrdType ordType = new OrdType(OrdType.MARKET);
            // QuickFix.Fields.OrdType ordType = new OrdType(OrdType.LIMIT);  

			// Establish whether buying or selling            
			char BuyOrSell;
            if (o.side == "BUY")
            	BuyOrSell = '1';
            else
            	BuyOrSell = '2';
            	
            QuickFix.Fields.Side side = new Side(BuyOrSell);

            QuickFix.FIX44.NewOrderSingle nos = new QuickFix.FIX44.NewOrderSingle(
            	clOrdID, symbol, side, time, ordType);

            nos.Set(new OrderQty(o.size));
            nos.Set(new Account(o.account));
            nos.Set(new Currency(o.instrument.Substring(0, 3)));
            
            // Setup a group for more stuff
            QuickFix.FIX44.NewOrderSingle.NoTradingSessionsGroup ts = new QuickFix.FIX44.NewOrderSingle.NoTradingSessionsGroup();
            ts.Set(new TradingSessionID("GSLDIRECT"));
			
        	// add group to request
        	nos.AddGroup(ts);            
            
 			// Eiger needs tag 117, 303, 336 for a NewOrderSingle
 			// Tag 117 is the quote id for the rfq. 		
 			nos.SetField(new StringField(117, o.quote_id));
 			nos.SetField(new StringField(303, "102"));

 			// nos.SetField(new StringField(303, QuoteRequestType.AUTOMATIC.ToString()));
 			// nos.SetField(new StringField(336, "GSLDIRECT"));

            return nos;
        }		
		
	}
}