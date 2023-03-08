using Content.Server.Actions;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Atmos;
using Robust.Server.GameObjects;
using Robust.Shared.Player;

namespace Content.Server.TickKing
{
    public sealed class TickKingSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popup = default!;
        [Dependency] private readonly ActionsSystem _action = default!;
        [Dependency] private readonly AtmosphereSystem _atmos = default!;
        [Dependency] private readonly TransformSystem _xform = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<TickKingComponent, ComponentStartup>(OnStartup);

            SubscribeLocalEvent<TickKingComponent, TickKingRaiseArmyActionEvent>(OnRaiseArmy);
            SubscribeLocalEvent<TickKingComponent, TickKingDomainActionEvent>(OnDomain);
        }

        private void OnStartup(EntityUid uid, TickKingComponent component, ComponentStartup args)
        {
            _action.AddAction(uid, component.ActionRaiseArmy, null);
            _action.AddAction(uid, component.ActionDomain, null);
        }

        /// <summary>
        /// Summons an allied rat servant at the King, costing a small amount of hunger
        /// </summary>
        private void OnRaiseArmy(EntityUid uid, TickKingComponent component, TickKingRaiseArmyActionEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            //make sure the hunger doesn't go into the negatives
            if (hunger.CurrentHunger < component.HungerPerArmyUse)
            {
                _popup.PopupEntity(Loc.GetString("rat-king-too-hungry"), uid, uid);
                return;
            }
            args.Handled = true;
            hunger.CurrentHunger -= component.HungerPerArmyUse;
            Spawn(component.ArmyMobSpawnId, Transform(uid).Coordinates); //spawn the little mouse boi
        }

        /// <summary>
        /// uses hunger to release a specific amount of miasma into the air. This heals the rat king
        /// and his servants through a specific metabolism.
        /// </summary>
        private void OnDomain(EntityUid uid, TickKingComponent component, TickKingDomainActionEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<HungerComponent>(uid, out var hunger))
                return;

            //make sure the hunger doesn't go into the negatives
            if (hunger.CurrentHunger < component.HungerPerDomainUse)
            {
                _popup.PopupEntity(Loc.GetString("rat-king-too-hungry"), uid, uid);
                return;
            }
            args.Handled = true;
            hunger.CurrentHunger -= component.HungerPerDomainUse;

            _popup.PopupEntity(Loc.GetString("rat-king-domain-popup"), uid);

            var transform = Transform(uid);
            var indices = _xform.GetGridOrMapTilePosition(uid, transform);
            var tileMix = _atmos.GetTileMixture(transform.GridUid, transform.MapUid, indices, true);
            tileMix?.AdjustMoles(Gas.Miasma, component.MolesMiasmaPerDomain);
        }
    }

    public sealed class TickKingRaiseArmyActionEvent : InstantActionEvent { };
    public sealed class TickKingDomainActionEvent : InstantActionEvent { };
};
