using Content.Server.GameTicking.Rules;
using Content.Server.StationEvents;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server.Economy.Wage
{
    public sealed class WageSchedulerSystem : GameRuleSystem
    {
        public override string Prototype => "WageScheduler";
        [Dependency] private readonly WageManagerSystem _wageManagerSystem = default!;
        private const float MinimumTimeUntilFirstWage = 900;
        private const float WageInterval = 1800;
        [ViewVariables(VVAccess.ReadWrite)]
        private float _timeUntilNextWage = MinimumTimeUntilFirstWage;
        public override void Started() { }
        public override void Ended()
        {
            _timeUntilNextWage = MinimumTimeUntilFirstWage;
        }
        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            if (!RuleStarted || !_wageManagerSystem.WagesEnabled)
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
