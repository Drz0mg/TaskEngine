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
    public class ExprSub : ArithBinExpression
    {
        public ExprSub(NodeTask task, NodeExpression a0, NodeExpression a1)
            : base(task, a0, a1)
        {
        }

        public override float FloatOp(float a, float b)
        {
            return a - b;
        }

        public override int IntOp(int a, int b)
        {
            return a - b;
        }

        public override string OpName()
        {
            return "-";
        }
    }
}