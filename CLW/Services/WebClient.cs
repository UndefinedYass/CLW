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
using System.Threading;

namespace CLW.Services
{



    public class WebClientMi
    {


        public struct GetTextResult
        {
            public string Text { get; set; }
            public int ClientExitCode { get; set; }
            public bool Success { get; set; }
            public TimeSpan RunningTime { get; set; }
            public Exception Error { get; internal set; }
        }

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
        /// webClient using a custome underlying process e.g curl/powershell/python
        /// abstracts away the hustel of spawning the process, configuring stdout...
        /// saving temporary HTML's, and reding data from them, provides simple, yet powerfull
        /// interface 
        /// NOTE: this was named WebClientn, this was alteresd partially deprecated to fuuly go MVVM, 
        /// </summary>
        public abstract class ProcessWebClientBase: IWebClient
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
                await Task.Run(new Action(() =>
                {
                    webClientProcess.Start();
                    webClientProcess.BeginErrorReadLine();
                    webClientProcess.BeginOutputReadLine();
                }));
                await Task.Run(new Action(() =>
                {
                    webClientProcess.WaitForExit();
                }));


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
                await Task.Run(new Action(() =>
                {
                    webClientProcess.Start();
                    webClientProcess.BeginErrorReadLine();
                    webClientProcess.BeginOutputReadLine();
                }));
                await Task.Run(new Action(() =>
                {
                    webClientProcess.WaitForExit();
                }));


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
                await Task.Run(new Action(() =>
                {
                    webClientProcess.Start();
                    webClientProcess.BeginErrorReadLine();
                    webClientProcess.BeginOutputReadLine();
                }));
                await Task.Run(new Action(() =>
                {
                    webClientProcess.WaitForExit();
                }));
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

           

            public class cURL : ProcessWebClientBase
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


          

         
        }

        public interface IWebClient
        {
            Task<string> GetText(string url, Headers headers, string saveAs);

            Task<GetTextResult> GetTextAdvanced(string url, Headers headers, string saveAs, bool followRedireects);

            Task<DownloadResult> DownloadBinary(string url, string saveAs, Headers headers);

            /// <summary>
            /// instantites a new cURL object and uses its GetTextAdvanced method, 
            /// </summary>
           

        }



        /// <summary>
        /// wraps around dot nt webclent, and ovverides UserAgent to chome value so no need to do that manuallt
        /// use Instance
        /// </summary>
        public class Native : System.Net.WebClient, IWebClient
        {
            public Native() {
                //Proxy = null;
                
            }

           static Native _istance;


            [Obsolete("need to fix concurent operations problem",true)]
            /// <summary>
            /// if the internal _nstance IsBusy, this creates and returns a new anonymous instance, 
            /// </summary>
            public static Native Instance { get { if (_istance == null){ _istance = new Native(); return _istance; }else{ return _istance.IsBusy ? new Native(): _istance ;}} }

            public static Native getNewInstance()
            {
                var newn = new Native();
                return newn;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                //Trace.WriteLine("addres: " + address.ToString());
                if (ConfigService.Instance.Dev_IsMockedWebClient)
                {
                    string fakeUrl = null;
                    address = Model.Enums.MockWebClient.TransformUrlToMockedVersion.TryGetValue(address.ToString(), out fakeUrl) ? new Uri(fakeUrl) : address;
                    //Trace.WriteLine("mocked: " + address.ToString());
                    
                    //connects to a localhost server running on port 5001 (tools/simulating-fb)
                    //make sure to transform the url as needed,
                }
                if (ConfigService.Instance.Dev_IsCrapifyProxyEnabled)
                {

                    
                    //use the crapyfy proxy, running on port 5000, (start the proxy with crapify start ..args)
                    var myproxy=  new WebProxy(new Uri("http://127.0.0.1:5000"));
                    myproxy.BypassProxyOnLocal = true;
                    myproxy.UseDefaultCredentials = true;
                    Proxy = myproxy;

                    string prxaddr = ((WebProxy)Proxy).Address.ToString();
                    //Trace.WriteLine("proxy: " + prxaddr);
                }
                else
                {
                    Proxy = null;
                    //Trace.WriteLine("proxy null");
                }
                
                var request= base.GetWebRequest(address) as HttpWebRequest;
                
                request.UserAgent = WebClientMi.Headers.USER_AGENT;
                request.Timeout = 10000;
                request.ServicePoint.ConnectionLeaseTimeout = 10000;

                request.ServicePoint.MaxIdleTime = 10000;
                request.Proxy = Proxy;
                
                return request;
            }
            public async Task<DownloadResult> DownloadBinary(string url, string saveAs, Headers headers)
            {
                DownloadResult res = new DownloadResult();
                try
                {

                    await DownloadFileTaskAsync(url, saveAs);
                    res.Success = true;
                }
                catch (Exception err)
                {
                    res.Success = false;
                    res.Error = err;
                }
                return res;

            }

            /// <summary>
            /// based off native .net HttpWebRequest class
            /// </summary>
            /// <param name="url">url to fetch</param>
            /// <param name="headers">not supported</param>
            /// <param name="saveAs">not supoorted</param>
            /// <returns></returns>
            public async Task<string> GetText(string url, Headers headers = null, string saveAs = null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    
                   return DownloadString(url);
                    
                }
                catch (Exception err)
                {
                    Model.Logger.Log(err);
                    return null;
                }
                finally
                {
                    Model.Logger.Info($"DownloadString time: {sw.Elapsed.ToString()} {Environment.NewLine}url: {url} ");
                }

            }

            /// <summary>
            /// doesnt create thread, runns synchronously on the caller threa
            /// advanced version of GetText, only usefull for RunningTime property, 
            /// </summary>
            /// <returns></returns>
            public GetTextResult GetTextAdvancedSync(string url, Headers headers = null, string saveAs = null, bool followRedireects = false)
            {
                GetTextResult res = new GetTextResult();
                    try
                    {
                       // res.Text = DownloadString(url);
                   // var bytes = this.DownloadData(url);
                   // string str =bytes.ToString();
                    string str2 = UTF8Encoding.UTF8.GetString(this.DownloadData(url));
                    res.Text = str2;
                        res.Success = true;
                    }
                    catch (Exception err)
                    {
                        res.Error = err;
                        res.Success = false;
                    }
                return res;
            }



            /// <summary>
            /// advanced version of GetText, only usefull for RunningTime property, 
            /// </summary>
            /// <param name="url">url too fetch</param>
            /// <param name="headers">not used</param>
            /// <param name="saveAs">not supported</param>
            /// <param name="followRedireects"></param>
            /// <returns></returns>
            public async Task<GetTextResult> GetTextAdvanced(string url, Headers headers = null, string saveAs = null, bool followRedireects = false)
            {
                GetTextResult res = new GetTextResult();
                Thread t = new Thread((_url) =>
                {
                    //Trace.WriteLine("GetTextAdvanced");
                    try
                    {
                        // res.Text = DownloadString(url);
                        string str2 = UTF8Encoding.UTF8.GetString(this.DownloadData(url));
                        res.Text = str2;
                        res.Success = true;
                    }
                    catch (Exception err)
                    {

                        res.Error = err;
                            res.Success = false;
                    }
                    
                    //Trace.WriteLine(res.Text.Substring(0, 10));

                });

                t.Start(url);
                t.Join();
                
                /*string resultHtml = await GetText(url, headers, saveAs);*/
                return res;

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

        
         
        }


    }



}
