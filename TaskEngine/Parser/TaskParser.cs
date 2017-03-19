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
using System.Collections.Generic;
using TaskEngine.Helpers;

namespace TaskEngine.Parser
{
	public class TaskParser
	{
		Tokenizer tn;

		public TaskParser(System.IO.TextReader reader)
		{
			this.tn = new Tokenizer(reader);
		}

		public void Error(string msg)
		{
			Logger.Log("Line "+this.tn.Line+": " +msg);
		}

		private bool Expect(Token.Type type, string val)
		{
			Token t = this.tn.Next();

			if (t.type != type || t.val != val)
			{
				this.Error("Expected " + val + " found " + t.val);
				return false;
			}

			return true;
		}

		private NodeExpression ParseExpressionP(NodeTask t)
		{
			Token next = this.tn.Peek();

			if (next.type == Token.Type.Literal)
			{
				// literal expression
				this.tn.Next();
				Token lpar = this.tn.Peek();

				if (lpar.type == Token.Type.Keyword && lpar.val == "(")
				{
					//fcall

					List<NodeExpression> exprs = new List<NodeExpression>();
					Token comma = null;

					do
					{
						this.tn.Next(); // eat ( or , 
						NodeExpression n = this.ParseExpression(t);
						exprs.Add(n);
						comma = this.tn.Peek();
					} while (comma.type == Token.Type.Keyword && comma.val == ",");

					this.Expect(Token.Type.Keyword, ")");

					NodeExpression e = new FcallExpression(t, next.val, exprs);
					return e;
				}
				else
				{
					NodeExpression e = new LiteralExpression(t, next.val);
					return e;
				}
			}
			else if (next.type == Token.Type.ID)
			{
				// ID expression
				this.tn.Next();

				Token lpar = this.tn.Peek();

				// assoc lookup
				if (lpar.type == Token.Type.Keyword && lpar.val == "{")
				{
					this.tn.Next(); // eat {
					NodeExpression id = new IDExpression(t, next.val);
					NodeExpression key = this.ParseExpression(t);
					this.Expect(Token.Type.Keyword, "}");
					NodeExpression e = new AssocReadExpression(t, id, key);
					return e;
				}
				else
				{
					NodeExpression e = new IDExpression(t, next.val);
					return e;
				}

			}
			else if (next.type == Token.Type.Keyword && next.val == "(")
			{
				// par expressions
				this.tn.Next();
				NodeExpression n = this.ParseExpression(t);
				this.Expect(Token.Type.Keyword, ")");
				return n;
			}
			else if (next.type == Token.Type.Keyword && next.val == "[")
			{
				// collection expressions

				List<NodeExpression> exprs = new List<NodeExpression>();
				Token comma = null;

				do
				{
					this.tn.Next();
					Token p = this.tn.Peek();
					if (p.type == Token.Type.Keyword && p.val == "]")
						break; // empty collection
					NodeExpression n = this.ParseExpression(t);
					exprs.Add(n);
					comma = this.tn.Peek();
				} while (comma.type == Token.Type.Keyword && comma.val == ",");

				this.Expect(Token.Type.Keyword, "]");
				CollectionExpression ce = new CollectionExpression(t, exprs);
				return ce;
			}
			else if (next.type == Token.Type.Keyword && next.val == "-")
			{
				// unary neg
				Token neg = this.tn.Next();
				NodeExpression P = this.ParseExpressionP(t);
				NodeExpression e = new NegExpression(t, P);
				return e;
			}
			else
			{
				this.Error("Currupt expression");
			}

			return null;
		}

		private NodeExpression ParseExpressionT(NodeTask t)
		{
			// this is the hard one
			NodeExpression o0 = this.ParseExpressionP(t);

			Token next = this.tn.Peek();

			while (next.type == Token.Type.Keyword &&
				(next.val == "*" || next.val == "/"))
			{
				Token op = this.tn.Next();
				NodeExpression o1 = this.ParseExpressionP(t);

				if (op.val == "*")
				{
					o0 = new ExprMul(t, o0, o1);
				}
				else if (op.val == "/")
				{
					o0 = new ExprDiv(t, o0, o1);
				}

				next = this.tn.Peek();
			}

			return o0;
		}

		private NodeExpression ParseExpressionE(NodeTask t)
		{
			// this is the hard one
			NodeExpression o0 = this.ParseExpressionT(t);

			Token next = this.tn.Peek();

			while (next.type == Token.Type.Keyword &&
				(next.val == "+" || next.val == "-" || next.val == "%"))
			{
				Token op = this.tn.Next();
				NodeExpression o1 = this.ParseExpressionT(t);

				if (op.val == "+")
				{
					o0 = new ExprAdd(t, o0, o1);
				}
				else if (op.val == "-")
				{
					o0 = new ExprSub(t, o0, o1);
				}
				else if (op.val == "%")
				{
					o0 = new ExprMod(t, o0, o1);
				}

				next = this.tn.Peek();
			}

			return o0;
		}

