using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JerkyTool
{

    public interface IJLogger
    {
        void Log(string message);

        void RollLogFile(string logFilePath);

    }

    public class RollingLogger : IJLogger
    {
        readonly static string LOG_FILE = GetNormalizePath();
        readonly static int MaxRolledLogCount = Properties.Settings.Default.MaxRolledLogCount; //  ConfigurationHelper.MaxRolledLogCount;
        readonly static int MaxLogSize = Properties.Settings.Default.MaxLogSize; // value in bytes 1000bytes = 1kb ; <- small value for testing that it works, you can try yourself, and then use a reasonable size, like 1M-10M
        readonly static string MsgFormat = Properties.Settings.Default.MsgFormat;
        readonly static string MsgDateFormat = Properties.Settings.Default.MsgDateFormat;

        public void Log(string msg)
        {
            var msgInfo = MsgFormat.Replace("%date", DateTime.Now.ToString(MsgDateFormat)).Replace("%message", msg);

            if (Properties.Settings.Default.MsgShowInConsole)
            {
                Console.WriteLine(msgInfo);
            }
            lock (LOG_FILE) // lock is optional, but.. should this ever be called by multiple threads, it is safer
            {
                RollLogFile(LOG_FILE);

                File.AppendAllText(LOG_FILE, msgInfo + Environment.NewLine, Encoding.UTF8);
            }
        }

        public void RollLogFile(string logFilePath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                }

                var length = new FileInfo(logFilePath).Length;

                if (length > MaxLogSize)
                {
                    var path = Path.GetDirectoryName(logFilePath);
                    var wildLogName = Path.GetFileNameWithoutExtension(logFilePath) + "*" + Path.GetExtension(logFilePath);
                    var bareLogFilePath = Path.Combine(path, Path.GetFileNameWithoutExtension(logFilePath));
                    string[] logFileList = Directory.GetFiles(path, wildLogName, SearchOption.TopDirectoryOnly);
                    if (logFileList.Length > 0)
                    {
                        // only take files like logfilename.log and logfilename.0.log, so there also can be a maximum of 10 additional rolled files (0..9)
                        var rolledLogFileList = logFileList.Where(fileName => fileName.Length == (logFilePath.Length + 2)).ToArray();
                        Array.Sort(rolledLogFileList, 0, rolledLogFileList.Length);
                        if (rolledLogFileList.Length >= MaxRolledLogCount)
                        {
                            File.Delete(rolledLogFileList[MaxRolledLogCount - 1]);
                            var list = rolledLogFileList.ToList();
                            list.RemoveAt(MaxRolledLogCount - 1);
                            rolledLogFileList = list.ToArray();
                        }
                        // move remaining rolled files
                        for (int i = rolledLogFileList.Length; i > 0; --i)
                            File.Move(rolledLogFileList[i - 1], bareLogFilePath + "." + i + Path.GetExtension(logFilePath));
                        var targetPath = bareLogFilePath + ".0" + Path.GetExtension(logFilePath);
                        // move original file
                        File.Move(logFilePath, targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private static string GetNormalizePath()
        {

            var filepath = Properties.Settings.Default.LogFile;

            var assemblyFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return System.IO.Path.Combine(assemblyFolder, filepath);
        }
    }
}
