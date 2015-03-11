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
	/// Data type for order execution requests
	/// </summary>
	public class Order
	{
		public string id {get; set;}
		public string quote_id {get; set;}		
		public decimal size {get; set;}
		public string side {get; set;}
		public string account {get; set;}
		public string instrument {get; set;}
		public string time {get; set;}
		public string value_date {get; set;}
	}
}
