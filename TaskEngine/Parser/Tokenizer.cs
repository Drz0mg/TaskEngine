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
	class Tokenizer
	{
	    private readonly System.IO.TextReader reader;

        public int Line { get; set; }

        public Tokenizer(System.IO.TextReader reader)
		{
			this.reader = reader;
			this.Line = 1;
		}

		Token current = null;

		public Token Peek()
		{
		    return this.current ?? (this.current = this.Next());
		}

		public Token Next()
		{
			if (this.current != null)
			{
				Token t = this.current;
				this.current = null;
				return t;
			}

			Token r;

			do
			{
				r = new Token(this.reader);
				if (r.type == Token.Type.Newline)
					this.Line++;
			} while (r.type == Token.Type.Newline);

			return r;
		}
	}
}
