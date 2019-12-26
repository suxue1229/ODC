using PTR.Extension;
using PTR.Logging;
using PTR.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ODClisrv
{
    public class FileMainten
    {
        private String _file = String.Empty;
        private Boolean _fmode = true;
        public String FileName
        {
            get
            {
                return this._file;
            }
            set
            {
                this._file = value;
            }
        }

        private String _hash = String.Empty;
        public String FileHash
        {
            get
            {
                return this._hash;
            }
        }

        private DateTime _time = new DateTime(0);
        public DateTime FileTime
        {
            get
            {
                return this._time;
            }
        }

        public FileMainten(String file, Boolean fmode)
        {
            this._file = file;
            this._fmode = fmode;
            if (File.Exists(this._file))
            {
                CalcHash();
                this._time = File.GetLastWriteTime(this._file);
            }
        }

        private void CalcHash()
        {
            try
            {
                if (File.Exists(this._file))
                {
                    using (FileStream stream = new FileStream(this._file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (_fmode)
                        {
                            Byte[] hash = SHA1.Create().ComputeHash(stream);
                            this._hash = hash.ToString(true);
                        }
                        else
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                String content = reader.ReadToEnd();                            
                                Byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
                                this._hash = hash.ToString(true);
                            }
                        }
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(String.Format("Error occurred when hashing file.\r\nFile:{0}", this._file), ex);
            }
        }

        public Boolean IsModified(String hash)
        {
            if (File.Exists(this._file))
            {
                if (File.GetLastWriteTime(this._file) != this._time)
                {
                    CalcHash();
                    this._time = File.GetLastWriteTime(this._file);
                }
                return (String.Compare(this._hash, hash, true) != 0);
            }
            else
            {
                return true;
            }
        }

        public void ReplaceContent(String content)
        {
            try
            {
                String file = Path.GetTempFileName();
                using (FileStream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.Write(content);
                        writer.Flush();
                        writer.Close();
                    }
                    stream.Close();
                }
                if (File.Exists(file))
                {
                    if (File.Exists(this._file))
                    {
                        File.Delete(this._file);
                    }
                    File.Move(file, this._file);
                }
                CalcHash();
                this._time = File.GetLastWriteTime(this._file);
                Logger.LogMessage(String.Format("Update file.\r\nSource File:{0}\r\nTarget File:{1}", file, this._file));
            }
            catch (Exception ex)
            {
                Logger.LogError(String.Format("Error occurred when replacing file.\r\nFile content:{0}\r\nTarget file:{1}", content, this._file), ex);
            }
        }

        public void ExceteUpdate(String file)
        {
            try
            {
                CloseExec();
                Process proc = new Process();
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = "/verysilent";
                proc.Start();
                Logger.LogMessage(String.Format("Update client application with {0}.", file));
                proc.WaitForExit();
                CalcHash();
                this._time = File.GetLastWriteTime(this._file);
            }
            catch (Exception ex)
            {
                Logger.LogError(String.Format("Error occurred when replacing file.\r\nSource file:{0}\r\nTarget file:{1}", file, this._file), ex);
            }
        }

        private Boolean CloseExec()
        {
            Boolean procexist = false;
            Process[] procs = Process.GetProcesses();
            foreach (Process p in procs)
            {
                
                try
                {
                    if (String.Compare(p.MainModule.FileName, this._file, true) == 0)
                    {
                        p.Kill();
                        procexist = true;
                    }
                }
                catch (Exception ex) {
                    Logger.LogError(String.Format("Error occurred when replacing file"), ex);
                }
            }
            return procexist;
        }

        public void RestartExec(WindowStyle style = WindowStyle.Normal, String args = null)
        {
            Boolean aa=CloseExec();
            Thread.Sleep(3000);
            PTR.Diagnostics.Process.Start(this._file, style, args);
        }
    }
}
