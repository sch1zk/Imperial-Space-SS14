using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.StationEvents;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Economy.Wage
{
    public sealed class WageSchedulerSystem : GameRuleSystem<WageSchedulerComponent>
    {
        [Dependency] private readonly WageManagerSystem _wageManagerSystem = default!;
        private const float MinimumTimeUntilFirstWage = 900;
        private const float WageInterval = 1800;
        [ViewVariables(VVAccess.ReadWrite)]
        private float _timeUntilNextWage = MinimumTimeUntilFirstWage;
        protected override void Started(EntityUid uid, WageSchedulerComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
        {
            base.Started(uid, component, gameRule, args);
        }
        protected override void Ended(EntityUid uid, WageSchedulerComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
        {
            base.Ended(uid, component, gameRule, args);
            _timeUntilNextWage = MinimumTimeUntilFirstWage;
        }
        protected override void ActiveTick(EntityUid uid, WageSchedulerComponent component, GameRuleComponent gameRule, float frameTime)
        {
            base.ActiveTick(uid, component, gameRule, frameTime);

            if (!_wageManagerSystem.WagesEnabled)
                return;
            if (_timeUntilNextWage > 0)
            {
                _timeUntilNextWage -= frameTime;
                return;
            }
            _wageManagerSystem.Payday();
            _timeUntilNextWage = WageInterval;
        }
    }
}
