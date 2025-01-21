using System.Text;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Formatter;

internal static class SsFormatter
{
    static Dictionary<char, QuoteType> QuoteTypes { get; } =
        new()
        {
            [SignTable.Escape] = QuoteType.Escaped,
            [SignTable.Quote] = QuoteType.Escaped,
            [SignTable.Tab] = QuoteType.Normal,
            [SignTable.Space] = QuoteType.Normal,
            [SignTable.Note] = QuoteType.Normal,
            [SignTable.Is] = QuoteType.Normal,
            [SignTable.Open] = QuoteType.Normal,
            [SignTable.Close] = QuoteType.Normal,
            [SignTable.Return] = QuoteType.Normal,
            [SignTable.NewLine] = QuoteType.Normal,
            [SignTable.Empty] = QuoteType.Normal,
        };

    private static string ToQuoted(string str)
    {
        if (str is "")
            return new StringBuilder().Append(SignTable.Quote).Append(SignTable.Quote).ToString();
        var sb = new StringBuilder();
        var useQuote = false;
        foreach (var ch in str)
        {
            if (QuoteTypes.TryGetValue(ch, out var quoteType))
            {
                switch (quoteType)
                {
                    case QuoteType.Escaped:
                        sb.Append(SignTable.Escape).Append(ch);
                        useQuote = true;
                        break;
                    case QuoteType.Normal:
                        useQuote = true;
                        sb.Append(ch);
                        break;
                }
            }
            else
                sb.Append(ch);
        }
        if (useQuote)
            return new StringBuilder()
                .Append(SignTable.Quote)
                .Append(sb)
                .Append(SignTable.Quote)
                .ToString();
        return sb.ToString();
    }

    private static StringBuilder AppendNewLine(
        this StringBuilder sb,
        bool writeIntoMultiLines,
        bool needSplitter
    )
    {
        if (writeIntoMultiLines)
            return sb.AppendLine();
        return needSplitter ? sb.Append(SignTable.Space) : sb;
    }

    private static StringBuilder AppendTab(
        this StringBuilder sb,
        int level,
        bool writeIntoMultiLines
    )
    {
        if (!writeIntoMultiLines)
            return sb;
        for (var i = 0; i < level; i++)
            sb.Append(SignTable.Space).Append(SignTable.Space);
        return sb;
    }

    public static string GetName(int level, string name, bool writeIntoMultiLines, bool leaveSpace)
    {
        return new StringBuilder()
            .AppendNewLine(writeIntoMultiLines, leaveSpace)
            .AppendTab(level, writeIntoMultiLines)
            .Append(ToQuoted(name))
            .ToString();
    }

    public static string GetStart(int level, bool writeIntoMultiLines, bool appendToName)
    {
        var sb = new StringBuilder();
        if (!appendToName)
            sb.AppendNewLine(writeIntoMultiLines, false).AppendTab(level, writeIntoMultiLines);
        return sb.Append(SignTable.Open).ToString();
    }

    public static string GetEnd(int level, bool writeIntoMultiLines)
    {
        return new StringBuilder()
            .AppendNewLine(writeIntoMultiLines, false)
            .AppendTab(level, writeIntoMultiLines)
            .Append(SignTable.Close)
            .ToString();
    }

    public static string GetValue(
        int level,
        string value,
        bool writeIntoMultiLines,
        bool appendToName,
        bool needSplitter
    )
    {
        var sb = new StringBuilder();
        if (appendToName)
            sb.Append(SignTable.Is);
        else
            sb.AppendNewLine(writeIntoMultiLines, needSplitter)
                .AppendTab(level, writeIntoMultiLines);
        return sb.Append(ToQuoted(value)).ToString();
    }
}
