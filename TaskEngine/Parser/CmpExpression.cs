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
	public abstract class CmpExpression : BinExpresssion
	{
	    protected CmpExpression(NodeTask task, NodeExpression a0, NodeExpression a1)
			: base(task, a0, a1)
		{
		}

		public override Value GetValue()
		{
			Value val0 = this.Left.GetValue();
			Value val1 = this.Right.GetValue();

			if (val0 == null || val1 == null)
				return Value.FalseValue;

			if (val0.IsInt() && val1.IsInt())
			{
				int c0 = val0.GetIntValue();
				int c1 = val1.GetIntValue();
				bool v = this.IntOp(c0, c1);

				return v ? Value.TrueValue : Value.FalseValue;
			}

			if ((val0.IsFloat() && val1.IsFloat()) ||
				(val0.IsInt() && val1.IsFloat()) ||
				(val0.IsFloat() && val1.IsInt()))
			{
				float c0 = val0.GetFloatValue();
				float c1 = val1.GetFloatValue();
				bool v = this.FloatOp(c0, c1);

				return v ? Value.TrueValue : Value.FalseValue;
			}

			bool sv = this.StringOp(val0.GetStringValue(), val1.GetStringValue());

			return sv ? Value.TrueValue : Value.FalseValue;
		}

		public abstract bool FloatOp(float a, float b);
		public abstract bool IntOp(int a, int b);
		public abstract bool StringOp(string a, string b);
	}
}
