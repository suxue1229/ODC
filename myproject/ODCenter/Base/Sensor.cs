using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ODCenter.Models;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ODCenter.Base
{
    public class Sensor
    {
        const UInt32 _hislength = 20;
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Guid Institute { get; set; }
        public Guid? Device { get; set; }
        public Dat_Type DatType { get; set; }
        public Sen_Type SenType { get; set; }
        public Boolean Inverse { get; set; }
        public Int32? Precise { get; set; }
        public Sen_Source Source { get; set; }
        public String OPCHost { get; set; }
        public String OPCServer { get; set; }
        public String OPCItem { get; set; }
        public Int32 Interval { get; set; }
        public Double? Minimum { get; set; }
        public Double? Maximum { get; set; }
        public Double Gain { get; set; }
        public Double Offset { get; set; }
        public String Unit { get; set; }
        public Int32? Order { get; set; }
        public Boolean Important { get; set; }

        private Int32 _count;
        private Double _sum;

        private Double _value;
        private DateTime _time;

        public Double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._count++;
                this._sum += value;
                this._value = value;
                this._time = DateTime.Now;
                if (this.Device != null && this.DatType == Dat_Type.BOOL &&
                    (this.SenType == Sen_Type.Status_Run || this.SenType == Sen_Type.Status_Open ||
                    this.SenType == Sen_Type.Status_Error || this.SenType == Sen_Type.Status_Manual))
                {
                    DbProvider.Devices.Update(this.Device.Value);
                }
            }
        }

        public DateTime Time
        {
            get
            {
                return this._time;
            }
        }

        public Sensor()
        {

        }

        public Sensor(SensorInfo sensor)
            : this()
        {
            this.Id = sensor.Id;
            this.Name = sensor.Name;
            this.Institute = sensor.Institute;
            this.Device = sensor.Device;
            this.DatType = sensor.DatType;
            this.SenType = sensor.SenType;
            this.Inverse = sensor.Inverse;
            this.Precise = sensor.Precise;
            this.Source = sensor.DatSource;
            this.OPCHost = sensor.OPCHost;
            this.OPCServer = sensor.OPCServer;
            this.OPCItem = sensor.OPCItem;
            this.Interval = sensor.Interval;
            this.Minimum = sensor.Minimum;
            this.Maximum = sensor.Maximum;
            this.Gain = sensor.Gain;
            this.Offset = sensor.Offset;
            this.Unit = sensor.Unit;
            this.Order = sensor.Order;
            this.Important = sensor.Important;
        }

        public void Record()
        {
            if (this._count > 0)
            {
                this._track.Add(new DataValue(this._count > 0 ? this._sum / this._count : 0));
                this._count = 0;
                this._sum = 0;
                while (this._track.Count > _hislength)
                {
                    this._track.RemoveAt(0);
                }
            }
        }

        private List<DataValue> _track = new List<DataValue>();

        //Statistics
        public object[] Stats
        {
            get
            {
                if (this._track.Count > 0)
                {
                    Double min = Double.MaxValue, max = Double.MinValue, sum = 0, count = 0;
                    foreach (DataValue dat in _track)
                    {
                        if ((DateTime.Now - dat.Time).TotalMinutes <= 60)
                        {
                            min = Math.Min(min, dat.Value);
                            max = Math.Max(max, dat.Value);
                            sum += dat.Value;
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        return new Object[] { this.Id.ToString(), DateTime.Now, min, max, sum / count, this._value };
                    }
                }
                return null;
            }
        }

        public DataValue[] Track(Int32 level, DateTime time)
        {
            Int32[] lvlmap = { 16, 16, 10, 7, 4 };
            if (level == 0)
            {
                //level-0:cached (20*3minutes)
                return this._track.ToArray();
            }
            else if (level >= 1 && level <= 5)
            {
                StringBuilder builder = new StringBuilder();
                //level-1:1day (24*1hours)
                //level-2:1week (7*4*6hours)
                //level-3:1month (30*1day)
                //level-4:1year (12*1month)
                //level-5:years (n*1year)
                builder.AppendFormat("select convert(varchar({0}),time,120) as date,avg(avg) as avg from sensor_dat ", lvlmap[level - 1]);
                builder.AppendFormat("where id='{0}' and datepart(mi,time)=0 ", this.Id.ToString());
                switch (level)
                {
                    case 1:
                        builder.AppendFormat("and time>'{0:yyyy-MM-dd HH:mm}' and time<='{1:yyyy-MM-dd HH:mm}' ", time.AddDays(-1), time);
                        break;
                    case 2:
                        builder.AppendFormat("and datepart(hh,time)%6=0 ");
                        builder.AppendFormat("and time>'{0:yyyy-MM-dd HH:mm}' and time<='{1:yyyy-MM-dd HH:mm}' ", time.AddDays(-7), time);
                        break;
                    case 3:
                        builder.AppendFormat("and time>'{0:yyyy-MM-dd HH:mm}' and time<='{1:yyyy-MM-dd HH:mm}' ", time.AddMonths(-1), time);
                        break;
                    case 4:
                        builder.AppendFormat("and time>'{0:yyyy-MM-dd HH:mm}' and time<='{1:yyyy-MM-dd HH:mm}' ", time.AddYears(-1), time);
                        break;
                    case 5:
                        builder.AppendFormat("and time<='{0:yyyy-MM-dd HH:mm}' ", time);
                        break;
                }
                builder.AppendFormat("group by convert(varchar({0}),time,120) order by convert(varchar({0}),time,120)", lvlmap[level - 1]);
                List<DataValue> sendat = new List<DataValue>();
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    Statistic.LogDbRequest("data");
                    conn.Open();
                    SqlDataAdapter sqlda = new SqlDataAdapter(builder.ToString(), conn);
                    DateTime tim;
                    Double val;
                    using (DataTable dt = new DataTable())
                    {
                        sqlda.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (level == 5)
                            {
                                if (DateTime.TryParse(dr.ItemArray[0].ToString() + "-1", out tim) && Double.TryParse(dr.ItemArray[1].ToString(), out val))
                                {
                                    sendat.Add(new DataValue(tim, val));
                                }
                            }
                            else
                            {
                                if (DateTime.TryParse(dr.ItemArray[0].ToString(), out tim) && Double.TryParse(dr.ItemArray[1].ToString(), out val))
                                {
                                    sendat.Add(new DataValue(tim, val));
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                return sendat.ToArray();
            }
            else
            {
                return new DataValue[0];
            }
        }

        public static DataTable[] Track(DataSource source)
        {
            Int32[] lvlmap = { 16, 13, 10, 5, 7, 4, 4 };
            List<DataTable> result = new List<DataTable>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                Statistic.LogDbRequest("data");
                conn.Open();
                foreach (DataItem item in source.Items)
                {
                    if (item.Ids == null || item.Ids.Length <= 0)
                    {
                        result.Add(new DataTable(item.Name));
                        continue;
                    }
                    List<String> ids = new List<String>();
                    foreach (Guid id in item.Ids)
                    {
                        ids.Add(id.ToString());
                    }
                    DataStore store = new DataStore(ids.ToArray());
                    if (item.Level == DataLevel.Cached)
                    {
                        #region 从缓存中获取数据

                        foreach (Guid id in item.Ids)
                        {
                            if (DbProvider.Sensors.ContainsKey(id.ToString("N")))
                            {
                                foreach (DataValue val in DbProvider.Sensors[id.ToString("N")]._track)
                                {
                                    String timestr = val.Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    store.AddValue(timestr, id.ToString(), val.Value.ToString("0.0000"));
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        try
                        {
                            #region 从数据库中获取数据
                            DateTime time = item.Time.HasValue ? item.Time.Value : DateTime.Now;
                            if (item.Slip)
                            {
                                switch (item.Span)
                                {
                                    case DataLevel.Minute:
                                        time = DateTime.Parse(String.Format("{0:yyyy-MM-dd HH:mm}:00", time));
                                        break;
                                    case DataLevel.Hour:
                                        time = DateTime.Parse(String.Format("{0:yyyy-MM-dd HH}:00:00", time));
                                        break;
                                    case DataLevel.Day:
                                        time = DateTime.Parse(String.Format("{0:yyyy-MM-dd} 00:00:00", time));
                                        break;
                                    case DataLevel.Week:
                                        time = time.AddDays(-1 * (Double)time.DayOfWeek);
                                        time = DateTime.Parse(String.Format("{0:yyyy-MM-dd} 00:00:00", time));
                                        break;
                                    case DataLevel.Month:
                                        time = DateTime.Parse(String.Format("{0:yyyy-MM}-01 00:00:00", time));
                                        break;
                                    case DataLevel.Year:
                                        time = DateTime.Parse(String.Format("{0:yyyy}-01-01 00:00:00", time));
                                        break;
                                    case DataLevel.Decade:
                                        time = new DateTime(0);
                                        break;
                                }
                            }
                            String timestr = String.Format("convert(varchar({0}),time,120)", lvlmap[(int)item.Level - 1]);
                            if (item.Level == DataLevel.Week)
                            {
                                timestr += "+right(replicate('0',3)+convert(varchar(3),datepart(ww,time)),3)";
                            }
                            builder.AppendFormat("select {0} as date, id, avg(avg) as value from sensor_dat where id in (", timestr);
                            for (int i = 0; i < item.Ids.Length; i++)
                            {
                                builder.AppendFormat((i == 0) ? "'{0}'" : ",'{0}'", item.Ids[i].ToString());
                            }
                            builder.AppendFormat(") and time>='{0:yyyy-MM-dd HH:mm}' ", time);
                            if (item.Level != DataLevel.Decade)
                            {
                                switch (item.Span)
                                {
                                    case DataLevel.Minute:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddMinutes(1));
                                        break;
                                    case DataLevel.Hour:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddHours(1));
                                        break;
                                    case DataLevel.Day:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddDays(1));
                                        break;
                                    case DataLevel.Week:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddDays(7));
                                        break;
                                    case DataLevel.Month:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddMonths(1));
                                        break;
                                    case DataLevel.Year:
                                        builder.AppendFormat("and time<'{0:yyyy-MM-dd HH:mm}' ", time.AddYears(1));
                                        break;
                                }
                            }
                            builder.AppendFormat("group by {0}, id order by date, id", timestr);
                            SqlDataAdapter sqlda = new SqlDataAdapter(builder.ToString(), conn);
                            using (DataTable dt = new DataTable())
                            {
                                if (sqlda.Fill(dt) > 0)
                                {
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        String col1 = dr.ItemArray[0].ToString();
                                        if (col1.Length == 0)
                                        {
                                            continue;
                                        }
                                        if (col1.Length == 4)
                                        {
                                            col1 += "-01";
                                        }
                                        if (col1.Length == 7)
                                        {
                                            col1 += "-01";
                                        }
                                        if (col1.Length == 10)
                                        {
                                            col1 += " 00";
                                        }
                                        if (col1.Length == 13)
                                        {
                                            col1 += ":00";
                                        }
                                        if (col1.Length == 16)
                                        {
                                            col1 += ":00";
                                        }
                                        store.AddValue(DateTime.Parse(col1).ToString("yyyy-MM-dd HH:mm:ss"), dr.ItemArray[1].ToString(), dr.ItemArray[2].ToString());
                                    }
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Error occurred when retrieving data.\r\nQuery String:" + builder.ToString(), ex);
                        }
                    }
                    DataTable table = store.GetTable(item.Limit);
                    table.TableName = item.Name;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        table.Columns[i].ColumnName = (i == 0) ? "Time" : String.Format("Data{0}", i);
                    }
                    result.Add(table);
                }
                conn.Close();
            }
            return result.ToArray();
        }
    }

    public class DataSource
    {
        public DataItem[] Items;

        public static DataSource Parse(String config)
        {
            try
            {
                return (DataSource)JsonConvert.DeserializeObject(config, typeof(DataSource));
            }
            catch { }
            return null;
        }

        public override String ToString()
        {
            Object obj = new
            {
                data = Items
            };
            return JsonConvert.SerializeObject(this);
        }
    }

    public class DataItem
    {
        public String Name;
        public Guid[] Ids;
        public DataLevel Span;
        public DataLevel Level;
        public Boolean Slip;
        public Int32 Limit;
        public DateTime? Time;
    }

    public class DataStore
    {
        public String PlaceHolder = "--";
        private DataTable data;
        private Dictionary<String, Int32> index;

        public DataStore(String[] names)
        {
            data = new DataTable();
            data.Columns.Add(new DataColumn("tag", typeof(String)));
            index = new Dictionary<String, Int32>();
            for (int i = 0; i < names.Length; i++)
            {
                data.Columns.Add(new DataColumn(names[i], typeof(String)) { DefaultValue = PlaceHolder });
                index.Add(names[i], i + 1);
            }
            data.PrimaryKey = new[] { data.Columns[0] };
        }

        public void AddValue(String tag, String name, String value)
        {
            if (!index.ContainsKey(name))
            {
                return;
            }
            DataRow dr = data.Rows.Find(tag);
            if (dr == null)
            {
                dr = data.NewRow();
                dr[0] = tag;
                data.Rows.Add(dr);
            }
            dr[index[name]] = value;
        }

        public DataTable GetTable(Int32 limit)
        {
            if (limit <= 0 || limit >= data.Rows.Count)
            {
                return data;
            }
            else
            {
                List<String> tags = new List<String>();
                foreach (DataRow row in data.Rows)
                {
                    tags.Add(row.ItemArray[0].ToString());
                }
                tags.Sort();
                DataTable tab = data.Copy();
                for (int i = 0; i < tags.Count && tab.Rows.Count > limit; i++)
                {
                    tab.Rows.Remove(tab.Rows.Find(tags[i]));
                }
                return tab;
            }
        }
    }

    public class DataValue
    {
        private Double _value;
        public Double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this._time = DateTime.Now;
            }
        }

        private DateTime _time;
        public DateTime Time
        {
            get
            {
                return this._time;
            }
        }

        public DataValue(Double val)
        {
            Value = val;
        }

        public DataValue(DateTime time, Double val)
        {
            this._time = time;
            this._value = val;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataLevel
    {
        Cached,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year,
        Decade
    }
}