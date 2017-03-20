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
using TaskEngine.Parser;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    // Run along hotspots
    public class HotspotTask : RunnerTask
    {
        public const string ParserKeyword = "Hotspots";
        private int currentHotSpotIndex;
        private readonly string order;
        private static Lazy<Random> random = new Lazy<Random>(() => new Random());
        
        public HotspotTask(NodeTask node)
            : base(node)
        {
            // save us the hassle of case sensitive checking
            this.order = node.GetValueOfId("Order").GetStringValue();
            this.currentHotSpotIndex = -1;
            if (this.order == "reverse")
                this.currentHotSpotIndex = this.Locations.Count;
        }

        public override void GetParams(List<string> l)
        {
            l.Add("Order");
            base.GetParams(l);
        }

        public override bool IsFinished()
        {
            return false;
        }

        public override string ToString()
        {
            return "Moving to hotspot";
        }

        public override Location GetNextLocation()
        {
            if (this.Locations.Count == 1)
                return this.Locations[0];
            if (this.order.Equals("order", StringComparison.InvariantCultureIgnoreCase))
            {
                this.currentHotSpotIndex++;
                if (this.currentHotSpotIndex >= this.Locations.Count)
                    this.currentHotSpotIndex = 0;
            }
            else if (this.order.Equals("reverse", StringComparison.InvariantCultureIgnoreCase))
            {
                this.currentHotSpotIndex--;
                if (this.currentHotSpotIndex < 0)
                    this.currentHotSpotIndex = this.Locations.Count - 1;
            }
            else
            {
                this.currentHotSpotIndex = random.Value.Next(this.Locations.Count);
            }

            return this.Locations[this.currentHotSpotIndex];
        }
    }
}