using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace IdentityServer.Helpers;
public static class MethodHelper
{
    public static IEnumerable<T> GetByFilter<T>(IEnumerable<T> dataSource, Func<T, string> condition, string? filter)
    {
        Guard.ThrowIfArgumentNull(dataSource, nameof(dataSource));
        Guard.ThrowIfArgumentNull(condition, nameof(condition));

        if (filter == null)
        {
            return dataSource;
        }
        return dataSource.Where(item => condition(item)?.ToNormalize().Contains(filter.ToNormalize()) == true);
    }

    // Bỏ dấu
    public static string RemoveDiacritics(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string normalizedString = input.Normalize(NormalizationForm.FormD);
        StringBuilder result = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToNormalize(this string input) => input.RemoveDiacritics().ToLower();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> MakeDefault<T>(this List<T> list, T defaultItem)
    {
        list ??= new();
        if (!list.Any()) list.Add(defaultItem);
        return list;
    }
}
