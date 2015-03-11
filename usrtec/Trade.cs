/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/03/2015
 * Time: 17:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Usrtec
{
	/// <summary>
	/// Data type for trade execution reports
	/// </summary>
	public class Trade
	{
		public double price {get; set;}
		public string id {get; set;}
		public decimal size {get; set;}
		public string counterparty {get; set;}
		public string quote_id {get; set;}
		public string instrument {get; set;}
		public string time {get; set;}
		public string value_date {get; set;}		
	}
}
