using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YM.Common.DataTypeEnum;

namespace YM.Common
{
    public static class DataTypeConversion
    {
        public static short ToSafeShort(this object obj)
        {
            string value = obj.ToSafeString();
            return string.IsNullOrEmpty(value) ? (short)0 : Convert.ToInt16(value);
        }
        public static int ToSafeInt(this object obj)
        {
            string value = obj.ToSafeString();

            if (value.ToHasValue())
            {
                if (Int32.TryParse(value, out int result))
                    return result;
                else
                    return 0;
            }
            else
                return 0;
        }
        public static int ToSafeInt(this object obj, bool isDecimal)
        {
            string value = obj.ToSafeString();
            return Convert.ToInt32(value);
        }
        public static int? ToSafeIntNull(this object obj)
        {
            string value = obj.ToSafeString();
            return string.IsNullOrEmpty(value) ? (int?)null : Convert.ToInt32(value);
        }
        public static long ToSafeLong(this object obj)
        {
            string value = obj.ToSafeString();
            if (string.IsNullOrEmpty(value))
                return 0;

            return Convert.ToInt64(value);
        }
        public static long ToSafeLong(this object obj, bool isDecimal)
        {
            decimal value = obj.ToSafeDecimal();
            return Convert.ToInt64(value);
        }
        public static decimal ToSafeDecimal(this object obj)
        {
            string value = obj.ToSafeString();
            if (string.IsNullOrEmpty(value))
                return 0;

            decimal result = Convert.ToDecimal(value);

            return result.ToSafeDecimal();
        }
        public static decimal ToSafeDecimal(this decimal obj)
        {
            string removeLeadingZero = obj.ToString("#.##############", CultureInfo.InvariantCulture);

            if (removeLeadingZero.ToHasValue())
                return Convert.ToDecimal(removeLeadingZero);

            return obj;
        }
        public static decimal ToSafeDecimal(this DateTime date) => Convert.ToDecimal(date.ToString("yyyyMMddHHmmss"));
        public static decimal ToSafeDecimalRound0(this object obj) => Math.Round(obj.ToSafeDecimal(), 0);
        public static decimal ToSafeDecimalRound0(this decimal obj) => Math.Round(obj, 0);
        public static decimal ToSafeDecimalRound3(this object obj) => Math.Round(obj.ToSafeDecimal(), 3);
        public static decimal ToSafeDecimalRound3(this decimal obj) => Math.Round(obj, 3);
        public static decimal ToSafeDecimalFloor2(this object obj)
        {
            var power = Math.Pow(10, 2).ToSafeDecimal();
            decimal value = obj.ToSafeDecimal();

            return Math.Floor(value * power) / power;
        }
        public static decimal ToSafeDecimalFloor2(this decimal value)
        {
            var power = Math.Pow(10, 2).ToSafeDecimal();

            return Math.Floor(value * power) / power;
        }

        public static bool ToSafeBool(this object obj)
        {
            string value = obj.ToSafeString(BoolFormat.TRUEFALSE);
            if (string.IsNullOrEmpty(value) || value == "False")
                return false;
            else
                return true;
        }
        public static string ToSafeString(this object obj, BoolFormat boolFormat = BoolFormat.NONE)
        {
            string value = obj.ToSafeLower();
            if (string.IsNullOrEmpty(value) || boolFormat == BoolFormat.NONE)
                return value;

            if (BoolFormat.TRUEFALSE == boolFormat)
            {
                if (value == "0" || value == "false" || value == "no") // || value == string.Empty
                    return "False";
                else if (value == "1" || value == "true" || value == "yes")
                    return "True";


            }
            else if (BoolFormat.YESNO == boolFormat)
            {
                if (value == "0" || value == "false" || value == "no")
                    return "No";
                else if (value == "1" || value == "true" || value == "yes")
                    return "Yes";
            }
            else if (BoolFormat.ZEROONE == boolFormat)
            {
                if (value == "0" || value == "false" || value == "no")
                    return "0";
                else if (value == "1" || value == "true" || value == "yes")
                    return "1";
            }


            return value;
        }
        public static string ToSafeString(this object? obj) => (obj?.ToString() ?? "").Trim();
        public static string ToSafeLower(this object obj) => obj.ToSafeString().ToLower();
        public static string ToSafeUpper(this object obj) => obj.ToSafeString().ToUpper();
        public static string ToSafeTitleCase(this object obj) =>
                            CultureInfo.InvariantCulture.TextInfo.ToTitleCase(obj.ToSafeString().ToLower());
        public static string ToSafeReplaceEmpty(this object obj) => obj.ToSafeString().Replace(" ", string.Empty);
        public static string ToSafeCleanHtml(this object obj)
        {
            return obj.ToSafeString()
                        .Replace("\r", "")
                        .Replace("\n", "")
                        .Replace("\t", "");
        }
        public static string ToSafeDateTimeAsString(this object obj, string datetimePattern)
        {
            if (obj != null && obj is DateTime)
                return Convert.ToDateTime(obj).ToString(datetimePattern);
            else
            {
                var val = obj?.ToSafeString();
                if (string.IsNullOrEmpty(val))
                    return "";

                DateTime dateTime = DateTime.ParseExact(val, datetimePattern, CultureInfo.InvariantCulture);
                return dateTime.ToString(datetimePattern);
            }
        }
        public static DateTime ToSafeDateTime(this decimal value, IFormatProvider provider) => DateTime.ParseExact(value.ToString(provider), "yyyyMMddHHmmss", provider);
        public static DateTime? ToSafeDateTime(this object value)
        {
            var obj = value.ToSafeString();
            if (string.IsNullOrEmpty(obj))
                return null;

            bool isConvert = DateTime.TryParse(obj, out DateTime result);

            if (isConvert)
                return result;

            return null;
        }
        public static DateTime ToSafeDateEndTime(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }
        public static object ToSafeDbValue(this object? obj)
        {
            if (obj == null)
                return DBNull.Value;

            string str = obj.ToSafeString();

            if (string.IsNullOrEmpty(str))
                return DBNull.Value;

            return str;
        }
        public static string ToSafeHexString(this byte[] bytes)
        {
            return bytes != null ? BitConverter.ToString(bytes).Replace("-", string.Empty) : string.Empty;
        }
        public static string ToStringWithoutDash(this Guid guid)
        {
            return guid
                    .ToSafeUpper()
                    .Replace("-", string.Empty);
        }
        public static string ToSafeHowLongAgo(this int seconds)
        {
            return new TimeSpan(0, 0, seconds).ToSafeHowLongAgo();
        }
        public static string ToSafeHowLongAgo(this TimeSpan ts)
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
                return ts.Seconds + " seconds ago";
            }

        }
        public static string ToSafeWait(this TimeSpan ts)
        {
            var msg = new StringBuilder("please wait ");

            if (ts.Days > 0)
                msg.Append($"{ts.Days} day(s) ");

            if (ts.Hours > 0)
                msg.Append($"{ts.Hours} hour(s) ");

            if (ts.Minutes > 0)
                msg.Append($"{ts.Minutes} minute(s) ");

            return msg.ToString().Trim();
        }
        public static bool ToHasValue(this string? obj)
        {
            string val = obj.ToSafeString();
            return !string.IsNullOrEmpty(val);
        }
    }

    public static class DataTypeEnum
    {
        public enum BoolFormat
        {
            NONE,
            YESNO,
            TRUEFALSE,
            ZEROONE
        };
    }
}
