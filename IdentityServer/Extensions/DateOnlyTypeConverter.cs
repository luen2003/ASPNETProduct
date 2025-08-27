using System.ComponentModel;
using System.Globalization;
using IdentityServer.Utils;

namespace IdentityServer.Extensions;

public class DateOnlyTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            return DateOnly.Parse(str, CultureInfo.InvariantCulture);
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is DateOnly date)
        {
            return date.ToString(Constant.UtcDateFormat);
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}