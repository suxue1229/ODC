using Newtonsoft.Json.Linq;
using PTR.Logging;
using PTR.Win32;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ODClisrv
{
    public partial class ODClisrv : ServiceBase
    {
        String host = String.Empty, uuid = String.Empty;
        String srvCfgFile = null;
        Timer manTimer = null;
        DateTime srvCfgTime = new DateTime(0);
        FileMainten appCliMan, appUpdMan, cfgCliMan;

        public ODClisrv()
        {
            InitializeComponent();
            try
            {
                appCliMan = new FileMainten(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ODClient.exe"), true);
                appUpdMan = new FileMainten(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update.exe"), true);
                cfgCliMan = new FileMainten(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ODClient.config"), false);
                srvCfgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ODClisrv.config");
                Logger.LogMessage(String.Format("Service start running.\r\nConfiguration file:{0}", srvCfgFile));
                if (File.Exists(srvCfgFile))
                {
                    String content = String.Empty;
                    using (FileStream stream = new FileStream(srvCfgFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            content = reader.ReadToEnd();
                            reader.Close();
                        }
                        stream.Close();
                    }
                    if (content.Length > 0)
                    {
                        var json = JObject.Parse(content);
                        host = (json["host"] != null) ? json["host"].Value<String>() : null;
                        uuid = (json["id"] != null) ? json["id"].Value<String>() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Initialization error.", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            manTimer = new Timer(10000);
            manTimer.Elapsed += manTimer_Elapsed;
            manTimer.Start();
        }

        String appid = null;
        void manTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ((Timer)sender).Stop();

            #region 检查配置文件
            if (!File.Exists(srvCfgFile))
            {
                try
                {
                    var json = new JObject(
                        new JProperty("host", "http://localhost/ODCenter/"),
                        new JProperty("id", Guid.Empty.ToString("N"))
                        );
                    using (FileStream stream = new FileStream(srvCfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            writer.Write(json.ToString());
                            writer.Close();
                        }
                        stream.Close();
                    }
                    srvCfgTime = File.GetLastWriteTime(srvCfgFile);
                    host = json["host"].Value<String>();
                    uuid = json["id"].Value<String>();
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when initialize service configuration file.", ex);
                }
            }
            #endregion

            #region 检测配置更新
            if (File.GetLastWriteTime(srvCfgFile) != srvCfgTime)
            {
                try
                {
                    String content = String.Empty;
                    using (FileStream stream = new FileStream(srvCfgFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            content = reader.ReadToEnd();
                            reader.Close();
                        }
                        stream.Close();
                    }
                    if (content.Length > 0)
                    {
                        var json = JObject.Parse(content);
                        host = (json["host"] != null) ? json["host"].Value<String>() : null;
                        uuid = (json["id"] != null) ? json["id"].Value<String>() : null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when loading service configuration file.", ex);
                }
            }
            #endregion

            #region 联网检查更新
            if (!String.IsNullOrWhiteSpace(host) && (uuid != Guid.Empty.ToString("N")))
            {
                try
                {
                    String addr = host.TrimEnd('/') + "/Update/Check/" + uuid;
                    String updstr = GetWebContent(addr, null);
                    if (!String.IsNullOrWhiteSpace(updstr))
                    {
                        var json = JObject.Parse(updstr);
                        Boolean bb = cfgCliMan.IsModified(json["config"].Value<String>());
                        if (json["config"] != null && cfgCliMan.IsModified(json["config"].Value<String>()))
                        {
                            addr = host.TrimEnd('/') + "/Update/Configs";
                            String cfgstr = GetWebContent(addr, null);
                            if (!String.IsNullOrWhiteSpace(cfgstr))
                            {
                                cfgCliMan.ReplaceContent(cfgstr);
                                appCliMan.RestartExec(WindowStyle.Minimized, "/silent");
                            }
                        }
                        Boolean test = appCliMan.IsModified(json["client"].Value<String>());
                        if (json["client"] != null && json["update"] != null && appCliMan.IsModified(json["client"].Value<String>()))
                        {
                            String updfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update.exe");
                            Boolean aa = appUpdMan.IsModified(json["update"].Value<String>());
                            if (appUpdMan.IsModified(json["update"].Value<String>()))
                            {
                                addr = host.TrimEnd('/') + "/Update/Client/" + uuid;
                                GetAppUpdate(addr, null, updfile);
                            }
                            FileInfo info = new FileInfo(updfile);
                            if (info.Exists && info.Length > 0)
                            {
                                appCliMan.ExceteUpdate(updfile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when retrieving new configuration.", ex);
                }
            }
            #endregion

            #region 监控ODClient运行
            if (File.Exists(appCliMan.FileName))
            {
                if (appid == null)
                {
                    try
                    {
                        appid = Assembly.LoadFile(appCliMan.FileName).ManifestModule.ModuleVersionId.ToString();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error occurred when loading client's assembly.", ex);
                    }
                }
                if (appid != null)
                {
                    try
                    {
                        Console.WriteLine("appid:"+ appid);
                        using (System.Threading.Mutex mutex = System.Threading.Mutex.OpenExisting("Global\\" + appid)) { }
                    }
                    catch (System.Threading.WaitHandleCannotBeOpenedException)
                    {
                        try
                        {
                            appCliMan.RestartExec(WindowStyle.Minimized, "/silent");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error occurred when starting client application.", ex);
                        }
                    }
                }
            }
            #endregion

            ((Timer)sender).Start();
        }

        DateTime lastFail = new DateTime(0);
        public String GetWebContent(String url, String data)
        {
            try
            {
                String content = null;
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                if (!String.IsNullOrWhiteSpace(data))
                {
                    request.Method = "POST";
                    Byte[] payload = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = payload.Length;
                    request.GetRequestStream().Write(payload, 0, payload.Length);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    content = reader.ReadToEnd();
                    response.Close();
                    reader.Close();
                }
                return content;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(WebException) && ((WebException)ex).Status == WebExceptionStatus.ConnectFailure)
                {
                    if ((DateTime.Now - lastFail).TotalSeconds >= 180)
                    {
                        lastFail = DateTime.Now;
                        Logger.LogError(String.Format("Error occurred when retrieving web content.\r\nUrl:{0}\r\nData:{1}", url, data), ex);
                    }
                }
                else
                {
                    Logger.LogError(String.Format("Error occurred when retrieving web content.\r\nUrl:{0}\r\nData:{1}", url, data), ex);
                }
                return null;
            }
        }

        public void GetAppUpdate(String url, String data, String file)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                if (!String.IsNullOrWhiteSpace(data))
                {
                    request.Method = "POST";
                    Byte[] payload = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = payload.Length;
                    request.GetRequestStream().Write(payload, 0, payload.Length);
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream webstream = response.GetResponseStream();
                FileStream filestream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                byte[] buff = new byte[10240];
                Int32 length = 0, total = 0;
                do
                {
                    length = webstream.Read(buff, 0, 10240);
                    if (length > 0)
                    {
                        total += length;
                        filestream.Write(buff, 0, length);
                    }
                } while (length > 0);
                filestream.Close();
                webstream.Close();
                Logger.LogMessage(String.Format("Download file.\r\nUrl:{0}\r\nFile:{1}\r\nSize:{2}", url, file, total));
            }
            catch (Exception ex)
            {
                Logger.LogError(String.Format("Error occurred when retrieving app update.\r\nUrl:{0}\r\nData:{1}", url, data), ex);
            }
        }

        protected override void OnStop()
        {
            Logger.LogMessage("Service stop running.");
        }
    }
}
