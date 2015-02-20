/*
 * Created by SharpDevelop.
 * User: Webster Systems
 * Date: 11/12/2014
 * Time: 16:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace Usrtec
{
    public class NmsTracer : Apache.NMS.ITrace
    {
        #region ITrace Members
        public void Debug(string message)
        {
            Console.WriteLine("DEBUG: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("ERROR: " + message);
        }

        public void Fatal(string message)
        {
            Console.WriteLine("FATAL: " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine("INFO:  " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("WARN:  " + message);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        #endregion
    }
}
