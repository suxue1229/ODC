using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ODCenter.Base
{
    public class Statistic
    {
        public class ServiceStatus
        {
            DateTime _startup = DateTime.Now;

            public DateTime Startup
            {
                get
                {
                    return _startup;
                }
            }

            public TimeSpan Duration
            {
                get
                {
                    return DateTime.Now - _startup;
                }
            }
        }

        static ServiceStatus _service = new ServiceStatus();

        public static ServiceStatus Service
        {
            get
            {
                return _service;
            }
        }

        static Dictionary<String, Int64> _webrequest = new Dictionary<String, Int64>();

        public static Dictionary<String, Int64> WebRequests
        {
            get
            {
                return _webrequest;
            }
        }

        public static void LogWebRequest(String path)
        {
            if (_webrequest.ContainsKey(path))
            {
                _webrequest[path]++;
            }
            else
            {
                _webrequest.Add(path, 1);
            }
        }

        static Dictionary<String, Int64> _dbrequest = new Dictionary<String, Int64>();

        public static Dictionary<String, Int64> DbRequests
        {
            get
            {
                return _dbrequest;
            }
        }

        public static void LogDbRequest(String name)
        {
            if (_dbrequest.ContainsKey(name))
            {
                _dbrequest[name]++;
            }
            else
            {
                _dbrequest.Add(name, 1);
            }
        }

        public class HistorianRecord
        {
            public UInt64 Count { get; internal set; }

            public DateTime Time { get; internal set; }

            public TimeSpan Elapsed { get; internal set; }
        }

        public class HistorianStatus : List<HistorianRecord>
        {
            HistorianRecord record;
            Stopwatch _watch = new Stopwatch();

            public void Start(UInt64 count)
            {
                record = new HistorianRecord()
                {
                    Count = count,
                    Time = DateTime.Now
                };
                _watch.Restart();
            }

            public void Stop()
            {
                _watch.Stop();
                if (record != null)
                {
                    record.Elapsed = _watch.Elapsed;
                    Add(record);
                    record = null;
                }
                while (Count > 120)
                {
                    RemoveAt(0);
                }
            }
        }

        static HistorianStatus _historian = new HistorianStatus();

        public static HistorianStatus Historian
        {
            get
            {
                return _historian;
            }
        }
    }
}