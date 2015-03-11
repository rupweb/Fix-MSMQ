/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/03/2015
 * Time: 09:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Usrtec
{
	/// <summary>
	/// Holds stuff used for RFQ processing, quote processing, order processing and execution reports
	/// It's a static class so it can be called from the 2 classes that use it: Demo and Engine
	/// </summary>

	public class Rfq
	{
		// This one is the rfq:quote array. Used when an order is placed, to select the latest quote given that rfq
		// It's a struct not a class because it's a data structure only
		// Difference is a struct is a value type on the stack, a class would have stack pointers addressing the heap
		// Therefore struct is a much quicker data lookup
		
		/* struct rfq_quote_array
		{
			string rfq_id;
			string quote_id;

			public void set_rfq_quote(string rfq, string quote)
			{
				this.rfq_id = rfq;
				this.quote_id = quote;
			}
			
			public rfq_quote_array get_rfq_quote()
			{
				return rfq_quote_array;
			}
		} */
		
		struct rfq_quote_struct
		{
			string rfq_id, quote_id;
		}
		
		protected static string[,] rfq_quote_array;
		
		protected static Dictionary<string,string> rfq_quote_dictionary {get; set;}
		
		protected static ConcurrentDictionary<string, string> rfq_quote_concurrent {get; set;} 
		
		static Rfq()
		{
 			rfq_quote_array = new string[,] {};
			rfq_quote_dictionary = new Dictionary<string, string>();
			rfq_quote_concurrent = new ConcurrentDictionary<string, string>();
			List<rfq_quote_struct>rfq_quote_list = new List<rfq_quote_struct>();
		}

	}
}
