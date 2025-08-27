using System.Runtime.CompilerServices;

namespace IdentityServer.Converters;
public static class StringConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string value) => int.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToLong(this string value) => long.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(this string value) => float.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string value) => double.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToAyDateTime(this string value, int addDay = 0) => localsetting.DateTimeParse(ref value) + addDay * 86400;
}
