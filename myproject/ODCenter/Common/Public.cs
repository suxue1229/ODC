using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ODCenter
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecvStat
    {
        Data_Received,
        Data_Error,
        Hash_Mismatch,
        Config_Outdate,
    }

    public class Smart
    {
        static Int32[] StatList = new Int32[] { 0, 1, 2, 3, 4, 5, 6, 16, 17, 18, 19, 20, 21, 32, 33, 34, 48, 49 };
        static Int32[] FuncList = new Int32[] { 4, 5, 7, 20 };

        public static Boolean StatusExist(Int32 id)
        {
            Boolean enset;
            return !String.IsNullOrWhiteSpace(StatusName(id, out enset));
        }

        public static Boolean StatusWritable(Int32 id)
        {
            Boolean enset;
            return !String.IsNullOrWhiteSpace(StatusName(id, out enset)) && enset;
        }

        public static Int32[] StatusCtrlist()
        {
            List<Int32> ctrl = new List<Int32>();
            foreach (Int32 id in StatList)
            {
                if (StatusWritable(id))
                {
                    ctrl.Add(id);
                }
            }
            return ctrl.ToArray();
        }

        public static String StatusName(Int32 id, out Boolean enset)
        {
            String name = null;
            enset = false;
            switch (id)
            {
                case 0:
                    name = "PLC编号";
                    enset = true;
                    break;
                case 1:
                    name = "DTU编号";
                    break;
                case 2:
                    name = "软件版本";
                    break;
                case 3:
                    name = "加密支持";
                    break;
                case 4:
                    name = "系统时间";
                    enset = true;
                    break;
                case 5:
                    name = "启动时间";
                    break;
                case 6:
                    name = "GPS刷新时间";
                    break;
                case 16:
                    name = "发送数据包统计";
                    enset = true;
                    break;
                case 17:
                    name = "接收数据包统计";
                    enset = true;
                    break;
                case 18:
                    name = "发送失败统计";
                    enset = true;
                    break;
                case 19:
                    name = "接收失败统计";
                    enset = true;
                    break;
                case 20:
                    name = "发送字节统计";
                    enset = true;
                    break;
                case 21:
                    name = "接收字节统计";
                    enset = true;
                    break;
                case 32:
                    name = "数据同步间隔";
                    enset = true;
                    break;
                case 33:
                    name = "状态同步间隔";
                    enset = true;
                    break;
                case 34:
                    name = "GPS同步间隔";
                    enset = true;
                    break;
                case 48:
                    name = "GPS经度";
                    break;
                case 49:
                    name = "GPS纬度";
                    break;
            }
            return name;
        }

        public static Boolean FunctionExist(Int32 id)
        {
            Boolean enset;
            return !String.IsNullOrWhiteSpace(FunctionName(id, out enset));

        }

        public static Boolean FunctionWritable(Int32 id)
        {
            Boolean enset;
            return !String.IsNullOrWhiteSpace(FunctionName(id, out enset)) && enset;
        }

        public static Int32[] FunctionCtrlist()
        {
            List<Int32> ctrl = new List<Int32>();
            foreach (Int32 id in FuncList)
            {
                if (FunctionWritable(id))
                {
                    ctrl.Add(id);
                }
            }
            return ctrl.ToArray();
        }

        public static String FunctionName(Int32 id, out Boolean enset)
        {
            String name = null;
            enset = false;
            switch (id)
            {
                case 4:
                    name = "同步状态";
                    enset = true;
                    break;
                case 5:
                    name = "同步数据";
                    enset = true;
                    break;
                case 7:
                    name = "同步GPS";
                    enset = true;
                    break;
                case 20:
                    name = "流量统计清零";
                    enset = true;
                    break;
            }
            return name;
        }
    }
}