﻿/*
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
using TaskEngine.Activities;
using TaskEngine.Parser;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    class LoadTask : ActivityFreeTask
    {
        bool done = false;
        public string File;

        public LoadTask(NodeTask node)
            : base(node)
        {
            this.File = node.GetValueOfId("File").GetStringValue();
        }

        public override void GetParams(List<string> l)
        {
            l.Add("File");
            base.GetParams(l);
        }

        public override string ToString()
        {
            return "Load";
        }

        public override Location GetLocation()
        {
            return null;
        }

        public override void Restart()
        {
            this.done = false;
        }
        public override bool IsFinished()
        {
            return this.done;
        }

        public override bool WantToDoSomething()
        {
            return !this.done;
        }

        public override bool DoActivity()
        {
            // Do the stuff here
            return true; // done
        }

        public override bool ActivityDone(Activity task)
        {
            this.done = true;
            return true;
        }
    }
}