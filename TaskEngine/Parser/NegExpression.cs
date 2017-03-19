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
using System.Globalization;

namespace TaskEngine.Parser
{
	public class NegExpression : NodeExpression
	{
	    private readonly NodeExpression e;

		public NegExpression(NodeTask task, NodeExpression e)
			: base(task)
		{
			this.e = e;
		}

		public override Value GetValue()
		{
			Value v = this.e.GetValue();

			if (v.IsInt())
			{
				int i = -v.GetIntValue();
				v = new Value(i.ToString());
			}
			else if (v.IsFloat())
			{
				float f = -v.GetFloatValue();
				v = new Value(f.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				this.Error("Negating non numerical value " + v);
			}

			return v;
		}

		public override void Dump(int d)
		{
			Console.Write("-");
			this.e.Dump(d);
		}

		public override bool BindSymbols()
		{
			return this.e.BindSymbols();
		}
	}
}