		private NodeExpression ParseExpressionC(NodeTask t)
		{
			NodeExpression o0 = this.ParseExpressionE(t);

			Token next = this.tn.Peek();

			while (next.type == Token.Type.Keyword &&
				(next.val == "<" || next.val == "<=" || next.val == "==" ||
				 next.val == ">=" || next.val == ">" || next.val == "!="))
			{
				Token op = this.tn.Next();
				NodeExpression o1 = this.ParseExpressionE(t);

				if (op.val == "<")
					o0 = new ExprCmpLt(t, o0, o1);
				else if (op.val == "<=")
					o0 = new ExprCmpLe(t, o0, o1);
				else if (op.val == "==")
					o0 = new ExprCmpEq(t, o0, o1);
				else if (op.val == ">=")
					o0 = new ExprCmpGe(t, o0, o1);
				else if (op.val == ">")
					o0 = new ExprCmpGt(t, o0, o1);
				else if (op.val == "!=")
					o0 = new ExprCmpNe(t, o0, o1);

				next = this.tn.Peek();
			}

			return o0;
		}

		private NodeExpression ParseExpressionBAnd(NodeTask t)
		{
			NodeExpression o0 = this.ParseExpressionC(t);

			Token next = this.tn.Peek();

			while (next.type == Token.Type.Keyword &&
				(next.val == "&&"))
			{
				Token op = this.tn.Next();
				NodeExpression o1 = this.ParseExpressionBOr(t);

				if (op.val == "&&")
					o0 = new ExprAnd(t, o0, o1);

				next = this.tn.Peek();
			}

			return o0;
		}

		private NodeExpression ParseExpressionBOr(NodeTask t)
		{
			NodeExpression o0 = this.ParseExpressionBAnd(t);

			Token next = this.tn.Peek();

			while (next.type == Token.Type.Keyword &&
				(next.val == "||"))
			{
				Token op = this.tn.Next();
				NodeExpression o1 = this.ParseExpressionBAnd(t);

				if (op.val == "||")
					o0 = new ExprOr(t, o0, o1);

				next = this.tn.Peek();
			}

			return o0;
		}

		private NodeExpression ParseExpression(NodeTask t)
		{
			// this is the hard one
			NodeExpression e = this.ParseExpressionBOr(t);

			Token next = this.tn.Peek();

			if (next.type == Token.Type.Keyword && next.val == ";")
			{
				// all fine
			}
			else
			{

			}

			return e;
		}

        public NodeTask ParseTask(NodeTask parent)
        {
            NodeTask t = new NodeTask(parent);
            Token r = this.tn.Peek();

            if (r.type == Token.Type.EOF)
                return null;

            String s_name = null;
            String s_type = null;

            // Type (or name)
            Token t1 = this.tn.Next();
            if (t1.type != Token.Type.Literal) // !!!
            {
                this.Error("Task must have a type or name");
                return null;
            }

            Token colon = this.tn.Peek();
            if (colon.type == Token.Type.Keyword && colon.val == ":")
            {
                this.tn.Next(); // eat colon
                Token t2 = this.tn.Next(); // type

                if (t2.type != Token.Type.Literal) // !!!
                {
                    this.Error("Expected task type after : in task definition");
                    return null;
                }

                s_name = t1.val;
                s_type = t2.val;
            }
            else
            {
                s_name = null;
                s_type = t1.val;
            }

            t.Type = s_type;
            t.Name = s_name;

            // {            

            if (!this.Expect(Token.Type.Keyword, "{"))
                return null;

            // definitions
            Token next = this.tn.Peek();
            while (next.type == Token.Type.ID)
            {
                Token ID = this.tn.Next(); // chomp ID

                if (!this.Expect(Token.Type.Keyword, "="))
                    return null;

                NodeExpression expr = this.ParseExpression(t);

                if (!this.Expect(Token.Type.Keyword, ";"))
                    return null;

                t.AddDefinition(new NodeDefinition(t, ID.val, expr));
                next = this.tn.Peek();
            }

            // child tasks
            next = this.tn.Peek();
            while (next.type == Token.Type.Literal)
            {
                NodeTask child = this.ParseTask(t);
                t.AddTask(child);
                next = this.tn.Peek();
            }

            // }

            if (!this.Expect(Token.Type.Keyword, "}"))
                return null;

            return t;
        }
	}
}
