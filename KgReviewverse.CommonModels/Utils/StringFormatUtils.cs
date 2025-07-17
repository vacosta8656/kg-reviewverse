namespace KgReviewverse.Common.Models.Utils;

public static class StringFormatUtils
{
    public static DateTime? ParseDateByIndex(string? raw, int index)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var matches = System.Text.RegularExpressions.Regex.Matches(raw, @"\d{4}-\d{2}-\d{2}");

        if (matches.Count > index && DateTime.TryParse(matches[index].Value, out var date))
            return date;

        return null;
    }

    public static string RemoveNumbersAndBrackets(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return System.Text.RegularExpressions.Regex.Replace(input, @"[\d\{\}\[\]\(\)]", "");
    }

    public static List<string> SplitCategoriesByUppercase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();

        var cleaned = RemoveNumbersAndBrackets(input);

        var words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        if (words.Length == 1)
        {
            result.AddRange(System.Text.RegularExpressions.Regex.Split(cleaned, @"(?<!^)(?=[A-Z])")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        }
        else
        {
            var temp = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                if (char.IsUpper(words[i][0]))
                {
                    result.Add(temp);
                    temp = words[i];
                }
                else
                {
                    temp += " " + words[i];
                }
            }
            result.Add(temp);

            result = result.SelectMany(r =>
                System.Text.RegularExpressions.Regex.Split(r, @"(?<!^)(?=[A-Z])")
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            ).ToList();
        }

        return result;
    }
}