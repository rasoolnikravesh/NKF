using System.Text;
using System.Text.RegularExpressions;

namespace NKF.Domain.SharedKernels.ValueObjects;

public static class Strings
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex ZwnjSpacingRegex = new(@"\s*‌\s*", RegexOptions.Compiled);

    public static string FixFormat(this string str, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        var normalizedValue = NormalizePersianArabicCharacters(str);
        normalizedValue = WhitespaceRegex.Replace(normalizedValue, " ").Trim();
        normalizedValue = ZwnjSpacingRegex.Replace(normalizedValue, "‌");
        return normalizedValue;
    }

    private static string NormalizePersianArabicCharacters(string value)
    {
        return value
            .Normalize(NormalizationForm.FormKC)
            .Replace('ي', 'ی')
            .Replace('ك', 'ک')
            .Replace("ـ", string.Empty);
    }
}