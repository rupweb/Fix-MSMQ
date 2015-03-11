/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/03/2015
 * Time: 12:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Usrtec
{
	/// <summary>
	/// Description of Rfq2.
	/// </summary>
	public class Rfq2
	{
		public struct rfq_quote
		{
			public string rfq_id, quote_id;
		}
		
		// Init the array of structs
		rfq_quote[] r = new rfq_quote[]{};
		
		public void set_rfq_quote(string rfq, string quote)
		{
			// search the struct for the rfq_id
			bool found_rfq = false;
			
			for (int i=0; i < r.Length; i++)
			{
				if (r[i].rfq_id == rfq)
				{
					// replace the latest quote id
					r[i].quote_id = quote;
					found_rfq = true;
				}
			};
			
			if (found_rfq == false)
			{
				r[r.Length].rfq_id = rfq;
				r[r.Length].quote_id = quote;
			}
		}
			
		// Get the latest quote id for the rfq
		public string get_rfq_quote(string rfq)
		{
			// search the struct for the rfq_id
			for (int i=0; i < r.Length; i++)
			{
				if (r[i].rfq_id == rfq)
					return r[i].quote_id;
			};
			
			return "error";
		}
	}
}
