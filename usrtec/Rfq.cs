/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/03/2015
 * Time: 12:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Usrtec
{
	/// <summary>
	/// Holds stuff used for RFQ processing, quote processing, order processing and execution reports
	/// It's a static class so it can be called from the 2 classes that use it: Demo and Engine
	/// </summary>

	public static class Rfq
	{
		// This one is the rfq:quote array. Used when an order is placed, to select the latest quote given that rfq
		// It's a struct not a class because it's a data structure only
		// Difference is a struct is a value type on the stack, a class would use stack pointers referencing the values on the heap
		// Therefore struct is a quicker data lookup... prove it !

		public struct rfq_quote
		{
			public string rfq_id, quote_id;
		}
		
		// Init a list of structs
		static List<rfq_quote> r = new List<rfq_quote>();
		
		public static void set_rfq_quote(string rfq, string quote)
		{
			// search the list of structs for the rfq_id
			rfq_quote new_rfq;
			new_rfq.rfq_id = rfq;
			new_rfq.quote_id = quote;
			
			bool found_rfq = false;
			
			for (int i=0; i < r.Count; i++)
			{
				if (r[i].rfq_id == rfq)
				{
					// replace the latest quote id
					r.Remove(r[i]);
					r.Add(new_rfq);
					found_rfq = true;
				}
			};
			
			if (found_rfq == false)
			{
				// it's a new rfq
					r.Add(new_rfq);
			}
		}
			
		// Get the latest quote id for the rfq
		public static string get_rfq_quote(string rfq)
		{
			// search the struct for the rfq_id
			for (int i=0; i < r.Count; i++)
			{
				if (r[i].rfq_id == rfq)
					return r[i].quote_id;
			};
			
			return "error";
		}
	}
}