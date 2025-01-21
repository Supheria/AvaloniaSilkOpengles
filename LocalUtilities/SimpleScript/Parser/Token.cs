namespace LocalUtilities.SimpleScript.Parser;

internal sealed class Token
{
    public string Text { get; private set; } = "";
    public TokenType Type { get; private set; }

    public void SetValue(string text)
    {
        Text = text;
        Type = TokenType.Value;
    }

    public void SetHead()
    {
        Type = TokenType.Head;
    }

    public void SetTail()
    {
        Type = TokenType.Tail;
    }

    public override string ToString()
    {
        return $"{Type}: {Text}";
    }
}
