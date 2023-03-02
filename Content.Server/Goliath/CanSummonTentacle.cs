using System.Linq;
using Content.Server.Popups;
using Content.Shared.Goliath;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Goliath;

public sealed class GoliathSystem : SharedGoliathSystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedCanSummonTentacleComponent, GoliathTentacleActionEvent>(OnTentacleTriggerSpawn);
    }

    public void OnTentacleTriggerSpawn(EntityUid uid, SharedCanSummonTentacleComponent component, GoliathTentacleActionEvent args)
    {
        if (args.Handled)
            return;

        var transform = Transform(uid);

        if (transform.GridUid == null)
        {
            _popup.PopupEntity(Loc.GetString("summon-tentacle-action-nogrid"), args.Performer, args.Performer);
            return;
        }

        var coords = transform.Coordinates;

        Spawn(component.TentaclePrototype, args.Target);
//                                         тут было coords
        args.Handled = true;
    }




}
