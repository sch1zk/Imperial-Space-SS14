using Content.Shared.MachineLinking;

namespace Content.Client.MachineLinking
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedSignalSwitchComponent))]
    [Access(typeof(SignalSwitchSystem))]
    public sealed class SignalSwitchComponent : SharedSignalSwitchComponent
    {
    }
}
