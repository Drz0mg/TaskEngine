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
	public class CollectionExpression : NodeExpression
	{
	    private readonly List<NodeExpression> expressions;

		public CollectionExpression(NodeTask task, List<NodeExpression> expressions)
			: base(task)
		{
			this.expressions = expressions;
		}

		public override Value GetValue()
		{
			List<Value> vals = new List<Value>();

			foreach (NodeExpression e in this.expressions)
			{
				vals.Add(e.GetValue());
			}

			return new Value(vals);
		}

		public override void Dump(int d)
		{
			Console.Write("[");

			foreach (NodeExpression e in this.expressions)
			{
				e.Dump(d);
				Console.Write(", ");
			}

			Console.Write("]");
		}

		public override bool BindSymbols()
		{
			bool ok = true;

			foreach (NodeExpression e in this.expressions)
			{
				ok &= e.BindSymbols();
			}

			return ok;
		}
	}
}
