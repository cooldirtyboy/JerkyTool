using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using JerkyTool.Utilities;

namespace JerkyTool
{
    public enum DateInterval
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Weekday,
        Second
    }

    public static class Extension
    {
        public static DateTime ToDateTime(this string s)
        {
            DateTime dtr;
            var tryDtr = DateTime.TryParse(s, out dtr);
            return (tryDtr) ? dtr : default(DateTime);
        }

        public static long DateDiff(this DateTime date1, DateTime date2, DateInterval interval)
        {

            TimeSpan ts = ts = date2 - date1;
            
            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;
                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
                case DateInterval.Weekday:
                    return Fix(ts.TotalDays) / 7;
                case DateInterval.Day:
                    return Fix(ts.TotalDays);
                case DateInterval.Hour:
                    return Fix(ts.TotalHours);
                case DateInterval.Minute:
                    return Fix(ts.TotalMinutes);
                default:
                    return Fix(ts.TotalSeconds);
            }
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

        /// <summary>
        /// Validates the string is an Email Address...
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>bool</returns>
        public static bool IsValidEmailAddress(this string emailAddress)
        {
            var valid = true;
            var isnotblank = false;

            var email = emailAddress.Trim();
            if (email.Length > 0)
            {
                // Email Address Cannot start with period.
                // Name portion must be at least one character
                // In the Name, valid characters are:  a-z 0-9 ! # _ % & ' " = ` { } ~ - + * ? ^ | / $
                // Cannot have period immediately before @ sign.
                // Cannot have two @ symbols
                // In the domain, valid characters are: a-z 0-9 - .
                // Domain cannot start with a period or dash
                // Domain name must be 2 characters.. not more than 256 characters
                // Domain cannot end with a period or dash.
                // Domain must contain a period
                isnotblank = true;
                valid = Regex.IsMatch(email, RegexExtension.EmailPattern, RegexOptions.IgnoreCase) &&
                    !email.StartsWith("-") &&
                    !email.StartsWith(".") &&
                    !email.EndsWith(".") &&
                    !email.Contains("..") &&
                    !email.Contains(".@") &&
                    !email.Contains("@.");
            }

            return (valid && isnotblank);
        }

        /// <summary>
        /// Validates the string is an Email Address or a delimited string of email addresses...
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>bool</returns>
        public static bool IsValidEmailAddressDelimitedList(this string emailAddress, char delimiter = ';')
        {
            var valid = true;
            var isnotblank = false;

            string[] emails = emailAddress.Split(delimiter);

            foreach (string e in emails)
            {
                var email = e.Trim();
                if (email.Length > 0 && valid) // if valid == false, no reason to continue checking
                {
                    isnotblank = true;
                    if (!email.IsValidEmailAddress())
                    {
                        valid = false;
                    }
                }
            }
            return (valid && isnotblank);
        }



        public static bool IsNullOrEmpty(this Guid guid)
        {
            return (guid == Guid.Empty);
        }

        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public static string Mid(this string param, int startIndex, int length)
        {
            string result = param.Substring(startIndex, length);
            return result;
        }

        public static string ToXml(this object obj)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                s.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        public static T FromXml<T>(this string data)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(data))
            {
                object obj = s.Deserialize(reader);
                return (T)obj;
            }
        }

        public static string ReadToEnd(this MemoryStream BASE)
        {
            BASE.Position = 0;
            StreamReader R = new StreamReader(BASE);
            return R.ReadToEnd();
        }

        public static string ReadAsString(this Stream stream)
        {
            var startPosition = stream.Position;
            try
            {
                // 1. Check for a BOM
                // 2. or try with UTF-8. The most (86.3%) used encoding. Visit: http://w3techs.com/technologies/overview/character_encoding/all/
                var streamReader = new StreamReader(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true), detectEncodingFromByteOrderMarks: true);
                return streamReader.ReadToEnd();
            }
            catch (DecoderFallbackException ex)
            {
                stream.Position = startPosition;

                // 3. The second most (6.7%) used encoding is ISO-8859-1. So use Windows-1252 (0.9%, also know as ANSI), which is a superset of ISO-8859-1.
                var streamReader = new StreamReader(stream, Encoding.GetEncoding(1252));
                return streamReader.ReadToEnd();
            }
        }


        public static string ToCsv(this DataTable dataTable)
        {
            StringBuilder sbData = new StringBuilder();

            // Only return Null if there is no structure.
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                    sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                        sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
                }
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }

            return sbData.ToString();
        }

        private static long Fix(double Number)
        {
            if (Number >= 0)
            {
                return (long)Math.Floor(Number);
            }
            return (long)Math.Ceiling(Number);
        }
    }

    public class RegexExtension
    {
        /// <summary>
        /// Set of Unicode Characters currently supported in the application for email, etc.
        /// </summary>
        public static readonly string UnicodeCharacters = "À-ÿ{L}{M}ÀàÂâÆæÇçÈèÉéÊêËëÎîÏïÔôŒœÙùÛûÜü«»€₣äÄöÖüÜß"; // German and French

        /// <summary>
        /// Set of Symbol Characters currently supported in the application for email, etc.
        /// Needed if a client side validator is being used.
        /// Not needed if validation is done server side.
        /// The difference is due to subtle differences in Regex engines.
        /// </summary>
        public static readonly string SymbolCharacters = @"!#%&'""=`{}~\.\-\+\*\?\^\|\/\$";

        /// <summary>
        /// Regular Expression string pattern used to match an email address.
        /// The following characters will be supported anywhere in the email address:
        /// ÀàÂâÆæÇçÈèÉéÊêËëÎîÏïÔôŒœÙùÛûÜü«»€₣äÄöÖüÜß[a - z][A - Z][0 - 9] _
        /// The following symbols will be supported in the first part of the email address(before the @ symbol):
        /// !#%&'"=`{}~.-+*?^|\/$
        /// Emails cannot start or end with periods,dashes or @.
        /// Emails cannot have two @ symbols.
        /// Emails must have an @ symbol followed later by a period.
        /// Emails cannot have a period before or after the @ symbol.
        /// </summary>
        public static readonly string EmailPattern = String.Format(
            @"^([\w{0}{2}])+@{1}[\w{0}]+([-.][\w{0}]+)*\.[\w{0}]+([-.][\w{0}]+)*$",                     //  @"^[{0}\w]+([-+.'][{0}\w]+)*@[{0}\w]+([-.][{0}\w]+)*\.[{0}\w]+([-.][{0}\w]+)*$",
            UnicodeCharacters,
            "{1}",
            SymbolCharacters
        );
    }
}
