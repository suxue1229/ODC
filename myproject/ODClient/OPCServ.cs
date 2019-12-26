using OPCAutomation;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ODClient
{
    public class OPCServ
    {
        public String HostName, ServerName, GroupName = "ODClient";
        public Int32 UpdateInterval = 500;
        private Timer UpdateTimer = null;
        private OPCServer Server = null;
        private OPCGroup Group = null;
        private Dictionary<String, Int32> Items = new Dictionary<String, Int32>();
        public DIOPCServerEvent_ServerShutDownEventHandler ServerShutDownHandler;
        public DIOPCGroupEvent_DataChangeEventHandler DataChangeHandler;
        public DIOPCGroupEvent_AsyncReadCompleteEventHandler AsyncReadCompleteHandler;
        public DIOPCGroupEvent_AsyncWriteCompleteEventHandler AsyncWriteCompleteHandler;
        public DIOPCGroupEvent_AsyncCancelCompleteEventHandler AsyncCancelCompleteHandler;

        public OPCServ(String host, String server)
        {
            this.HostName = host;
            this.ServerName = server;
        }

        public void Start()
        {
            try
            {
                #region Connect and initialize OPC Server
                Server = new OPCServer();
                Server.Connect(this.ServerName, this.HostName);
                if (ServerShutDownHandler != null)
                {
                    Server.ServerShutDown += ServerShutDownHandler;
                }
                Server.OPCGroups.RemoveAll();
                Group = Server.OPCGroups.Add(GroupName);
                Group.IsSubscribed = true;
                if (DataChangeHandler != null)
                {
                    Group.DataChange += DataChangeHandler;
                }
                if (AsyncReadCompleteHandler != null)
                {
                    Group.AsyncReadComplete += AsyncReadCompleteHandler;
                }
                if (AsyncWriteCompleteHandler != null)
                {
                    Group.AsyncWriteComplete += AsyncWriteCompleteHandler;
                }
                if (AsyncCancelCompleteHandler != null)
                {
                    Group.AsyncCancelComplete += AsyncCancelCompleteHandler;
                }
                #endregion

                #region Register OPC Items
                foreach (String name in Items.Keys)
                {
                    try
                    {
                        Group.OPCItems.AddItem(name, Items[name]);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error occurred when register items.", ex);
                    }
                }
                #endregion

                #region Start server monitor
                if (UpdateTimer == null)
                {
                    UpdateTimer = new Timer(UpdateInterval) { AutoReset = true };
                    UpdateTimer.Elapsed += Update;
                    UpdateTimer.Start();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when connecting server.", ex);
                Group = null;
            }
        }

        private void Update(object sender, ElapsedEventArgs e)
        {
            foreach (String name in Items.Keys)
            {
                try
                {
                    OPCItem item = null;
                    try
                    {
                        item = Group.OPCItems.Item(name);
                    }
                    catch
                    {
                        try
                        {
                            Group.OPCItems.AddItem(name, Items[name]);
                        }
                        catch { }
                    }
                    if (item != null && (DateTime.UtcNow - item.TimeStamp).TotalSeconds > 3)
                    {
                        Array Handles = new[] { 0, item.ServerHandle }, Errors = new[] { 0 };
                        Int32 Cancel = 0;
                        Group.AsyncRead(1, ref Handles, out Errors, 1, out Cancel);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error occurred when updating opc items.", ex);
                }
            }
        }

        public Boolean AddItem(String name, Int32 id)
        {
            if (!Items.ContainsKey(name) && !Items.ContainsValue(id))
            {
                Items.Add(name, id);
                return true;
            }
            return false;
        }

        public OPCServerState CheckState()
        {
            try
            {
                return (OPCServerState)Server.ServerState;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when checking opc server state.", ex);
                Start();
                return OPCServerState.OPCDisconnected;
            }
        }
    }
}
