namespace LocalUtilities.SimpleScript.Parser;

internal enum ComposeState : byte
{
    None,
    Quotation,
    Escape,
    Word,
    Note,
}
