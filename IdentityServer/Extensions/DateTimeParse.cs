using System.Globalization;
using IdentityServer.Utils;

namespace IdentityServer.Extensions;

public static class DateTimeParse
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
        var convertToDateTime = DateTime.ParseExact(dateTimeString, (string)Constant.DateTimeFormatFromLocalSetting, CultureInfo.CurrentCulture);
        return convertToDateTime.AddSeconds(- convertToDateTime.Second);
    }

    public static long ConvertDateTimeToLong(DateTime dateTime)
    {
        var dateTimeString = dateTime.AddSeconds(- dateTime.Second).ToString(Constant.DateTimeFormat);
        return localsetting.DateTimeParse(ref dateTimeString);
    }
}