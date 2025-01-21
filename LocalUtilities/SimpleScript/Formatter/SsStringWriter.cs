using System.Text;

namespace LocalUtilities.SimpleScript.Formatter;

internal sealed class SsStringWriter(bool writeIntoMultiLines) : SsWriter(writeIntoMultiLines)
{
    StringBuilder Writer { get; } = new();

    protected override void WriteString(string str)
    {
        Writer.Append(str);
    }

    public override string ToString()
    {
        return Writer.ToString();
    }
}
