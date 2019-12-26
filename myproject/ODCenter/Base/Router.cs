using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ODCenter.Base
{
    public class Router
    {
        public static String Manbase = String.Empty;

        public static String Combine(String url)
        {
            return Manbase.TrimEnd('/') + "/" + url.TrimStart('/');
        }

        public static String ParamString(NameValueCollection keys)
        {
            StringBuilder builder = new StringBuilder();
            foreach (String k in keys.AllKeys)
            {
                if (builder.Length != 0)
                {
                    builder.Append("&");
                }
                builder.AppendFormat("{0}={1}", k, keys[k]);
            }
            return builder.ToString();
        }

        public static String HttpGet(String url)
        {
            return HttpPost(url, null);
        }

        public static String HttpPost(String url, String data)
        {
            String result = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                if (!String.IsNullOrWhiteSpace(data))
                {
                    byte[] bs = Encoding.UTF8.GetBytes(data);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = bs.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Flush();
                        reqStream.Close();
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                        reader.Close();
                    }
                    response.Close();
                }
            }
            catch { }
            return result;
        }
    }
}