using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Parser;

internal sealed class ParseTree
{
    ParseStep Step { get; set; } = ParseStep.Start;

    public bool Parse(in Word word, in Token token)
    {
        var ch = word.Head();
        switch (Step)
        {
            case ParseStep.Start:
                return OnStart(word, token, ch);
            case ParseStep.Name:
                return OnName(word, token, ch);
            case ParseStep.SubOpen:
                return OnSubOpen(word, token, ch);
            case ParseStep.SubClose:
                return OnSubClose(word, token, ch);
            case ParseStep.Value:
                return OnValue(word, token, ch);
            case ParseStep.Finish:
            default:
                throw SsParseException.OutRange();
        }
    }

    private bool OnStart(Word word, Token token, char ch)
    {
        switch (ch)
        {
            case SignTable.Is:
                Step = ParseStep.Value;
                word.Submit();
                return false;
            case SignTable.Close:
                Step = ParseStep.SubClose;
                return false;
            case SignTable.Open:
                Step = ParseStep.SubOpen;
                word.Submit();
                token.SetHead();
                return true;
            default:
                Step = ParseStep.Name;
                word.Submit();
                token.SetValue(word.Text);
                return true;
        }
    }

    private bool OnName(Word word, Token token, char ch)
    {
        switch (ch)
        {
            case SignTable.Open:
                Step = ParseStep.SubOpen;
                word.Submit();
                token.SetHead();
                return true;
            case SignTable.Close:
                Step = ParseStep.SubClose;
                token.SetHead();
                return true;
            case SignTable.Is:
                Step = ParseStep.Value;
                word.Submit();
                return false;
            default:
                Step = ParseStep.Start;
                token.SetHead();
                return true;
        }
    }

    private bool OnSubOpen(Word word, Token token, char ch)
    {
        switch (ch)
        {
            case SignTable.Is:
                throw SsParseException.UnexpectedMark(word, Step);
            case SignTable.Close:
                Step = ParseStep.SubClose;
                return true;
            case SignTable.Open:
                word.Submit();
                token.SetHead();
                Step = ParseStep.Start;
                return true;
            default:
                Step = ParseStep.Name;
                word.Submit();
                token.SetValue(word.Text);
                return true;
        }
    }

    private bool OnSubClose(Word word, Token token, char ch)
    {
        switch (ch)
        {
            case SignTable.Open:
                Step = ParseStep.SubOpen;
                token.SetHead();
                return true;
            case SignTable.Close:
                Step = ParseStep.SubClose;
                word.Submit();
                token.SetTail();
                return true;
            default:
                Step = ParseStep.Start;
                token.SetHead();
                return true;
        }
    }

    private bool OnValue(Word word, Token token, char ch)
    {
        switch (ch)
        {
            case SignTable.Open:
                Step = ParseStep.SubOpen;
                word.Submit();
                token.SetHead();
                return true;
            case SignTable.Close:
            case SignTable.Is:
                throw SsParseException.UnexpectedMark(word, Step);
            default:
                Step = ParseStep.Name;
                word.Submit();
                token.SetValue(word.Text);
                return true;
        }
    }

    public override string ToString()
    {
        return $"{Step}";
    }
}
