using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using JerkyTool.Utilities;
using System.Collections.Generic;

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

        #region Others

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
        #endregion

        #region Numeric


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
            if (value == null)
            {
                return false;
            }


            try
            {
                var num = Double.Parse(value);
                return true;
            }
            catch { return false; }

        }


        private static long Fix(double Number)
        {
            if (Number >= 0)
            {
                return (long)Math.Floor(Number);
            }
            return (long)Math.Ceiling(Number);
        }

        public static string ToWord(this double d)
        {
            return NumberToWord.changeNumericToWords(d);
        }

        #endregion

        #region Dates
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

        public static int GetWeekNumber(this DateTime inDate)
        {
            int dayOfYear;
            int wkNumber;
            int compensation = 0;
            string oneDate;
            System.DateTime firstDayDate;
            dayOfYear = inDate.DayOfYear;
            oneDate = "1/1/" + inDate.Year.ToString();
            firstDayDate = DateTime.Parse(oneDate);
            if (firstDayDate.DayOfWeek == DayOfWeek.Sunday)
            {
                compensation = 0;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Monday)
            {
                compensation = 6;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                compensation = 5;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Wednesday)
            {
                compensation = 4;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Thursday)
            {
                compensation = 3;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Friday)
            {
                compensation = 2;
            }
            else if (firstDayDate.DayOfWeek == DayOfWeek.Saturday)
            {
                compensation = 1;
            }
            dayOfYear = dayOfYear - compensation;
            if (dayOfYear % 7 == 0)
            {
                wkNumber = dayOfYear / 7;
            }
            else
            {
                wkNumber = (dayOfYear / 7) + 1;
            }
            return wkNumber;
        }

        public static int AgeCalculate(this DateTime dateOfBirth)
        {
            int dayBirth;
            int monthBirth;
            int yearBirth;

            try
            {
                //Constructor
                dayBirth = dateOfBirth.Day;
                monthBirth = dateOfBirth.Month;
                yearBirth = dateOfBirth.Year;

                DateTime birthDayThisYear = new DateTime(DateTime.Today.Year, monthBirth, dayBirth);
                int years = DateTime.Today.Year - yearBirth;
                int months = DateTime.Today.Month - monthBirth;
                int days = DateTime.Today.Day - dayBirth;

                if (birthDayThisYear > DateTime.Today) //you've not yet celebrated your birthday this year
                {
                    years -= 1;
                    months += 12;
                }
                if (birthDayThisYear.Day > DateTime.Today.Day)
                {
                    months -= 1;
                    DateTime dt = new DateTime(birthDayThisYear.Year, DateTime.Today.Month - 1, birthDayThisYear.Day);
                    TimeSpan ts = DateTime.Today - dt;
                    days = ts.Days;
                }
                return years;
            }
            catch
            {
                return 0;
            }

            //return 0;
        }

        public static List<DateTime> GetWeekDateRange(this DateTime d)
        {
            var list = new List<DateTime>();
            System.DateTime stDate, endDate;

            double offset = 0;

            if (d.DayOfWeek == DayOfWeek.Sunday)
            {
                offset = -6;
            }
            else if (d.DayOfWeek == DayOfWeek.Monday)
            {
                offset = 0;
            }
            else if (d.DayOfWeek == DayOfWeek.Tuesday)
            {
                offset = -1;
            }
            else if (d.DayOfWeek == DayOfWeek.Wednesday)
            {
                offset = -2;
            }
            else if (d.DayOfWeek == DayOfWeek.Thursday)
            {
                offset = -3;
            }
            else if (d.DayOfWeek == DayOfWeek.Friday)
            {
                offset = -4;
            }
            else if (d.DayOfWeek == DayOfWeek.Saturday)
            {
                offset = -5;
            }

            stDate = DateTime.Today.AddDays(offset);
            endDate = DateTime.Today.AddDays(7 + offset - 1);	 // -1 so that from monday to sunday only not monday to monday next week

            list.Add(stDate); // index[0] is the begin of date range
            list.Add(endDate); // index[1] is the end of date range

            return list;

        }


        public static string DayNameFromDayNumber(this int DayOfWeek)
        {
            return DayNameFromDayNumber(DayOfWeek, true);
        }

        public static string DayNameFromDayNumber(this int DayOfWeek, bool bShort)
        {
            DateTime dt = DateTime.Today;

            while (dt.DayOfWeek.ToString() != "Sunday")
                dt = dt.AddDays(-1);

            return DayNameFromDate(dt.AddDays(DayOfWeek - 1), bShort);
        }

        public static string DayNameFromDate(this DateTime dt, bool bShort)
        {
            if (bShort)
            {
                //	Return a short day name.

                return dt.DayOfWeek.ToString().Substring(0, 3);
            }
            else
            {
                //	Return full day name.

                return dt.DayOfWeek.ToString();
            }
        }

        public static string MonthNameFromNumber(this int MonthNo)
        {
            return MonthNameFromNumber(MonthNo, true);
        }

        public static string MonthNameFromNumber(this int MonthNo, Boolean bShort)
        {
            DateTime dt = DateTime.Parse("1 Jan 2000");

            while (MonthNo > 1)
            {
                dt = dt.AddMonths(1);
                MonthNo--;
            }

            if (bShort)
                return dt.ToString("MMMM").Substring(1, 3);
            else
                return dt.ToString("MMMM");
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            while (dt.Day > 1)
                dt = dt.AddDays(-1);

            return dt;
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            Int32 MonthNo = dt.Month;

            while (dt.Month == MonthNo)
                dt = dt.AddDays(1);

            dt = dt.AddDays(-1);
            return dt;
        }

        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            while (dt.DayOfWeek.ToString() != "Monday")
                dt = dt.AddDays(-1);

            return dt;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            while (dt.DayOfWeek.ToString() != "Sunday")
                dt = dt.AddDays(1);

            return dt;
        }

        /// <summary>
        /// Input is second: Sample is 
        /// DateTime.Now.Second
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string HowLongAgo(this int seconds)
        {
            //DateTime.Now.Second
            return HowLongAgo(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// Input is timeStamps
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string HowLongAgo(this TimeSpan ts)
        {

            if (ts.Days > 0)
            {
                if (ts.Days == 1)
                {
                    if (ts.Hours > 2)
                    {
                        return "1 day and " + ts.Hours + " hours ago";
                    }
                    else
                    {
                        return "1 day ago";
                    }
                }
                else
                {
                    return ts.Days + " days ago";
                }
            }
            else if (ts.Hours > 0)
            {
                if (ts.Hours == 1)
                {
                    if (ts.Minutes > 5)
                    {
                        return "1 hour and " + ts.Minutes + " minutes ago";
                    }
                    else
                    {
                        return "1 hour ago";
                    }
                }
                else
                {
                    return ts.Hours + " hours ago";
                }
            }
            else if (ts.Minutes > 0)
            {
                if (ts.Minutes == 1)
                {
                    return "1 minute ago";
                }
                else
                {
                    return ts.Minutes + " minutes ago";
                }
            }
            else
            {

                if (ts.Seconds == 1)
                {
                    return "1 second ago";
                }
                else
                {
                    return ts.Seconds + " seconds ago";
                }

               
            }

        }

        #endregion
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
