using Content.Shared.Economy.Eftpos;

namespace Content.Client.Economy.Eftpos
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedEftposComponent))]
    [Access(typeof(EftposSystem))]
    public sealed class EftposComponent : SharedEftposComponent
    {
    }
}
