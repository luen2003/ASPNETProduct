using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using BOSDLL.BOSL;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace ServiceAPI.Utils
{
    public class DateTimeParse
    {
        public static DateOnly ConvertLongToDate(long date)
        {
            var convertToDateTime = ConvertLongToDateTime(date);
            return DateOnly.FromDateTime(convertToDateTime);
        }

        public static long ConvertDateToLong(DateOnly date)
        {
            var dateString = date.ToDateTime(TimeOnly.MinValue).ToString(Constant.DateTimeFormat);
            return localsetting.DateTimeParse(ref dateString);
        }

        public static DateTime ConvertLongToDateTime(long date)
        {
            var dateTimeString = localsetting.DateTimeStr(date);
            var convertToDateTime = DateTime.ParseExact(dateTimeString, Constant.DateTimeFormatFromLocalSetting, CultureInfo.CurrentCulture);
            return convertToDateTime.AddSeconds(-convertToDateTime.Second);
        }

        public static long ConvertDateTimeToLong(DateTime dateTime)
        {
            var dateTimeString = dateTime.AddSeconds(-dateTime.Second).ToString(Constant.DateTimeFormat);
            return localsetting.DateTimeParse(ref dateTimeString);
        }

        public static long ConvertDatetimestringToLong(string dateTimestr)
        {
            try
            {
                var dateTime = DateTime.ParseExact(dateTimestr, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                return dateTime.ToFileTimeUtc();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string ConvertLongToStringDatetome(long date)
        {
            var dateTimeString = localsetting.DateTimeStr(date);
            var convertToDateTime = DateTime.ParseExact(dateTimeString, "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            return convertToDateTime.ToString();
        }

        public static string DateTimeStrVN_Viettel(object tmp)
        {
            if (RuntimeHelpers.GetObjectValue(tmp) is DBNull)
                return "";
            int integer1 = Conversions.ToInteger(Conversion.Int(Operators.DivideObject(tmp, (object)35942400L)));
            tmp = Operators.SubtractObject(tmp, (object)checked((long)integer1 * 35942400L));
            int integer2 = Conversions.ToInteger(Conversion.Int(Operators.DivideObject(tmp, (object)2764800)));
            tmp = Operators.SubtractObject(tmp, (object)checked(integer2 * 2764800));
            int integer3 = Conversions.ToInteger(Conversion.Int(Operators.DivideObject(tmp, (object)86400)));
            tmp = Operators.SubtractObject(tmp, (object)checked(integer3 * 86400));
            int integer4 = Conversions.ToInteger(Conversion.Int(Operators.DivideObject(tmp, (object)3600)));
            tmp = Operators.SubtractObject(tmp, (object)checked(integer4 * 3600));
            int integer5 = Conversions.ToInteger(Conversion.Int(Operators.DivideObject(tmp, (object)60)));
            int num = checked(integer1 + 1976);
            DateTime dateTime = DateTime.Parse(Conversions.ToString(num) + "-" + (integer2 < 10 ? "0" + Conversions.ToString(integer2) : Conversions.ToString(integer2)) + "-" + Conversions.ToString(integer3) + " " + (integer4 < 10 ? "0" + Conversions.ToString(integer4) : Conversions.ToString(integer4)) + ":" + Strings.Right("0" + Conversions.ToString(integer5), 2) + ":00");
            string date = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds().ToString();
            return date;
        }

        public static string DateTimeStrBOS(object longDate)
        {

            if (longDate == null) return "";

            long tmp = long.Parse(longDate.ToString());

            int y = (int)(tmp / 35942400);
            tmp -= y * 35942400;

            int m = (int)(tmp / 2764800);
            tmp -= m * 2764800;

            int d = (int)(tmp / 86400);
            tmp -= d * 86400;

            int h = (int)(tmp / 3600);
            tmp -= h * 3600;

            int n = (int)(tmp / 60);

            y += 1976;

            return (d < 10 ? "0" + d : "" + d) + "/" + (m < 10 ? "0" + m : "" + m) + "/" + y + " " + (h < 10 ? "0" + h : "" + h) + ":" + (n < 10 ? "0" + n : "" + n) + "";
        }
    }
}