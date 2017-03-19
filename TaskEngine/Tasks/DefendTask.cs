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

using System.Collections.Generic;
using System.Linq;
using TaskEngine.Activities;
using TaskEngine.Helpers;
using TaskEngine.Parser;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    // Fight back attackers
    public class DefendTask : ParserTask
    {
        WoWUnit unit = null;


        public DefendTask(NodeTask node)
            : base(node)
        {
        }

        public override Location GetLocation()
        {
            return null; // anywhere
        }

        public override void GetParams(List<string> l)
        {
            base.GetParams(l);
        }

        public override string ToString()
        {
            return "Defend";
        }

        public override bool IsFinished()
        {
            return false;
        }
        public override bool WantToDoSomething()
        {
            this.unit = null;
            this.unit =
                ObjectManager.Instance.Units.Where(u => u.TargetGuid.Equals(ObjectManager.Instance.Player.Guid))
                    .OrderBy(u => u.DistanceToPlayer)
                    .FirstOrDefault();//WowUnit.GetNearestAttacker(0);
            if (this.unit != null)
                if (BlackList.IsBlacklisted(this.unit))
                    this.unit = null;
            return this.unit != null;
        }

        public override Activity GetActivity()
        {
            Activity attackTask = new ActivityAttack(this, this.unit);

            return attackTask;
        }

        public override bool ActivityDone(Activity task)
        {
            task.Stop();

            return false; // never done
        }
    }
}
