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
using TaskEngine.Activities;
using TaskEngine.Parser;
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{

    public abstract class ActivityFreeTask : ParserTask
    {
        protected ActivityFreeTask(NodeTask node)
            : base(node)
        {
        }

        public override Location GetLocation()
        {
            return null;
        }

        public abstract bool DoActivity();

        public override Activity GetActivity()
        {
            return new SimpleActivity(this, this.ToString());
        }

        private class SimpleActivity : Activity
        {
            public SimpleActivity(Task task, String name)
                : base(task, name)
            {
            }

            public override Location GetLocation()
            {
                return null;
            }

            public override bool Do()
            {
                ActivityFreeTask aft = (ActivityFreeTask)this.Task;
                return aft.DoActivity();
            }

        }
    }
}
