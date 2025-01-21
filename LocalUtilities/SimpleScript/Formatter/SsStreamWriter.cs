using System.Text;

namespace LocalUtilities.SimpleScript.Formatter;

internal sealed class SsStreamWriter(Stream stream, bool writeIntoMultiLines, Encoding encoding)
    : SsWriter(writeIntoMultiLines),
        IDisposable
{
    Stream Stream { get; } = stream;
    Encoding Encoding { get; } = encoding;

    protected override void WriteString(string str)
    {
        var buffer = Encoding.GetBytes(str);
        Stream.Write(buffer);
    }

    public void Dispose()
    {
        Stream.Flush();
        Stream.Dispose();
    }
}
