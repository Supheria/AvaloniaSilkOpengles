namespace LocalUtilities.SimpleScript.Parser;

internal sealed class SsParseException(string message) : Exception(message)
{
    public static SsParseException UnexpectedMark(Word word, ParseStep step)
    {
        return new($"step on {step}: unexpected mark {word}");
    }

    public static SsParseException CannotOpenFile(string filePath)
    {
        return new($"cannot open file: \"{filePath}\"");
    }

    public static SsParseException OutRange()
    {
        return new($"out range of parse tree");
    }

    public static SsParseException PropertyNameMismatch(Type type, string propertyName)
    {
        return new($"property {propertyName} is mismatch to type {type.FullName}");
    }
}
