using System;
using System.IO;
using JerkyTool.Utilities;

namespace JerkyTool
{
    public static class Extension
    {
        public static DateTime ToDateTime(this string s)
        {
            DateTime dtr;
            var tryDtr = DateTime.TryParse(s, out dtr);
            return (tryDtr) ? dtr : default(DateTime);
        }


        public static DateTime ToDate(this string s)
        {
            DateTime dtr;
            var tryDtr = DateTime.TryParse(s, out dtr);
            return (tryDtr) ? dtr.Date : default(DateTime).Date;
        }

        public static int ToInt(this string s)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (FormatException e)
            {
                return 0;
            }
        }

        public static bool ToBool(this string s)
        {
            try
            {
                return Convert.ToBoolean(s);
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        public static double ToDouble(this string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (FormatException e)
            {
                return 0.00;
            }
        }

        public static T ToEnumOnly<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }


        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static string ToEncrypt(this string s)
        {
            return Helper.Encrypt(s);
        }

        public static string ToDecrypt(this string s)
        {
            return Helper.Decrypt(s);
        }

        public static bool IsNumeric(this object value)
        {
            if (value == null || value is DateTime)
            {
                return false;
            }

            if (value is Int16 || value is Int32 || value is Int64 || value is Decimal || value is Single || value is Double || value is Boolean)
            {
                return true;
            }

            try
            {
                if (value is string)
                    Double.Parse(value as string);
                else
                    Double.Parse(value.ToString());
                return true;
            }
            catch { return false; }
          
        }

        public static bool IsNumeric(this string value)
        {
            if (value == null )
            {
                return false;
            }


            try
            {
               var num =   Double.Parse(value);
                return true;
            }
            catch { return false; }
            
        }
    }
}
