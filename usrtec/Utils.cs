﻿/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 19/02/2015
 * Time: 19:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Usrtec
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		public static DateTime AddBusinessDays(DateTime date, int days)
		{
    		if (days < 0)
    		{
        		throw new ArgumentException("days cannot be negative", "days");
    		}

    		if (days == 0) return date;

    		if (date.DayOfWeek == DayOfWeek.Saturday)
    		{
        		date = date.AddDays(2);
        		days -= 1;
    		}
    		else if (date.DayOfWeek == DayOfWeek.Sunday)
    		{
        		date = date.AddDays(1);
        		days -= 1;
    		}

    		date = date.AddDays(days / 5 * 7);
			int extraDays = days % 5;

			if ((int)date.DayOfWeek + extraDays > 5)
			{
 				extraDays += 2;
			}

    		return date.AddDays(extraDays);
		}
	}
}
