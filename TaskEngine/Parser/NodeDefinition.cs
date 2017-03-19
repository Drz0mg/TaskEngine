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
using TaskEngine.Helpers;

namespace TaskEngine.Parser
{
	public class NodeDefinition : ASTNode
	{
		private NodeTask task;
	    private readonly string name;
	    private readonly NodeExpression expression;

		public NodeDefinition(NodeTask task, string name, NodeExpression expression)
		{
			this.task = task;
			this.name = name;
			this.expression = expression;
		}

		public bool IsNamed(string s)
		{
			return String.CompareOrdinal(s, this.name) == 0;
		}

		public NodeExpression GetExpression()
		{
			return this.expression;
		}

		public override void Dump(int d)
		{
			Console.Write(this.Prefix(d) + "$" + this.name + " = ");
			this.expression.Dump(d);
			Logger.Log(";");
		}
	}
}
