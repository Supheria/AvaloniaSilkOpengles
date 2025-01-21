using System.Text;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Parser;

internal sealed class Tokenizer
{
    public Token Token => _token;
    readonly Token _token = new();
    ComposeState ComposeState { get; set; } = ComposeState.None;
    Word Composed { get; set; } = new();
    StringBuilder Composing { get; } = new();
    string Buffer { get; set; }
    int BufferPosition { get; set; }
    int Line { get; set; } = 1;
    int Column { get; set; } = 1;
    ParseTree Tree { get; } = new();
    bool Eof { get; set; }

    public Tokenizer(byte[] buffer, int offset, int count, Encoding encoding)
    {
        Buffer = encoding.GetString(buffer, offset, count);
    }

    public Tokenizer(string str)
    {
        Buffer = str;
    }

    public void NextToken()
    {
        var word = NextWord();
        while (!Tree.Parse(word, in _token))
        {
            word = NextWord();
        }
    }

    private Word NextWord()
    {
        while (!Compose()) { }
        return Composed;
    }

    private bool Compose()
    {
        if (!Composed.Submitted)
            return true;
        Eof = BufferPosition >= Buffer.Length;
        var ch = Eof ? SignTable.Close : Buffer[BufferPosition];
        switch (ComposeState)
        {
            case ComposeState.Quotation:
                return OnQuotaion(ch);
            case ComposeState.Escape:
                return OnEscape(ch);
            case ComposeState.Word:
                return OnWord(ch);
            case ComposeState.Note:
                return OnNote(ch);
            case ComposeState.None:
            default:
                return OnNone(ch);
        }
    }

    private bool OnNone(char ch)
    {
        switch (ch)
        {
            case SignTable.Quote:
                Composing.Clear();
                GetChar();
                ComposeState = ComposeState.Quotation;
                return false;
            case SignTable.Note:
                ComposeState = ComposeState.Note;
                GetChar();
                return false;
            case SignTable.Is:
            case SignTable.Open:
            case SignTable.Close:
                Composed = new(GetChar().ToString(), Line, Column, false);
                return true;
            case SignTable.Tab:
            case SignTable.Space:
            case SignTable.Return:
            case SignTable.NewLine:
            case SignTable.Empty:
                GetChar();
                return false;
            default:
                Composing.Clear();
                Composing.Append(GetChar());
                ComposeState = ComposeState.Word;
                return false;
        }
    }

    private bool OnNote(char ch)
    {
        switch (ch)
        {
            case SignTable.Return:
            case SignTable.NewLine:
            case SignTable.Empty:
                ComposeState = ComposeState.None;
                GetChar();
                return false;
            default:
                GetChar();
                return false;
        }
    }

    private bool OnWord(char ch)
    {
        switch (ch)
        {
            case SignTable.Tab:
            case SignTable.Space:
            case SignTable.Return:
            case SignTable.NewLine:
            case SignTable.Note:
            case SignTable.Is:
            case SignTable.Open:
            case SignTable.Close:
            case SignTable.Quote:
            case SignTable.Empty:
                Composed = new(Composing.ToString(), Line, Column, false);
                ComposeState = ComposeState.None;
                return true;
            default:
                Composing.Append(GetChar());
                return false;
        }
    }

    private bool OnEscape(char ch)
    {
        switch (ch)
        {
            case SignTable.Return:
            case SignTable.NewLine:
            case SignTable.Empty:
                Composed = new(Composing.ToString(), Line, Column, true);
                ComposeState = ComposeState.None;
                return true;
            default:
                Composing.Append(GetChar());
                ComposeState = ComposeState.Quotation;
                return false;
        }
    }

    private bool OnQuotaion(char ch)
    {
        switch (ch)
        {
            case SignTable.Escape:
                ComposeState = ComposeState.Escape;
                GetChar();
                return false;
            case SignTable.Quote:
                Composed = new(Composing.ToString(), Line, Column, true);
                ComposeState = ComposeState.None;
                GetChar();
                return true;
            default:
                Composing.Append(GetChar());
                return false;
        }
    }

    private char GetChar()
    {
        if (Eof)
            return SignTable.Close;
        var ch = Buffer[BufferPosition++];
        switch (ch)
        {
            case SignTable.NewLine:
                Line++;
                Column = 1;
                break;
            case SignTable.Tab:
                Column += 4;
                break;
            default:
                Column++;
                break;
        }
        return ch;
    }
}
