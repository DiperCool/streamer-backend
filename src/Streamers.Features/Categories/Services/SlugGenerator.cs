using System.Text;
using System.Text.RegularExpressions;

namespace Streamers.Features.Categories.Services;

public interface ISlugGenerator
{
    string GenerateSlug(string title);
}

public class SlugGenerator : ISlugGenerator
{
    public string GenerateSlug(string phrase)
    {
        string str = phrase.ToLowerInvariant();

        str = RemoveDiacritics(str);

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        str = Regex.Replace(str, @"\s+", " ").Trim();

        str = Regex.Replace(str, @"\s", "-");

        return str;
    }

    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
