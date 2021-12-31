using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;

using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml;

using System.Security.Policy;
using System.Dynamic;
using System.Xml.Serialization;
using System.Windows.Documents;
using System.Net;
using CLW.Model;
using CLW.Model.Enums;

namespace CLW
{



    public class NoNewsCardVisibilityCalculator : IValueConverter
    {
         
        //currente equation: unread count is zero and is failing is false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value == null || value.GetType() != typeof(CustomLW)) return Visibility.Visible;

            //if (value.GetType() != typeof(CustomLW)) return Visibility.Visible;//in design time the data is not of tipe CustomLW and that causes designer failur
            CustomLW clw = (CustomLW)value;
            return ((clw != null) && (!clw.HasUnreadNews )&& clw.IsRunningSuccessfully)?Visibility.Visible:Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NewsErrorCardVisibilityCalculator : IValueConverter
    {
        //currente equation: unread count is zero and is failing is false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value==null || value.GetType()!= typeof(CustomLW)) return Visibility.Visible;
            //if (value.GetType() != typeof(CustomLW)) return Visibility.Visible;//in design time the data is not of tipe CustomLW and that causes designer failur
            CustomLW clw = (CustomLW)value;
            return ( (clw!=null) && clw.IsFailing ) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class visibleIfNonEmptyText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return string.IsNullOrWhiteSpace(value.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class badgeZeroToNullReplacer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int asInt=0;
            int.TryParse(value.ToString(), out asInt);
            return asInt == 0 ? null : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class boolToVisibilityInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class visibleWhenZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return (int)value==0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class visibleWhenGreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int valInt = 0, paramInt = 0;
            int.TryParse(parameter.ToString(), out paramInt);
            return (int)value > paramInt ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// returns the first non null or empty string withing an array, 
    /// returns null if non was found
    /// </summary>
    public class Fucs
    {


        internal const int CTRL_C_EVENT = 0;

        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandelerRoutine, bool add);
        internal delegate Boolean ConsoleCtrlDelegate(uint CtrlType);









        public static Type MapCPETypeEnumToType(CPEElements type_name)
        {
            Trace.WriteLineIf(false,type_name, "MapCPETypeEnumToType :");
            Type outp = Type.GetType("CLW.Model."+type_name.ToString() + "CPE");
            Trace.WriteLineIf(false, outp?.FullName, "res type");
            return outp;


        }


       static Random rnd = new Random();

        public static Brush RandomAvatarBackgroundBrush()
        {
            string raw_codes= "#FFB71C1C,#FF782249,#FF264D6E,#FF006064,#FF004D40,#FF1B5E20,#FF1A237E,#FF0D47A1,#FF01579B,#FF33691E,#FF827717,#FFF57F17,#FFFF6F00";
            var raw_codes_arr = raw_codes.Split(',');
           
            int picked_ix = rnd.Next(0, raw_codes_arr.Length);
            string picked_code = raw_codes_arr[picked_ix];
            Brush outp = new SolidColorBrush((Color)( ColorConverter.ConvertFromString(picked_code)));
            Trace.WriteLineIf(false, $"picked random color was {(outp as SolidColorBrush).Color.ToString()}");
            return outp;
        }


        /// <summary>
        /// iterates over all directories listed in the PATH env var and their direct sub files searching for a file that matches the specified predicate
        /// NOTE: the predicate takes the whole filepath and not just the file name, use Path.getFileName to exctract the filname
        ///  </summary>
        public static bool ExecutableExistsInThePath(Predicate<string> match)
        {
            string path = Environment.GetEnvironmentVariable("path");
            // MessageBox.Show(path);
            foreach (var dir in path.Split(new char[] { ';' }))
            {
                if (!Directory.Exists(dir)) continue;
                string exists = Directory.GetFiles(dir).ToList().Find(match);
                if (exists != null) return true;
            }
            return false;
        }


        /// <summary>
        /// mi string to time span converter that uses a DateTime object and some regex adjustement under the hood
        /// NOTE: on missMatch, null value is returned,
        /// NOTE: only inputes like these examples are suported: "3:23" "1:45:12" "1:45:12.568" "20"
        /// avoide these input formats: "120" , "1:24.456" 
        /// NOTE: the input "3:32" is interpreted as "0:3:32" unlike how .Net does it (3:32:0)
        /// </summary>
        public static TimeSpan? TimeSpanFromString(string str)
        {
            if ((Regex.IsMatch(str, "^\\d*$")))
            { // one number will be interpreted as minutes eg: "12" gives "0:12:0"
                str = "0:" + str + ":0";
            }
            else if (Regex.IsMatch(str, "^\\d*:\\d*$")) //if the suer typed something like 3:24 it gets automatically convetred to 0:3:24, so that the .Net TimeSpan parser recognize it
            {
                str = "0:" + str;
            }
            DateTime asDate;
            bool isValid = DateTime.TryParse(str, out asDate);

            return (isValid == false) ? (TimeSpan?)null : new TimeSpan(asDate.Hour, asDate.Minute, asDate.Second);

        }



        public static string decodeUtf(string raw)
        {
            string output = "";

            //raw = "u\\627u\\627&#x647;&#x643;&#x630;&#x627; &#x62a;&#x628;&#x62f;&#x648; &#x627;&#x644;&#x623;&#x631;&#x636; &#x645;&#x646; &#x645;&#x62d;&#x637;&#x629; &#x627;&#x644;&#x641;&#x636;&#x627;&#x621; &#x627;&#x644;&#x62f;&#x648;&#x644;&#x64a;&#x629;";
            output = Regex.Replace(raw, "&#x([\\dabcdef]{3,5});", (Match m) =>
            {
                string decoded_char = "";
                try
                {
                    decoded_char = char.ConvertFromUtf32(int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier));

                }
                catch (Exception)
                {

                }

                return decoded_char;
            });

            output = Regex.Replace(output, "\\\\u([\\dabcdef]{3,5})", (Match m) =>
            {

                string decoded_char = "";

                try
                {
                    decoded_char = char.ConvertFromUtf32(int.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier));

                }
                catch (Exception)
                {

                }

                return decoded_char;
            });








            return output;
        }





