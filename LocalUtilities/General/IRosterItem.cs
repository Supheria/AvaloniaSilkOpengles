namespace LocalUtilities.General;

public interface IRosterItem<out TSignature>
    where TSignature : notnull
{
    public TSignature Signature { get; }
}
