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
using System.Collections.Generic;
using TaskEngine.Activities;
using TaskEngine.Helpers;
using TaskEngine.Parser;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    public abstract class RunnerTask : ParserTask
    {
        public List<Location> Locations;
        private Location currentHotSpot = null;
        private ActivityWalkTo currentWalker = null;
        private bool useMount;
        private float howClose;

        public override void GetParams(List<string> l)
        {
            l.Add("Locations");
            l.Add("UseMount");
            l.Add("HowClose");
            base.GetParams(l);
        }

        protected RunnerTask(NodeTask node)
            : base(node)
        {
            this.Locations = new List<Location>();
            Value hs = node.GetValueOfId("Locations");
            List<Value> hsList = hs.GetCollectionValue();
            foreach (Value v in hsList)
            {
                Location l = v.GetLocationValue();
                this.Locations.Add(l);
            }
            this.useMount = node.GetBoolValueOfId("UseMount");
            var expression = node.GetExpressionOfId("HowClose");

            if (expression != null)
            {
                this.howClose = node.GetFloatValueOfId("HowClose");
            }
            else
            {
                this.howClose = 3.0f;
            }
        }

        public abstract Location GetNextLocation();

        public override Location GetLocation()
        {
            if (this.currentHotSpot == null)
            {
                this.currentHotSpot = this.GetNextLocation();
                Logger.Log("GetLoc need next . got " + this.currentHotSpot);
            }
            return this.currentHotSpot;
        }

        public override bool WantToDoSomething()
        {
            if (this.GetLocation() == null)
                return false;
            return true; // always want to run
        }

        public override Activity GetActivity()
        {
            if (this.currentHotSpot == null)
            {
                Logger.Log("GetAct need next");
                this.currentHotSpot = this.GetNextLocation();
            }
            if (this.currentWalker == null)
                this.currentWalker = new ActivityWalkTo(this, this.currentHotSpot, this.howClose, this.useMount);
            return this.currentWalker;
        }

        public override bool ActivityDone(Activity task)
        {
            Logger.Log("ActDone need next");
            this.currentHotSpot = this.GetNextLocation();
            this.currentWalker = null;
            task.Stop();
            return false;
        }
    }
}