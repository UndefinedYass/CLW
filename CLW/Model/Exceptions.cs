using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLW.Model
{
    public class ConnectivityException : Exception
    {
        private object error;

        public ConnectivityException(string p): base("Mi: " + p) {}



        public ConnectivityException(string p, Exception inner) : base("Mi: " + p, inner){}
    }




    /// <summary>
    /// exceptions that relates to the watcher: from loading to watching and checking excepcions
    ///speify inner exheption for better feedback
    ///  </summary>
    public class WatcherException : Exception
    {
        public WatcherException(string p,  bool shouldStopWatching, Exception inner) : base("Mi: " + p, inner)
        {

            ShouldStopWatching = shouldStopWatching;
        }
        public bool ShouldStopWatching { get; internal set; }
    }

    public class WatcherNotInitialzedException : Exception
    {
        public WatcherNotInitialzedException( ) : base("Mi: Watcher Not Initialized Cannot start watching " )
        {
        }
    }


    /// <summary>
    /// exceptions that may happen at the checking routin of a watcher, they may or may not implly that the watcher should stop watching
    ///speify inner exheption for better feedback
    ///  </summary>
    public class CheckingException : Exception
    {
        public CheckingException(string p,bool shouldStopWatching, Exception inner) : base("Mi: "+p, inner) {

            ShouldStopWatching = shouldStopWatching;
        }
        public bool ShouldStopWatching { get; internal set; }
    }

    public class ListParserException : Exception
    {
        public ListParserException(string p) : base("Mi: "+p) { }
        public ListParserException(string p, Exception inner) : base("Mi: " + p, inner) { }
    }


    public static class MiExceptions
    {

        /// <summary>
        /// returns true if the type falls under the CLW.Model namespace
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool IsMiException(this Exception err)
        {
            return err.GetType().FullName.StartsWith("CLW.Model.");
        }
    }

    public class Logger
    {
        public static void Log(Exception err)
        {
            string formatted = null;
            string at = DateTime.Now.ToString();
            string nl = Environment.NewLine;
            string sp = "#########";
            formatted = $"[ERROR] {at}{nl} {err.TargetSite.Name}";
            formatted += Environment.NewLine;
            File.AppendAllText(MI.ERRORS_LOG_FILE, formatted);
        }


        public static void Info(string str)
        {
            string formatted = null;
            string at = DateTime.Now.ToString();
            string nl = Environment.NewLine;
            string sp = "#########";
            formatted = $"[INFO] {at}{nl}{str}";
            formatted += Environment.NewLine;
            File.AppendAllText(MI.ERRORS_LOG_FILE, formatted);
        }


        /// <summary>
        /// returns a string that combines the nested exceptions messages, goes as deep as the first inner exception that is not IsMiException
        /// isMiException is true when the exception's type is under under the Model. namespace
        /// </summary>
        /// <param name="err"></param>
        /// <param name="separator">the separator between exceptions, default is newLine</param>
        /// <returns></returns>
        public static string BuildStackMessage(Exception err, string separator = null)
        {
            if (err == null) return "null";
            var outp = new StringBuilder();
            if (separator == null) separator = Environment.NewLine;
            bool FirstCycle = true; //used to prevent starting the streing with the separator
            Exception currExc =err ;
            outp.Append(currExc.Message==null?"null":currExc.Message);
            do
            {
                currExc = currExc.InnerException;
                string currentFragment = currExc==null?"null": currExc.Message;
                outp.Append((separator) + currentFragment);
                FirstCycle = false;
            } while (currExc!=null && currExc.IsMiException() );
            return outp.ToString();
        }
    }





}
