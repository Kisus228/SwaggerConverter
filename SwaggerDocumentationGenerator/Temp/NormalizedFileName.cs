namespace SwaggerDocumentationGenerator.Temp;

public sealed class NormalizedFileName
{
    public NormalizedFileName(string rawFileName)
        : this(rawFileName, Path.GetInvalidFileNameChars())
    {
    }

    public NormalizedFileName(string rawFileName, char[] invalidCharactersInFileName, string replaceWith = "_")
    {
        Name = rawFileName.Replace(invalidCharactersInFileName, replaceWith);
    }

    public string Name { get; }

    public static implicit operator string(NormalizedFileName source) =>
        source.Name;

    public override string ToString() =>
        Name;
}