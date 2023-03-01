using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;


namespace Content.Shared.Goliath;

[RegisterComponent]
[NetworkedComponent]
[Access(typeof(SharedGoliathSystem))]
// не забыть создать Goliath систему
public sealed class SharedCanSummonTentacleComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("tentaclePrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    // не забыть создать tentaclePrototype
    public string TentaclePrototype = "Tentacle";
    // не забыть создать entiry Tentacle с ID Tentacle

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("tentacleActionName")]
    public string TentacleActionName = "GoliathTentacle";
}
