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
	public class LiteralExpression : NodeExpression
	{
	    private readonly Value val;

		public LiteralExpression(NodeTask task, string val)
			: base(task)
		{
			this.val = new Value(val);
		}

		public LiteralExpression(NodeTask task, Value val)
			: base(task)
		{
			this.val = val;
		}

		public override Value GetValue()
		{
			return this.val;
		}

		public override void Dump(int d)
		{
			Console.Write(this.val);
		}

		public override bool BindSymbols()
		{
			return true;
		}
	}
}
