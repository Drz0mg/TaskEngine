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
using TaskEngine.Helpers;

namespace TaskEngine.Parser
{
	public class NodeTask : ASTNode
	{
	    public NodeTask Parent { get; set; }
	    public string Type { get; set; }
	    public string Name { get; set; }

	    private readonly List<NodeDefinition> definitions = new List<NodeDefinition>();
	    private readonly List<FuncDefinition> funcDefinitions = new List<FuncDefinition>();
	    public List<NodeTask> SubTasks { get; } = new List<NodeTask>();

	    public NodeTask(NodeTask parent)
		{
			this.Parent = parent;
		}

		public void AddDefinition(NodeDefinition def)
		{
			this.definitions.Add(def);
		}

		public void AddTask(NodeTask task)
		{
		    if (task != null)
		    {
                this.SubTasks.Add(task);
                task.Parent = this;
            }
		}

		public int GetPrio()
		{
			Value val = this.GetValueOfId("Prio");

			if (!val.IsInt())
				this.Error("$Prio has bad value " + val);

			return val.GetIntValue();
		}

		public void AddFunction(FuncDefinition fd)
		{
			this.funcDefinitions.Add(fd);
		}

		public virtual Value GetValueOfFcall(string def, List<Value> parms)
		{
			int dotIndex = def.IndexOf('.');

			if (dotIndex != -1)
			{
				// dot in name, search in named chilren tasks for it
				String cname = def.Substring(0, dotIndex);
				String cdef = def.Substring(dotIndex + 1);

				foreach (NodeTask t in this.SubTasks)
				{
					if (t.Name != null && t.Name == cname)
					{
						Value v = t.GetValueOfFcall(cdef, parms);
						return v;
					}
				}
			}
			else
			{
				foreach (FuncDefinition d in this.funcDefinitions)
				{
					if (d.IsNamed(def))
					{
						Value val = d.Call(parms);
						return val;
					}
				}
				if (this.Parent != null)
					return this.Parent.GetValueOfFcall(def, parms);
			}

			this.Error("Undefined function " + def);
			return null;
		}

		public bool GetBoolValueOfId(string id)
		{
			Value v = this.GetValueOfId(id);
			return v.GetBoolValue();
		}

		public float GetFloatValueOfId(string id)
		{
			Value v = this.GetValueOfId(id);
			return v.GetFloatValue();
		}

		public int GetIntValueOfId(string id)
		{
			Value v = this.GetValueOfId(id);
			return v.GetIntValue();
		}

		public void SetValueOfId(string def, Value val)
		{
			NodeDefinition n = null;

			foreach (NodeDefinition d in this.definitions)
			{
				if (d.IsNamed(def))
					n = d;
			}

			if (n != null)
				this.definitions.Remove(n);

			n = new NodeDefinition(this, def, new LiteralExpression(this, val));
			this.definitions.Add(n);

		}

		public virtual Value GetValueOfId(string def)
		{
			int dotIndex = def.IndexOf('.');

			if (dotIndex != -1)
			{
				// dot in name, search in named chilren tasks for it
				String cname = def.Substring(0, dotIndex);
				String cdef = def.Substring(dotIndex + 1);

				foreach (NodeTask t in this.SubTasks)
				{
					if (t.Name != null && t.Name == cname)
					{
						Value v = t.GetValueOfId(cdef);
						return v;
					}
				}
			}
			else
			{
				foreach (NodeDefinition d in this.definitions)
				{
					if (d.IsNamed(def))
					{
						NodeExpression e = d.GetExpression();
						Value val = e.GetValue();

						//  PPather.WriteLine(" def: " + def + " got e " + e + " and val " + val);

						return val;
					}
				}

				if (this.Parent != null)
				{
					Value e = this.Parent.GetValueOfId(def);
					return e;
				}
			}

			this.Error("No definition of idenfifier " + def);
			return null;
		}

		public virtual NodeExpression GetExpressionOfId(string def)
		{
			/*int dotIndex = def.IndexOf('.'); 

			if (dotIndex != -1)
			{
				// dot in name, search in named chilren tasks for it
				String cname = def.Substring(0, dotIndex);
				String cdef  = def.Substring(dotIndex + 1);
				foreach (
					NodeTask t in subTasks)
				{
					if (t.name != null && t.name == cname)
						return t.GetExpressionOfId(def); 
				}
			}
			else*/
			{
				foreach (NodeDefinition d in this.definitions)
				{
					if (d.IsNamed(def))
					{
						return d.GetExpression();
					}
				}
			}

			if (this.Parent != null)
			{
				NodeExpression e = this.Parent.GetExpressionOfId(def);
				return e;
			}

			this.Error("No definition of idenfifier " + def);
			return null;
		}

		public override void Dump(int d)
		{
			Logger.Log(this.Prefix(d) + this.Type);
			Logger.Log(this.Prefix(d) + "{");

			foreach (NodeDefinition nd in this.definitions)
			{
				nd.Dump(d + 1);
			}

			foreach (NodeTask task in this.SubTasks)
			{
				task.Dump(d + 1);
			}

			Logger.Log(this.Prefix(d) + "}");
		}

		public bool BindSymbols()
		{
			bool ok = true;

			// Traverse the tree and connect ID expression to their expression
			foreach (NodeDefinition d in this.definitions)
			{
				NodeExpression e = d.GetExpression();
				ok &= e.BindSymbols();
			}

			foreach (NodeTask t in this.SubTasks)
			{
				ok &= t.BindSymbols();
			}

			return ok;
		}
	}
}
