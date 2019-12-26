using PTR.Extension;
using PTR.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ODCenter.Base
{
    public class Update
    {
        private static DateTime lastupdate = new DateTime(0);
        private static DateTime clientdate = new DateTime(0);
        private static DateTime updatedate = new DateTime(0);
        private static String clienthash = null, updatehash = null;

        private static String GetFileHash(String file)
        {
            try
            {
                SHA1 sha1 = SHA1.Create();
                Byte[] hash = sha1.ComputeHash(new byte[0]);
                if (System.IO.File.Exists(file))
                {
                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        hash = sha1.ComputeHash(stream);
                        stream.Close();
                    }
                }
                return hash.ToString(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(String.Format("Error occurred when hashing file.\r\nFile:{0}", file), ex);
                return null;
            }
        }

        private static void CheckUpdate()
        {
            if ((DateTime.Now - lastupdate).TotalMilliseconds > 10000)
            {
                lastupdate = DateTime.Now;
                String client = HttpContext.Current.Server.MapPath("/Update/ODClient.exe");
                String update = HttpContext.Current.Server.MapPath("/Update/Update.exe");
                if (System.IO.File.Exists(client))
                {
                    if (System.IO.File.GetLastWriteTime(client) != clientdate)
                    {
                        clienthash = GetFileHash(client);
                        clientdate = File.GetLastWriteTime(client);
                    }
                }
                if (System.IO.File.Exists(update))
                {
                    if (System.IO.File.GetLastWriteTime(update) != updatedate)
                    {
                        updatehash = GetFileHash(update);
                        updatedate = File.GetLastWriteTime(update);
                    }
                }
            }
        }

        public static String ClientHash
        {
            get
            {
                CheckUpdate();
                return clienthash;
            }
        }

        public static String UpdateHash
        {
            get
            {
                CheckUpdate();
                return updatehash;
            }
        }
    }
}