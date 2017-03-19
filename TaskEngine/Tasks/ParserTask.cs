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
using System.Reflection;
using TaskEngine.Helpers;
using TaskEngine.Parser;

namespace TaskEngine.Tasks
{
    public abstract class ParserTask : Task
    {
        // this stuff dynamically gets a task object using reflection
        // so that we don't have to hard code in the different task
        // types in PPather.CreateTaskFromNode()

        // public just for debug, SHOULD BE PRIVATE
        public static Dictionary<string, Type> registeredTasks = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        public static void RegisterTask(string taskName, Type classType)
        {
            registeredTasks[taskName] = classType;

            //		PPather.WriteLine("Registered task: " + taskName + " -> " + classType);
            //		MessageBox.Show("Registered task: " + taskName + " -> " + classType.FullName);
        }

        public static ParserTask GetTask(NodeTask node)
        {
            //		string s = "";

            //		foreach (string key in registeredTasks.Keys)
            //			s += key + ",";

            //		PPather.WriteLine("Types: " + s);

            if (!registeredTasks.ContainsKey(node.Type))
            {
                Logger.Log("No registered task for " + node.Type);
                return null;
            }

            Logger.Log("Attempting to instantiate " + registeredTasks[node.Type]);
            // (In some big task files, the above line can crash Glider)

            ConstructorInfo ci = null;
            Object o = null;

            try
            {
                ci = registeredTasks[node.Type].GetConstructor(new[] { node.GetType() });
                o = ci.Invoke(new object[] { node });
            }
            catch (Exception e)
            {
                // an exception here is fatal, popup a dialog
                // to give some feedback and rethrow the error
                string s = "Error instantiating \"" + node.Type + "\" task\n\n" + e.GetType().FullName + "\n" + e;
                if (e.InnerException != null)
                {
                    s += "\n\nInner exception:" + e.InnerException.GetType().FullName + "\n" + e.InnerException.Message;
                }

                Logger.Log(s);
            }

            if (null == o)
            {
                Logger.Log("Created null");
            }

            return (ParserTask)o;
        }


        public NodeTask Nodetask { get; }

        protected ParserTask(NodeTask nodetask)
        {
            this.Nodetask = nodetask;
        }


        public override bool IsParserTask()
        {
            return true;
        }

        public virtual void Unload()
        {

            /*
			if (this.IsFinished() && !this.isActive)
			{
				lock (this)
				{
					PPather.WriteLine("Unloading Task: " + this.nodetask.name);
					this.nodetask.parent.subTasks.Remove(this.nodetask);
					PPather.WriteLine("Unloaded Task: " + this.nodetask.name);
				}
			}
			 */
        }
    }
}
