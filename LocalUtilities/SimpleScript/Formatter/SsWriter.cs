namespace LocalUtilities.SimpleScript.Formatter;

internal abstract class SsWriter(bool writeIntoMultiLines)
{
    bool FirstLine { get; set; } = true;
    int Level { get; set; }
    bool AppendToName { get; set; }
    bool Subsequent { get; set; }
    bool WriteIntoMultiLines { get; } = writeIntoMultiLines;
    public bool ValueArray { get; set; }

    protected abstract void WriteString(string str);

    public void AppendName(string name)
    {
        var str = SsFormatter.GetName(Level, name, WriteIntoMultiLines, Subsequent);
        WriteString(str);
        AppendToName = true;
        Subsequent = true;
        ValueArray = false;
    }

    public void AppendStart()
    {
        var str = SsFormatter.GetStart(Level++, WriteIntoMultiLines, AppendToName || FirstLine);
        WriteString(str);
        AppendToName = false;
        Subsequent = false;
        FirstLine = false;
    }

    public void AppendEnd()
    {
        var str = SsFormatter.GetEnd(--Level, WriteIntoMultiLines);
        WriteString(str);
        ValueArray = false;
        Subsequent = false;
    }

    public void AppendValue(string value)
    {
        var str = SsFormatter.GetValue(Level, value, WriteIntoMultiLines, AppendToName, Subsequent);
        AppendToName = false;
        Subsequent = true;
        WriteString(str);
    }

    public void AppendUnquotedValue(string value)
    {
        WriteString(value);
    }
}
