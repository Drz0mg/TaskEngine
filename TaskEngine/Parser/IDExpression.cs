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

namespace TaskEngine.Parser
{
	public class IDExpression : NodeExpression
	{
	    private readonly string id;
		private NodeExpression expression;

		public IDExpression(NodeTask task, string id)
			: base(task)
		{
			this.id = id;
		}

		public override Value GetValue()
		{
		    // know where it is
		    this.expression?.GetValue();

		    return this.Task.GetValueOfId(this.id);
		}

		public override void Dump(int d)
		{
			Console.Write("$" + this.id);
		}

		public override bool BindSymbols()
		{
			this.expression = this.Task.GetExpressionOfId(this.id);
			return this.expression != null;
		}
	}
}
