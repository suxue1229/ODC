using Newtonsoft.Json.Linq;
using OPCAutomation;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ODClient
{
    public partial class ODClient : Form
    {
        private List<String> host = new List<String>();
        private List<String> uuid = new List<String>();
        private static String defnetmsg = "Network Waiting";
        private static Dictionary<String, OPCServ> servers = new Dictionary<String, OPCServ>();
        private static Dictionary<Int32, OPCData> values = new Dictionary<Int32, OPCData>();
        private static Dictionary<String, DispFrame> frames = new Dictionary<String, DispFrame>();

        public ODClient()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            //加载配置文件
            try
            {
                StatOPC.Text = "OPC Connecting";
                StatNET.Text = defnetmsg;
                MainLayout.Controls.Clear();
                MainLayout.RowCount = 0;
                MainLayout.RowStyles.Clear();

                String cfgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ODClient.config");
                String cfgString = String.Empty;
                using (FileStream stream = new FileStream(cfgFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        cfgString = reader.ReadToEnd();
                        reader.Close();
                    }
                    stream.Close();
                }
                if (!String.IsNullOrWhiteSpace(cfgString))
                {
                    var json = JObject.Parse(cfgString);
                    foreach (JObject val in json["config"])
                    {
                        
                        host.Add((val["host"] != null) ? val["host"].Value<String>() : null);
                        uuid.Add((val["id"] != null) ? val["id"].Value<String>() : null);
                        if (val["name"] != null)
                        {
                            this.Text += String.Format("({0})", val["name"].Value<String>());
                        }
                        var sensors = (val["sensors"] != null) ? val["sensors"].Value<JArray>() : null;
                        if (sensors != null)
                        {
                            for (int i = 0; i < sensors.Count; i++)
                            {
                                OPCData dat = OPCData.Parse(sensors[i]);
                                if (dat != null)
                                {
                                    dat.Handle = values.Count + 1;
                                    values.Add(dat.Handle, dat);
                                }
                            }
                        }
                    }

                }
                foreach (OPCData dat in values.Values)
                    {
                        try
                        {
                            String hostname = String.Format("{0}@{1}", dat.Server, dat.Host);
                            if (!servers.ContainsKey(hostname))
                            {
                                OPCServ serv = new OPCServ(dat.Host, dat.Server);
                                serv.DataChangeHandler = OPCGroup_DataChange;
                                serv.AsyncReadCompleteHandler = OPCGroup_AsyncReadComplete;
                                serv.AsyncWriteCompleteHandler = OPCGroup_AsyncWriteComplete;
                                serv.AsyncCancelCompleteHandler = OPCGroup_AsyncCancelComplete;
                                servers.Add(hostname, serv);

                                DispFrame frame = new DispFrame();
                                MainLayout.Controls.Add(frame, 0, frames.Count);
                                MainLayout.RowCount++;
                                MainLayout.RowStyles.Add(new RowStyle());
                                frame.Caption = hostname;
                                frame.Dock = DockStyle.Fill;
                                frames.Add(frame.Caption, frame);
                        }
                        if (servers.ContainsKey(hostname))
                            {
                                servers[hostname].AddItem(dat.Item, dat.Handle);
                            }
                            if (frames.ContainsKey(hostname))
                            {
                                frames[hostname].AddItem(dat.Id, dat.Name, dat.Unit);
                                frames[hostname].SetItemValue(dat.Id, "#####", 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error occurred when creating opc item.", ex);
                        }
                    }
                    foreach (OPCServ serv in servers.Values)
                    {
                        serv.Start();
                    }

            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when reading configuration file.", ex);
            }
            //启动定时器
            Timer Upd_Timer = new Timer();
            Upd_Timer.Interval = 1000;
            Upd_Timer.Tick += Upd_Timer_Tick;
            Upd_Timer.Enabled = true;
        }

        void OPCGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                try
                {
                    Int32 handle = Convert.ToInt32(ClientHandles.GetValue(i));
                    if (values.ContainsKey(handle) && ItemValues.GetValue(i) != null)
                    {
                        OPCData dat = values[handle];
                        dat.SetOpcValue(Convert.ToDouble(ItemValues.GetValue(i)),
                            Convert.ToInt32(Qualities.GetValue(i)),
                            Convert.ToDateTime(TimeStamps.GetValue(i)));
                        String framename = String.Format("{0}@{1}", dat.Server, dat.Host);
                        if (frames.ContainsKey(framename))
                        {
                            frames[framename].SetItemValue(dat.Id, dat.PhyValue, dat.Quality);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when reading opc item data.", ex);
                }
            }
            GC.Collect();
        }

        void OPCGroup_AsyncReadComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps, ref Array Errors)
        {
            OPCGroup_DataChange(TransactionID, NumItems, ref ClientHandles, ref ItemValues, ref Qualities, ref TimeStamps);
        }

        void OPCGroup_AsyncWriteComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array Errors)
        {

        }

        void OPCGroup_AsyncCancelComplete(int CancelID)
        {

        }

        UInt32 tick_count = 0;
        UInt32 post_success = 0;
        UInt32 post_fail = 0;
        DateTime lastFail = new DateTime(0);
        private void PushData(Object data)
        {
            try
            {
                if (data == null)
                {
                    return;
                }
                if (StatNET.Text == defnetmsg)
                {
                    StatNET.Text = "Network Connecting";
                }
                for (int i=0;i<host.Count;i++) { 
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(host[i].TrimEnd('/') + "/" + uuid[i]);
                    byte[] bs = Encoding.UTF8.GetBytes((String)data);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = bs.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Close();
                    }
                    using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            reader.ReadToEnd();
                            reader.Close();
                        }
                        response.Close();
                    }
                }
                post_success++;
                StatNET.Text = "Connected";
                StatNET.ToolTipText = String.Format("{0}\r\nTotal:{1}, Succeed:{2}, Fails:{3}", DateTime.Now, post_success + post_fail, post_success, post_fail);
            }
            catch (Exception ex)
            {
                if (!(ex.GetType() == typeof(WebException) && ((WebException)ex).Status == WebExceptionStatus.ConnectFailure) && (DateTime.Now - lastFail).TotalSeconds >= 180)
                {
                    lastFail = DateTime.Now;
                    Logger.LogError("Error occurred when posting data.", ex);
                }
                post_fail++;
                StatNET.Text = "Connect Failed";
                StatNET.ToolTipText = String.Format("{0}\r\nTotal:{1}, Succeed:{2}, Fails:{3}", DateTime.Now, post_success + post_fail, post_success, post_fail);
            }
        }

        DateTime lastPost = new DateTime(0);
        private void Upd_Timer_Tick(object sender, EventArgs e)
        {
            tick_count++;

            #region Send updated values
            StringBuilder builder = new StringBuilder();
            foreach (OPCData dat in values.Values)
            {
                if (tick_count % dat.Interval == 0 && dat.Isvalid)
                {
                    builder.AppendFormat("&{0}={1}", dat.Id, dat.PhyValue);
                }
            }
            if (builder.Length == 0 && (DateTime.Now - lastPost).TotalSeconds >= 60)
            {
                builder.AppendFormat("&{0}={1}", Guid.Empty.ToString("N"), 1);
            }
            if (builder.Length > 0)
            {
                lastPost = DateTime.Now;
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(PushData));
                thread.Start(builder.ToString().TrimStart('&'));
            }
            #endregion

            #region Update client user interface
            Boolean servok = true;
            builder = new StringBuilder();
            builder.AppendFormat("{0}\r\n", DateTime.Now);
            foreach (OPCServ serv in servers.Values)
            {
                try
                {
                    OPCServerState state = serv.CheckState();
                    builder.AppendFormat("{0}@{1}:{2}\r\n", serv.ServerName, serv.HostName, state);
                    servok &= (state == OPCServerState.OPCRunning);
                }
                catch (Exception ex)
                {
                    builder.AppendFormat("{0}@{1}:{2}\r\n", serv.ServerName, serv.HostName, ex.Message);
                }
            }
            StatOPC.Text = servok ? "OPC Running" : "OPC Failed";
            StatOPC.ToolTipText = builder.ToString();
            #endregion
        }
    }
}
