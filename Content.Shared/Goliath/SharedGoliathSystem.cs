using System.Linq;
using Content.Shared.Goliath;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Goliath;
public abstract class SharedGoliathSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedCanSummonTentacleComponent, ComponentStartup>(OnTentacleStartup);
    }

    private void OnTentacleStartup(EntityUid uid, SharedCanSummonTentacleComponent component, ComponentStartup args)
    {
        var tentacleAction = new WorldTargetAction(_proto.Index<WorldTargetActionPrototype>(component.TentacleActionName));
        _action.AddAction(uid, tentacleAction, null);
    }

}

public sealed class GoliathTentacleActionEvent : WorldTargetActionEvent { }
