using Content.Shared.Access.Systems;
using Content.Shared.PDA;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components
{
    [RegisterComponent, NetworkedComponent]
    [Access(typeof(SharedIdCardSystem), typeof(SharedPDASystem), typeof(SharedAgentIdCardSystem))]
    public sealed class IdCardComponent : Component
    {
        [DataField("fullName")]
        [Access(typeof(SharedIdCardSystem), typeof(SharedPDASystem), typeof(SharedAgentIdCardSystem),
            Other = AccessPermissions.ReadWrite)] // FIXME Friends
        public string? FullName;

        [DataField("jobTitle")]
        public string? JobTitle;

        // Imperial Space Start
        [DataField("storedBankAccountNumber")]
        public string? StoredBankAccountNumber;

        [DataField("storedBankAccountPin")]
        public string? StoredBankAccountPin;
        // Imperial Space End
    }

    [Serializable, NetSerializable]
    public sealed class IdCardComponentState : ComponentState
    {
        public string? FullName;
        public string? JobTitle;

        public IdCardComponentState(string? fullName, string? jobTitle)
        {
            FullName = fullName;
            JobTitle = jobTitle;
        }
    }
}
