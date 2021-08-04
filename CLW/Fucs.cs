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


namespace CLW
{




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


        static MainWindow mw = (MainWindow)Application.Current.MainWindow;

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








        

        [Obsolete("use curl instead", true)]
        /// <summary>
        /// deprecated and ruined on 24-04-2021 due to changing the html temp to app data,
        /// curl WebClient should be used now
        /// 
        /// runs the python fetching script and gets back the raw html content from its
        /// output file 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> run_py_fetcher(Uri url)
        {

            StringBuilder html_content = new StringBuilder();
            string outputFile = url.AbsoluteUri.GetHashCode() + ".html";
            string python_args = ".\\scripts\\fetch-python.py " + url.AbsoluteUri + " " + $"{MI.TEMP_HTML_FILES}\\{outputFile}";
            Process process = constructProcess("python.exe", python_args);
            process.StartInfo.WorkingDirectory = "C:\\TOOLS\\fbhd-obsolete-gui";
            DataReceivedEventHandler py_hndl = ((sender, args) =>
            {
                html_content.AppendLine(args.Data);
            });
            process.OutputDataReceived += py_hndl;
            MI.Verbose("starting py_fetcher..");
            await Task.Run(new Action(() =>
            {
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }));
            MI.Verbose("py_fetcher is runing");
            await Task.Run(new Action(() =>
            {
                process.WaitForExit();
            }));
            MI.Verbose(null);
            string result = (html_content.ToString());
            if (result.Contains("success:"))
            {
                return File.ReadAllText(MI.TEMP_HTML_FILES + "\\" + outputFile);

            }

            return result;

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
                throw new Exception("mi: invalid number");
            }






        }
    }













    /// <summary>
    /// webClient using a custome underlying process e.g curl/powershell/python
    /// abstracts away the hustel of spawning the process, configuring stdout...
    /// saving temporary HTML's, and reding data from them, provides simple, yet powerfull
    /// interface 
    /// </summary>
    public abstract class WebClient
    {

        public abstract Headers DefaultHeaders { get; }
        public abstract string ProcessFileName { get; }

        /// <summary>
        /// used for better loggings , instead of printing the wholre processName that may include full paths
        /// </summary>
        public abstract string Name { get; }

        public abstract string makeArgs(string url, Headers headers, string outputFile, bool followRedirects);
        public abstract bool success(int exitCode);
        public abstract string AfterArgs { get; set; }



        /// <summary>
        /// get text using a temp html file
        /// returns null string on any failure, use the enhanced version GetTextAdvanced for more detailed failure reason
        /// </summary>
        /// <param name="saveAs">assigning this will cause the temp html to be saved under a custome name, and kept safe after the task completes</param>
        /// <returns>returns string on success, null on failure</returns>
        public async Task<string> GetText(string url, Headers headers = null, string saveAs = null)
        {
            bool shouldKeepFile = !(saveAs == null);
            string output = "";
            string tempHTMLFile = shouldKeepFile ? saveAs : "temp," + DateTime.Now.GetHashCode().ToString();
            string args = makeArgs(url, headers, tempHTMLFile, true);
            Process webClientProcess = Fucs.constructProcess(ProcessFileName, args);
            StringBuilder cachedStdout = new StringBuilder();
            webClientProcess.OutputDataReceived += (s, e) => { cachedStdout.Append(e.Data); };
            webClientProcess.StartInfo.WorkingDirectory = MI.TEMP_HTML_FILES;
            /// ## RUNNING ##
            MI.Verbose($"starting {Name}..");
            await Task.Run(new Action(() =>
            {
                webClientProcess.Start();
                webClientProcess.BeginErrorReadLine();
                webClientProcess.BeginOutputReadLine();
            }));
            MI.Verbose($"{Name} is runing");
            await Task.Run(new Action(() =>
            {
                webClientProcess.WaitForExit();
            }));
            MI.Verbose(null);


            /// ## READING FROM FILE ##

            if (success(webClientProcess.ExitCode))
            {
                output = File.ReadAllText(MI.TEMP_HTML_FILES + "\\" + tempHTMLFile);
                if (!shouldKeepFile) File.Delete(MI.TEMP_HTML_FILES + "\\" + tempHTMLFile);
                return output;
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// get text using a temp html file, workes somilar to GetText but returns detailed information including the process exit code, the time spent
        /// </summary>
        /// <param name="saveAs">assigning this will cause the temp html to be saved under a custome name, and kept safe after the task completes</param>
        public async Task<GetTextResult> GetTextAdvanced(string url, Headers headers = null, string saveAs = null, bool followRedireects = false)
        {
            bool shouldKeepFile = !(saveAs == null);
            string output = "";

            string tempHTMLFile = saveAs;
            if (!shouldKeepFile)
            {
                tempHTMLFile = MI.TEMP_HTML_FILES + "\\" + "temp," + DateTime.Now.GetHashCode().ToString();
            }
            string args = makeArgs(url, headers, tempHTMLFile, followRedireects);
            Process webClientProcess = Fucs.constructProcess(ProcessFileName, args);
            StringBuilder cachedStdout = new StringBuilder();
            webClientProcess.OutputDataReceived += (s, e) => { cachedStdout.Append(e.Data); };
            webClientProcess.StartInfo.WorkingDirectory = MI.TEMP_HTML_FILES;
            /// ## RUNNING ##
            MI.Verbose($"starting {Name}..");
            await Task.Run(new Action(() =>
            {
                webClientProcess.Start();
                webClientProcess.BeginErrorReadLine();
                webClientProcess.BeginOutputReadLine();
            }));
            MI.Verbose($"{Name} is runing");
            await Task.Run(new Action(() =>
            {
                webClientProcess.WaitForExit();
            }));
            MI.Verbose(null);


            /// ## READING FROM FILE ##

            if (success(webClientProcess.ExitCode))
            {
                output = File.ReadAllText(tempHTMLFile);
                if (!shouldKeepFile) File.Delete(tempHTMLFile);
                return new GetTextResult() { Text = output, Success = true, ClientExitCode = webClientProcess.ExitCode, RunningTime = webClientProcess.StartTime - webClientProcess.ExitTime };
            }
            else
            {
                return new GetTextResult() { Text = null, Success = false, ClientExitCode = webClientProcess.ExitCode, RunningTime = webClientProcess.StartTime - webClientProcess.ExitTime };
            }

        }



        internal event EventHandler<string> DateRecieved;
        internal event EventHandler<int> ProcessExited;


        public struct DownloadResult
        {
            public Exception Error;
            public bool Success;
            public int agentReturnCode;

            public override string ToString()
            {
                return $"success:{Success.ToString()} , error:{Error.ToString()} , returnCode:{this.agentReturnCode.ToString()} ";
            }
        }





        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns>returns string on success, null on failure</returns>
        public async Task<DownloadResult> DownloadBinary(string url, string saveAs, Headers headers = null)
        {
            string args = makeArgsBinary(url, saveAs, headers);
            //MI.ConsoleLog(args);
            Process webClientProcess = Fucs.constructProcess(ProcessFileName, args);
            StringBuilder cachedStdout = new StringBuilder();


            webClientProcess.OutputDataReceived += (s, e) => {
                cachedStdout.Append(e.Data);
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    if (DateRecieved != null)
                    {
                        DateRecieved(this, e.Data);
                    }

                }

            };
            webClientProcess.ErrorDataReceived += (s, e) => {
                cachedStdout.Append(e.Data);
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    if (DateRecieved != null)
                    {
                        DateRecieved(this, e.Data);
                    }

                }

            };
            webClientProcess.StartInfo.WorkingDirectory = MI.MAIN_PATH;
            /// ## RUNNING ##
            MI.Verbose($"starting {Name}..");
            await Task.Run(new Action(() =>
            {
                webClientProcess.Start();
                webClientProcess.BeginErrorReadLine();
                webClientProcess.BeginOutputReadLine();
            }));
            MI.Verbose($"{Name} is runing");
            await Task.Run(new Action(() =>
            {
                webClientProcess.WaitForExit();
            }));
            MI.Verbose(null);
            webClientProcess.Exited += (s, e) => {

                ProcessExited.Invoke(s, webClientProcess.ExitCode);
            };
            /// ## ##


            return new DownloadResult()
            {
                Error = success(webClientProcess.ExitCode) ? null : new Exception("agent failed with code " + webClientProcess.ExitCode.ToString()),
                Success = success(webClientProcess.ExitCode),
                agentReturnCode = webClientProcess.ExitCode

            };


        }

        internal abstract string makeArgsBinary(string url, string saveAs, Headers headers);


        public class cURL : WebClient
        {

            public event EventHandler<curlProgress> onProgress;

            public struct curlProgress
            {
                public double Percent; // DL ratio [0,1]
                public int Dled, Speed; // in  bytes
                public TimeSpan? Total, Left;
                public TimeSpan Current;

                public override string ToString()
                {
                    return $"percent:{Math.Round(Percent * 100, 1).ToString()}, left:{Left.ToString()}, elapsed:{Current.ToString()}, Total:{Total.ToString()}, Dled:{Dled.ToString()} ";
                }

            }


            public bool TryParseProgressInfo(string rawLine, out curlProgress progInfo)
            {
                curlProgress outp = new curlProgress();
                //DL% UL%  Dled  Uled  Xfers  Live   Qd Total     Current  Left    Speed
                //100 --  3906k     0     4     0     0  0:00:17  0:00:23 --:--:--  223k
                try
                {
                    Regex re = new Regex("(\\d{1,3}) (\\S{1,3}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8}) +(\\S{1,8})");
                    Match m = re.Match(rawLine);
                    if (!m.Success)
                    {
                        progInfo = outp;
                        return false;
                    }

                    string rawPercent = m.Groups[1].Value;
                    string rawDled = m.Groups[3].Value;
                    string rawTotal = m.Groups[8].Value;
                    string rawCurrent = m.Groups[9].Value;
                    string rawLeft = m.Groups[10].Value;
                    string rawSpeed = m.Groups[11].Value;




                    outp.Percent = double.Parse(rawPercent) / 100;
                    outp.Dled = new Size(rawDled + "b").bytes;
                    outp.Total = Fucs.TimeSpanFromString(rawTotal);
                    outp.Current = Fucs.TimeSpanFromString(rawCurrent).Value;
                    outp.Left = Fucs.TimeSpanFromString(rawLeft);
                    outp.Speed = new Size(rawSpeed + "b").bytes;
                    //MessageBox.Show("787");
                    progInfo = outp;
                    return true;
                }
                catch (Exception)
                {
                    progInfo = new curlProgress();
                    return false;
                }

            }


            public cURL()
            {
                base.DateRecieved += (s, e) =>
                {
                    if (onProgress != null)
                    {
                        curlProgress cp;
                        if (TryParseProgressInfo(e, out cp))
                        {
                            onProgress(this, cp);
                        }

                    }


                };
            }
            public override Headers DefaultHeaders
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string ProcessFileName { get { return MI.CURL_PATH; } }

            public override string Name { get { return "cURL"; } }

            private string afterArgs = "";
            public override string AfterArgs
            {
                set
                {
                    afterArgs = value;
                }
                get
                {
                    return afterArgs;
                }
            }


            /// <summary>
            /// instantites a new cURL object and uses its GetTextAdvanced method, 
            /// </summary>
            public static async Task<cURL.GetTextResult> GetTextStatic(string url, Headers headers, bool folowRedirects)
            {
                return await new cURL().GetTextAdvanced(url, headers, null, folowRedirects);

            }



            public override string makeArgs(string url, Headers headers, string outputFile, bool folowRedirects = false)
            {
                string args = "";
                args += (folowRedirects ? " -L " : "");
                args += $" --url {Fucs.qoute(url)}";
                args += $" -o {Fucs.qoute(outputFile)}";

                //
                foreach (var h in headers)
                {
                    args += $" -H \"{h.Key}:{h.Value}\" ";
                }
                args += $" {AfterArgs}";
                return args;

            }

            public override bool success(int exitCode) { return exitCode == 0; }

            internal override string makeArgsBinary(string url, string saveAs, Headers headers)
            {
                string args = "";

                args += $" --url {Fucs.qoute(url)}";
                args += $" -o {Fucs.qoute(saveAs)}";

                //
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        args += $" -H \"{h.Key}:{h.Value}\" ";
                    }

                }
                args += $" {AfterArgs}";


                // Clipboard.SetText(args);
                return args;
            }
        }

        public class PyFetcher
        {

        }
        public class Powershell
        {

        }

        public struct GetTextResult
        {
            public string Text { get; set; }
            public int ClientExitCode { get; set; }
            public bool Success { get; set; }
            public TimeSpan RunningTime { get; set; }
        }
    }

    public class Headers : Dictionary<string, string>
    {
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.11 Safari/537.36";
        public static Headers DefaultFbWatchHeaders
        {
            get
            {
                var v = new Headers();
                v.Add("accept-language", "en-US;");
                return v;
            }
        }

        public static Headers FakeUserAgentHeaders
        {
            get
            {
                var v = new Headers();
                v.Add("user-agent", Headers.USER_AGENT);
                return v;
            }
        }

        /// <summary>
        /// these cookies are critical for fbhd post parsing to work, not using them will result in fb 
        /// sending a minimal response that suits lite browsers and doesn't include all the information were looking for e.g quality labels, audio stream..   
        /// </summary>
        public static Headers FB_Fake_Browser_For_HD_Videos
        {
            get
            {
                var v = new Headers();
                v.Add("user-agent", Headers.USER_AGENT);
                //  v.Add("sec-ch-ua", "\"Google Chrome\";v=\"87\", \"\\\"Not;A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\"");
                // commented out because there seem to be some quote escaping issues, fortunatly this one
                //is not critical 
                v.Add("accept", "text/html");
                v.Add("sec-ch-ua-mobile", "?0");
                v.Add("accept-encoding", "");
                v.Add("sec-fetch-mode", "navigate");
                v.Add("accept-language", "en-US,en;q=0.9");
                v.Add("sec-fetch-dest", "document");

                return v;
                /* some cokies as taken from chrome dev tools
                "sec-ch-ua": "\"Google Chrome\";v=\"87\", \"\\\"Not;A\\\\Brand\";v=\"99\", \"Chromium\";v=\"87\"",
                "accept":"text/html",
                "sec-ch-ua-mobile": "?0",
                "accept-encoding":"",
                "sec-fetch-mode": "navigate" ,
                "accept-language": "en-US,en;q=0.9" ,
                "sec-fetch-dest": "document"
                */
            }
        }

        public void evz()
        {

        }
    }





    public enum OverrideBehaviour
    {
        Override, Enumerate, Prompt, Skip, CheckSizeAndSkip
    }



}
