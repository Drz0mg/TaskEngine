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
	class Token
	{
		public enum Type
		{
			ID,
			Literal,
			Newline,
			Keyword,
			EOF
		};
		public Type type;
		public string val;

		public Token(string s)
		{
			if (s[0] == '$')
			{
				this.type = Type.ID;
				this.val = s.Substring(1);
			}
			else
			{
				this.type = Type.Literal;
				this.val = s;
			}
		}

		private bool IsIDChar(char c)
		{
			if ((c >= 'a' && c <= 'z') ||
			   (c >= 'A' && c <= 'Z') ||
			   (c >= '0' && c <= '9') ||
			   c == '.' ||
			   c == '_'
			   )
				return true;
			return false;
		}

		public Token(System.IO.TextReader reader)
		{
			this.type = Type.EOF;

			try
			{
				bool done = false;
				bool foundNonWhite = false;

				do
				{
					int c = reader.Peek();

					if (c == -1)
						break;

					if (!foundNonWhite && Char.IsWhiteSpace((char)c))
					{
						reader.Read();

						if ((char)c == '\n')
						{
							this.type = Type.Newline;
							return;
						}
					}
					else
					{
						foundNonWhite = true;

						if (c == '$')
						{
							reader.Read(); // chomp $
							this.readLiteral(reader);
							this.type = Type.ID;
							return;
						}

						if (c == '"')
						{
							this.readString(reader);
							return;
						}
						else if (this.IsKeyChar(c))
						{
							if (this.readKeyWord(reader))
							{
								// a comment, keep on reading
								foundNonWhite = false;
								this.type = Type.Newline;
							}

							return;
						}
						else
						{
							this.readLiteral(reader);
							this.type = Type.Literal;
							return;
						}
					}
				} while (!done);
			}
			catch (ObjectDisposedException)
			{
				this.type = Type.EOF;
			}
			catch (System.IO.IOException)
			{
				this.type = Type.EOF;
			}

			this.type = Type.EOF;
		}


		private bool IsKeyChar(int c)
		{
			if (c == '+')
				return true;
			if (c == '-')
				return true;
			if (c == '*')
				return true;
			if (c == '/')
				return true;
			if (c == '%')
				return true;
			if (c == '>')
				return true;
			if (c == '=')
				return true;
			if (c == '!')
				return true;
			if (c == '<')
				return true;
			if (c == '{')
				return true;
			if (c == '}')
				return true;
			if (c == ':')
				return true;
			if (c == ';')
				return true;
			if (c == '(')
				return true;
			if (c == ')')
				return true;
			if (c == ',')
				return true;
			if (c == ']')
				return true;
			if (c == '[')
				return true;
			if (c == '&')
				return true;
			if (c == '|')
				return true;
			if (c == '^')
				return true;
			return false;
		}

		// return true if it is a comment
		private bool readKeyWord(System.IO.TextReader reader)
		{
			int i = reader.Read();
			char c = (char)i;
			this.type = Type.Keyword;
			// some are 2 chars
			char c2 = (char)reader.Peek();

			if (c == '>' && c2 == '=')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '<' && c2 == '=')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '=' && c2 == '=')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '!' && c2 == '=')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '&' && c2 == '&')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '|' && c2 == '|')
			{
				reader.Read();
				this.val = "" + c + c2;
			}
			else if (c == '/' && c2 == '/')
			{
				reader.ReadLine();
				return true;
			}
			else
				this.val = "" + c;
			return false;
		}

		private void readString(System.IO.TextReader reader)
		{
			this.val = "";
			this.type = Type.Literal;
			int c = reader.Read(); // read "
			bool esc = false;
			do
			{
				c = reader.Read();
				if (c == '\\' && !esc)
				{
					esc = true;
				}
				else
				{
					this.val += (char)c;
					esc = false;
				}
				c = reader.Peek();
			} while (c != -1 && (c != '"' || esc == true));
			if (c == '"')
				reader.Read(); // read "
		}

		private void readLiteral(System.IO.TextReader reader)
		{
			this.val = "";
			this.type = Type.Literal;
			int c = -1;
			do
			{
				c = reader.Read();
				this.val += (char)c;
				c = reader.Peek();
			} while (this.IsIDChar((char)c) && c != -1);
		}
	}
}
