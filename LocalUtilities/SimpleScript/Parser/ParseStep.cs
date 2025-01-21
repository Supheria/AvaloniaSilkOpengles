namespace LocalUtilities.SimpleScript.Parser;

internal enum ParseStep : byte
{
    Start,
    Finish,
    Name,
    SubOpen,
    SubClose,
    Value,
}
