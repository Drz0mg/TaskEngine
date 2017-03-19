/*
  This file is part of PPather.

    PPather is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    PPather is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with PPather.  If not, see <http://www.gnu.org/licenses/>.

*/

using System.Threading;
using TaskEngine.Helpers;
using TaskEngine.Tasks;
using ZzukBot.Constants;
using ZzukBot.ExtensionFramework;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace TaskEngine.Activities
{
    public class ActivityAttack : Activity
    {
        WoWUnit unit;
        public ActivityAttack(Task t, WoWUnit unit)
            : base(t, "Attack " + unit.Name)
        {
            this.unit = unit;
        }

        public override Location GetLocation()
        {
            return this.unit.Position;
        }

        public override bool Do()
        {
            if (BlackList.IsBlacklisted(this.unit))
            { 
                return true;
            }

            if (this.unit.Reaction.Equals(Enums.UnitReaction.Friendly) || this.unit.Health <= 0)
            {
                BlackList.Blacklist(this.unit);
                return true;
            }

            if (CustomClasses.Instance == null || CustomClasses.Instance.Current == null)
            {
                return true;
            }

            Logger.Log("We are doing combat!");


            if (ObjectManager.Instance.Player.TargetGuid.Equals(this.unit.Guid) == false &&
                ObjectManager.Instance.Player.IsInCombat.Equals(false))
            {
                ObjectManager.Instance.Player.Face(this.unit);
                ObjectManager.Instance.Player.SetTarget(this.unit);
                CustomClasses.Instance.Current.OnPull();
            }
            else
            {
                CustomClasses.Instance.Current.OnFight();
            }

            return this.unit.Health <= 0;
        }
    }
}
