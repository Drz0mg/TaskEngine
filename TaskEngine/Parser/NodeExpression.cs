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
	public abstract class NodeExpression : ASTNode
	{
		protected NodeTask Task;

	    protected NodeExpression(NodeTask task)
		{
			this.Task = task;
		}

		public abstract Value GetValue();

		public abstract bool BindSymbols();
	}
}