        /// <summary>
        /// searchs whithin the input string for url-like substrings and retuns them as a List<string>
        /// </summary>
        /// <param name="rawUserInput">can be a simple url  or a bench of urls  </param>
        /// <returns></returns>
        public static List<string> extractUrls(string Inputstr)
        {
            List<string> outp = new List<string>();

            MatchCollection mc = Regex.Matches(Inputstr,
                "(https?://.*?)(?=(?:http|;|[ \\t\\n\\r\\f\\v]|\\Z))");

            foreach (Match m in mc)
            {
                outp.Add(m.Value);
                // MessageBox.Show(m.Value);
            }

            return outp;
        }

        public static string getFirstNonNull(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] != null && arr[i] != "")
                    return arr[i];
            return null;
        }





        /// <summary>
        /// double quotes a string: "yass" => ""yass""
        /// </summary>
        public static string qoute(string input)
        {
            return "\"" + input + "\"";
        }



        /// <summary>
        /// removes special characters from a string: 
        /// < > : / \ ? * \n
        /// </summary>
        public static string filenamify(string name)
        {
            return Regex.Replace(name, "[/<>:\\*\\?\"\\|\\\\\\n]", "");
            //what fuck am I doing with my life
        }



        public static string bytesToString(byte[] b)
        {
            string res = "";
            foreach (byte item in b)
            {
                char c;
                c = (char)item;

                res = res + c.ToString();
            }
            return res;
        }


        public static Process constructProcess(string filename, string args)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = filename;
            startInfo.Arguments = args;
            startInfo.WorkingDirectory = MI.MAIN_PATH;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            return process;
        }








        

      









        static DateTime d70 = new DateTime(1970, 1, 1, 0, 0, 0);



        /// <summary>
        /// timstamp should be in seconds, counting from 1/1/1970 00:00:00
        /// </summary>
        internal static DateTime dateFromTimestamp(int timestamp)
        {
            return (d70.AddSeconds(timestamp));
        }



        /// <summary>
        /// decodes stuff like "Mi &amp; You " to "Mi & You"
        /// it uses the xml parser, so it should handle all the escape notations
        /// </summary>
        internal static string decodeXml(string input)
        {
            XElement ElemWithTextNode = XElement.Parse($"<elem>{input}</elem>");

            return (((XText)ElemWithTextNode.FirstNode).Value);

        }


        /// <summary>
        /// mutiplies a timspan 
        /// Note: the value may not be 100% acurate as the ticks property of TimeSpan only accepts integeres
        /// </summary>
        internal static TimeSpan TimeSpanMultipler(TimeSpan input, double multiplier)
        {
            return new TimeSpan((long)(Math.Truncate(input.Ticks * multiplier)));
        }

        internal static string TimeSpanToString(TimeSpan input)
        {
            return $"{(input.Hours > 0 ? input.Hours.ToString() + ":" : "")}{input.Minutes}:{input.Seconds}.{input.Milliseconds}";

        }

        /// <summary>
        /// parses fb duration format into a timespan instance, 
        /// returns zero timespan on errors
        /// supported formats:  //# known formats: //PT690.04S //PT0H3M31.441S    /// </summary>
        /// Note: the "PT" part is not important
        /// <param name="durationStr"></param>
        /// <returns></returns>
        internal static TimeSpan FBDurationToTimeSpan(string durationStr)
        {


            Match H = Regex.Match(durationStr, "(\\d*?)H");
            Match M = Regex.Match(durationStr, "(\\d*?)M");
            Match S = Regex.Match(durationStr, "(\\d*\\.\\d*)S");
            double hours = H.Success ? double.Parse(H.Groups[1].Value) : 0;
            double minutes = M.Success ? double.Parse(M.Groups[1].Value) : 0;
            double seconds = double.Parse(S.Groups[1].Value);
            //  fixed fractional digits problem using rounding
            int millis = (int)(Math.Round(seconds - Math.Truncate(seconds), 3) * 1000.0);
            var outp = new TimeSpan(0, (int)hours, (int)minutes, (int)Math.Truncate(seconds), millis);
            return outp;
        }

      
    }


















    public struct Size
    {
        public Size(int bytes_)
        {
            bytes = bytes_;
        }

        /// <summary>
        /// uses raw string, e.g:  2056kB | 3.62GB | 500Mb
        /// </summary>
        /// <param name="str"></param>
        public Size(string str)
        {

            int sizeInBytes = 0;
            Match match = Regex.Match(str, "(b|kb|mb|gb)", RegexOptions.IgnoreCase);
            if (match.Value == null) throw new Exception("mi: Size parsing failed; unsuported format");
            string asNumber = str.Replace(match.Value, "").Trim();

            switch (match.Value.ToLower())
            {
                case "b":
                    sizeInBytes = int.Parse(asNumber);
                    break;
                case "kb":
                    sizeInBytes = (int)(Double.Parse(asNumber) * 1024);
                    break;
                case "mb":
                    sizeInBytes = (int)(Double.Parse(asNumber) * 1048576);
                    break;
                case "gb":
                    sizeInBytes = (int)(Double.Parse(asNumber) * 1073741824);
                    break;
                default:

                    break;
            }
            // MessageBox.Show(sizeInBytes.ToString());
            bytes = sizeInBytes;
        }
        public int bytes;
        public override string ToString()
        {

            if (bytes >= 1073741824)
            {
                return $"{ Math.Round((double)(bytes / 1073741824), 2).ToString()}GB";
            }
            else if (bytes >= 1048576)
            {
                return $"{ Math.Round((double)(bytes / 1048576), 2).ToString()}MB";
            }

            else if (bytes >= 1024)
            {
                return $"{ Math.Round((double)(bytes / 1024)).ToString()}KB";
            }

            else if (bytes < 1024)
            {
                return $"{bytes.ToString()}B";
            }
            else
            {
                return("mi: invalid number");
            }






        }
    }















    public enum OverrideBehaviour
    {
        Override, Enumerate, Prompt, Skip, CheckSizeAndSkip
    }



}
