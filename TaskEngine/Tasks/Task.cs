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
using ZzukBot.Objects;

namespace TaskEngine.Tasks
{
    /*
	 * Design notes:    
	 * 
	 *  GetLocation
	 *  WantToDoSomething
	 *  IsFinished
	 *  
	 *  GetActivity
	 *  ActivityDone
	 */

    public abstract class Task
    {
        public enum TaskState
        {
            Idle,
            Want,
            Active,
            Done
        };

        public bool IsActive { get; set; }

        public Task Parent { get; set; }

        public virtual bool IsParserTask()
        {
            return false;
        }
        public virtual TaskState State
        {
            get
            {
                if (this.IsActive)
                    return TaskState.Active;
                if (this.IsFinished())
                    return TaskState.Done;
                return TaskState.Idle;
            }
        }
        
        public abstract Location GetLocation();

        public virtual void Restart()
        {

        }

        public virtual Task[] GetChildren()
        {
            return null;
        }

        public abstract bool WantToDoSomething();
        public abstract bool IsFinished();

        public abstract Activity GetActivity();
        public abstract bool ActivityDone(Activity task); // called when activity is done

        public virtual void GetParams(List<string> l)
        {
        }

    }
}
