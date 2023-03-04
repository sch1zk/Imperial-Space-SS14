using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Shared.Goliath;

[RegisterComponent]
[NetworkedComponent]
[Access(typeof(SharedTentacleSystem))]
// не забыть создать Goliath систему
public sealed class SharedTentacleComponent : Component
{

}
