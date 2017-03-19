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

namespace TaskEngine.Parser
{
	public class FcallExpression : NodeExpression
	{
		private readonly string id;
		private readonly List<NodeExpression> parms;

		public FcallExpression(NodeTask task, string id, List<NodeExpression> parms)
			: base(task)
		{
			this.id = id;
			this.parms = parms;
		}

		public override Value GetValue()
		{
			List<Value> vals = new List<Value>(this.parms.Count);

			foreach (NodeExpression e in this.parms)
			{
				Value v = e.GetValue();
				if (v == null)
					return null;
				vals.Add(v);
			}

			return this.Task.GetValueOfFcall(this.id, vals);
		}

		public override void Dump(int d)
		{
			Console.Write("call " + this.id + "(");

			foreach (NodeExpression e in this.parms)
			{
				e.Dump(d);
				Console.Write(", ");
			}

			Console.Write(")");
		}

		public override bool BindSymbols()
		{
			bool ok = true;

			foreach (NodeExpression e in this.parms)
			{
				ok &= e.BindSymbols();
			}

			// TODO check fcall name
			return ok;
		}
	}
}
