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

using System;
using System.Linq;
using TaskEngine.Tasks;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace TaskEngine.Activities
{
    public class ActivityWalkTo : Activity
    {
        private Location to;
        private float howClose;
        private bool useMount;

        public ActivityWalkTo(Task t, Location to, float howClose, bool useMount)
            : base(t, "Walk to " + to)
        {
            this.to = to;
            this.howClose = howClose;
            this.useMount = useMount;
        }

        public override Location GetLocation()
        {
            return this.to;
        }

        public override void Start()
        {
        }

        public override bool Do()
        {
            Location meLocation = ObjectManager.Instance.Player.Position;
            float distanceToDestination = meLocation.GetDistanceTo(this.to);
            /*float mountRange = PPather.PatherSettings.MountRange; //TODO MOUNT RANGE

			// mount if it's far enough away
			if (distanceToDestination >= mountRange)
			{
				if (PPather.PatherSettings.UseMount != "Never Mount")
				{
					if (PPather.PatherSettings.UseMount == "Always Mount" ||
						(PPather.PatherSettings.UseMount == "Let Task Decide" &&
						UseMount == true))
					{
						Helpers.Mount.MountUp();
					}
				}
			}
            */

            if (distanceToDestination < this.howClose && Math.Abs(meLocation.Z - this.to.Z) < this.howClose)
            {
                // we're here so dismount, if we weren't mounted it doesn't matter
                //Helpers.Mount.Dismount();
                ObjectManager.Instance.Player.CtmStopMovement();
                return true;
            }

            var path = Navigation.Instance.CalculatePath(ObjectManager.Instance.Player.MapId,
                ObjectManager.Instance.Player.Position, this.to, true);

            if (path.Length > 1)
            {
                ObjectManager.Instance.Player.CtmTo(path[1]);
                return false; // done, can't do more
            }

            return true;
        }

        public override void Stop()
        {
           ObjectManager.Instance.Player.CtmStopMovement();
        }
    }
}
