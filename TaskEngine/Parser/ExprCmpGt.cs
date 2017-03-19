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
    public class ExprCmpGt : CmpExpression
    {
        public ExprCmpGt(NodeTask task, NodeExpression a0, NodeExpression a1)
            : base(task, a0, a1)
        {
        }
        public override bool FloatOp(float a, float b)
        {
            return a > b;
        }
        public override bool IntOp(int a, int b)
        {
            return a > b;
        }
        public override bool StringOp(string a, string b)
        {
            return String.Compare(a, b, StringComparison.Ordinal) > 0;
        }
        public override string OpName()
        {
            return ">";
        }
    }
}