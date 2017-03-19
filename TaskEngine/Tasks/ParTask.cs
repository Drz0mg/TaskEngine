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
using ZzukBot.Game.Statics;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    public class ParTask : ParserTask
    {
        public const string ParserKeyword = "Par,Parallel";
        private readonly SortedList<int, List<Task>> orderedChildTasks = new SortedList<int, List<Task>>();
        bool done = false;

        public ParTask(NodeTask node)
            : base(node)
        {
            foreach (NodeTask nt in node.SubTasks)
            {
                Task t = TaskCreator.CreateTaskFromNode(nt, this);
                if (t != null)
                {
                    int prio = nt.GetPrio();
                    List<Task> l;
                    if (this.orderedChildTasks.TryGetValue(prio, out l))
                    {
                        l.Add(t);
                    }
                    else
                    {
                        l = new List<Task>();
                        l.Add(t);
                        this.orderedChildTasks.Add(prio, l);
                    }
                    //PPather.WriteLine("Par add prio " + prio + " task " + t);

                }
            }
        }


        public override bool IsFinished()
        {
            if (this.done)
                return true;
            foreach (List<Task> l in this.orderedChildTasks.Values)
            {
                foreach (Task t in l)
                {
                    if (!t.IsFinished())
                        return false;
                }
            }
            this.Unload();
            this.done = true;
            return true;
        }

        public override void GetParams(List<string> l)
        {
            base.GetParams(l);
        }


        public override Task[] GetChildren()
        {
            List<Task> ts = new List<Task>();
            foreach (int prio in this.orderedChildTasks.Keys)
            {
                List<Task> l;
                this.orderedChildTasks.TryGetValue(prio, out l);
                foreach (Task t in l)
                {
                    ts.Add(t);
                }
            }
            return ts.ToArray();
        }


        public override void Restart()
        {
            foreach (List<Task> l in this.orderedChildTasks.Values)
            {
                foreach (Task t in l)
                {
                    t.Restart();
                }
            }
            this.done = false;

        }

        private Task GetBestTask()
        {
            Location meLoc = ObjectManager.Instance.Player.Position;
            Task bestFound = null;
            float bestFoundDistance = 1E30f;
            foreach (int prio in this.orderedChildTasks.Keys)
            {
                List<Task> l;
                this.orderedChildTasks.TryGetValue(prio, out l);
                foreach (Task t in l)
                {
                    //PPather.WriteLine("Consider " + t);
                    // PPather.WriteLine("  f: "  + t.IsFinished() + " wtd " + t.WantToDoSomething());
                    if (!t.IsFinished() && t.WantToDoSomething())
                    {
                        float d = 0;
                        Location loc = t.GetLocation();
                        if (loc != null)
                            d = loc.GetDistanceTo(meLoc);
                        if (d < bestFoundDistance)
                        {
                            bestFound = t;
                            bestFoundDistance = d;
                        }
                    }
                }
                if (bestFound != null)
                {
                    return bestFound; // Found one
                }
            }

            return null;
        }

        public override Location GetLocation()
        {
            return this.GetBestTask().GetLocation();
        }

        public override bool WantToDoSomething()
        {
            Task bestFound = this.GetBestTask();
            return bestFound != null;
        }

        public override Activity GetActivity()
        {
            Task bestFound = this.GetBestTask();
            //PPather.WriteLine("par beast task: " + bestFound);
            Activity a = bestFound?.GetActivity();
            //PPather.WriteLine("par beast a: " + a);
            return a;
        }

        public override bool ActivityDone(Activity task)
        {
            Task bestFound = this.GetBestTask();
            bestFound?.ActivityDone(task);
            return false; // I am never done
        }
    }
}
