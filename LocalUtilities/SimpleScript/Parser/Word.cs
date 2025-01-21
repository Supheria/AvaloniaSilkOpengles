using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Parser;

internal sealed class Word
{
    public string Text { get; } = "";
    public bool Submitted { get; private set; }
    int Line { get; }
    int Column { get; }
    bool QuotedOrEscaped { get; }
    public static Word Eof { get; } = new(SignTable.Close.ToString());

    public Word()
    {
        Submitted = true;
    }

    private Word(string eof)
    {
        Text = eof;
    }

    public Word(string text, int line, int column, bool quotedOrEscaped)
    {
        Text = text;
        Line = line;
        Column = column;
        QuotedOrEscaped = quotedOrEscaped;
    }

    public char Head()
    {
        if (Text.Length is not 1 || QuotedOrEscaped)
            return '\0';
        return Text[0];
    }

    public void Submit()
    {
        Submitted = true;
    }

    public override string ToString()
    {
        return $"\"{Text}\" at Line({Line}), Column({Column})";
    }
}
