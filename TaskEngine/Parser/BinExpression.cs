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
	public abstract class BinExpresssion : NodeExpression
	{
		protected NodeExpression Left;
		protected NodeExpression Right;

	    protected BinExpresssion(NodeTask task, NodeExpression left, NodeExpression right)
			: base(task)
		{
			this.Left = left;
			this.Right = right;
		}

		public abstract string OpName();

		public override void Dump(int d)
		{
			Console.Write("(");
		    this.Left.Dump(d);
			Console.Write(this.OpName());
		    this.Right.Dump(d);
			Console.Write(")");
		}

		public override bool BindSymbols()
		{
			return this.Left.BindSymbols() && this.Right.BindSymbols();
		}
	}
}
