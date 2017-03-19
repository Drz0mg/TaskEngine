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
using TaskEngine.Activities;
using TaskEngine.Helpers;
using TaskEngine.Parser;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    // 
    public class IfTask : ParserTask
    {
        private readonly Task childTask;

        public IfTask(NodeTask node)
            : base(node)
        {
            this.childTask = TaskCreator.CreateTaskFromNode(node.SubTasks[0], this);
        }

        public override void GetParams(List<string> l)
        {
            l.Add("cond");
            base.GetParams(l);
        }

        public override bool IsFinished()
        {
            bool want = this.Nodetask.GetBoolValueOfId("cond");
            if (!want) return true;
            return this.childTask.IsFinished();
        }

        public override Location GetLocation()
        {
            return this.childTask.GetLocation();
        }

        public override Task[] GetChildren()
        {
            return new[] { this.childTask };
        }


        public override void Restart()
        {
            this.childTask.Restart(); // restart my baby
        }

        public override bool WantToDoSomething()
        {
            bool want;

            want = this.Nodetask.GetBoolValueOfId("cond");
            if (want)
            {
                want = this.childTask.WantToDoSomething();
            }

            return want;
        }

        public override Activity GetActivity()
        {
            return this.childTask.GetActivity();
        }

        public override bool ActivityDone(Activity task)
        {
            bool childDone = this.childTask.ActivityDone(task);
            if (childDone)
            {
                //  hmm, my child is done... cool
            }
            return childDone;
        }
    }
}
