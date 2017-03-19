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
	public class AssocReadExpression : BinExpresssion
	{
		public AssocReadExpression(NodeTask task, NodeExpression a0, NodeExpression a1)
			: base(task, a0, a1)
		{
		}

		public override Value GetValue()
		{
			Value assoc = this.Left.GetValue();
			Value key = this.Right.GetValue();

			if (assoc == null || key == null)
				return null;

			String keyS = key.GetStringValue();
			Value val = assoc.GetAssocValue(keyS);

			return val;
		}

		public override string OpName()
		{
			return null;
		} // not used

		public override void Dump(int d)
		{
		    this.Left.Dump(d);
			Console.Write("{");
		    this.Right.Dump(d);
			Console.Write("}");
		}
	}
}
