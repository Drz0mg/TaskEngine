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

namespace TaskEngine.Parser
{
	public abstract class BoolBinExpression : BinExpresssion
	{
	    protected BoolBinExpression(NodeTask task, NodeExpression a0, NodeExpression a1)
			: base(task, a0, a1)
		{
		}

		public override Value GetValue()
		{
			Value val0 = this.Left.GetValue();
			Value val1 = this.Right.GetValue();

			if (val0 == null || val1 == null)
				return null;

			bool c0 = val0.GetBoolValue();
			bool c1 = val1.GetBoolValue();

			bool res = this.BoolOp(c0, c1);

			return res ? Value.TrueValue : Value.FalseValue;
		}

		public abstract bool BoolOp(bool a, bool b);
	}
}
