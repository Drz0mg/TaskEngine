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
using System.Globalization;
using System.Text;
using ZzukBot.Objects;

namespace TaskEngine.Parser
{
	public class Value
	{
		public static Value NilValue = new Value("");
		public static Value FalseValue = new Value("false");
		public static Value TrueValue = new Value("true");
		public static Value ZeroValue = new Value("0");

		string value = null; // for scalars
		List<Value> values = null; // for collections
		Dictionary<String, Value> dic = null; // for associative arrays

		public Value(string val)
		{
			this.value = val;
		}

		public Value(int val)
		{
			this.value = val.ToString();
		}

		public Value(float val)
		{
			this.value = val.ToString();
		}

		public Value(List<Value> val)
		{
			this.values = val;
		}

		public Value(Dictionary<string, Value> val)
		{
			if (!Equals(val.Comparer, StringComparer.InvariantCultureIgnoreCase))
			{
				Dictionary<string, Value> tmp = new Dictionary<string, Value>(StringComparer.InvariantCultureIgnoreCase);

				foreach (string key in val.Keys)
				{
					tmp[key] = val[key];
				}

				val = tmp;
			}

			this.dic = val;
		}

		// handy utility function 
		public Value(Dictionary<String, int> val)
		{
			this.dic = new Dictionary<string, Value>(StringComparer.InvariantCultureIgnoreCase);

			foreach (string key in val.Keys)
			{
				int c = 0;
				val.TryGetValue(key, out c);
				Value v = new Value(c);
				this.SetAssocValue(key, v);
			}
		}

		public Value(Dictionary<string, string> val)
		{
			this.dic = new Dictionary<string, Value>(StringComparer.InvariantCultureIgnoreCase);

			foreach (string key in val.Keys)
			{
				string s = "";
				val.TryGetValue(key, out s);
				Value v = new Value(s);
				this.SetAssocValue(key, v);
			}
		}

		public bool GetBoolValue()
		{
			string s = this.GetStringValue();
			if (s == "false" || s == "False")
				return false;
			if (s == "0")
				return false;
			if (s == "")
				return false;
			return true;
		}

		public string GetStringValue()
		{
			if (this.value != null)
				return this.value;

			if (this.values != null)
			{
				StringBuilder sb = new StringBuilder("[");

				foreach (Value v in this.values)
				{
					sb.Append(v.GetStringValue());
					sb.Append(", ");
				}

				sb.Append("]");
				return sb.ToString();
			}

			if (this.dic != null)
			{
				StringBuilder sb = new StringBuilder("{");

				foreach (String s in this.dic.Keys)
				{
					Value val = this.dic[s];
					sb.Append(s);
					sb.Append(" => ");
					sb.Append(val);
					sb.Append(", ");
				}

				sb.Append("}");
				return sb.ToString();
			}

			return "";
		}

		public bool IsInt()
		{
			if (this == NilValue)
				return true;

			try
			{
				Int32.Parse(this.GetStringValue());
				return true;
			}
			catch
			{
			}

			return false;
		}

		public bool IsFloat()
		{
			if (this == NilValue)
				return true;

			try
			{
				Single.Parse(this.GetStringValue(), CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool IsCollection()
		{
			return this.values != null;
		}

		public bool IsAssocArray()
		{
			return this.dic != null;
		}

		public void SetAssocValue(string key, Value value)
		{
			if (this.dic == null)
				this.dic = new Dictionary<string, Value>(StringComparer.InvariantCultureIgnoreCase);

			this.value = null;
			this.values = null;

			this.dic.Remove(key);
			this.dic.Add(key, value);
		}

		public Value GetAssocValue(string key)
		{
			if (this.dic == null)
				return null;

			Value val = null;

			this.dic.TryGetValue(key, out val);

			if (val == null)
				return NilValue;

			return val;
		}

		public int GetIntValue()
		{
			if (this == NilValue)
				return 0;

			try
			{
				return Int32.Parse(this.GetStringValue());
			}
			catch
			{
			}

			return 0;
		}

		public float GetFloatValue()
		{
			if (this == NilValue)
				return 0;

			try
			{
                //TODO: return Int32.Parse(GetStringValue(), CultureInfo.InvariantCulture); SEEMS TO FUCK WITH OMEGA??????
				return Single.Parse(this.GetStringValue());
			}
			catch
			{
			}

			return 0f;
		}

		public List<string> GetStringCollectionValues()
		{
			List<string> vals = new List<string>();

			if (this.values == null)
			{
				if (this.value != null && this != NilValue)
					vals.Add(this.GetStringValue()); // not a collection 
			}
			else
			{
				foreach (Value v in this.values)
				{
					vals.Add(v.GetStringValue());
				}
			}

			return vals;
		}

		public List<int> GetIntCollectionValues()
		{
			List<int> vals = new List<int>();

			if (this.values == null)
			{
				if (this.value != null && this != NilValue)
					vals.Add(this.GetIntValue()); // not a collection 
			}
			else
			{
				foreach (Value v in this.values)
				{
					vals.Add(v.GetIntValue());
				}
			}

			return vals;
		}


		// this has to be a collection of floats with at least 3 items
		public Location GetLocationValue()
		{
			if (this.values == null)
				return null;

			float x = 0f, y = 0f, z = 0f;

			try
			{
				x = this.values[0].GetFloatValue();
				y = this.values[1].GetFloatValue();
				z = this.values[2].GetFloatValue();
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					"Current value doesn't seem to be a valid Location", e);
			}

			return new Location(x, y, z);
		}

        /*
		public Pather.Tasks.BuySet GetBuySetValue()
		{
			if (values == null)
				return null;
			string item = "", minAmount = "", buyAmount = "";
			try
			{
				item = values[0].GetStringValue();
				minAmount = values[1].GetStringValue();
				buyAmount = values[2].GetStringValue();
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					"Current value doesn't seem to be a valid buy set", e);
			}
			return new Pather.Tasks.BuySet(item, minAmount, buyAmount);
		}
        */
		public List<Value> GetCollectionValue()
		{
			if (this.values != null)
				return this.values;

			List<Value> c = new List<Value>();

			if (this.value != null)
				c.Add(new Value(this.value));

			return c;
		}

		public override string ToString()
		{
			return this.GetStringValue();
		}
	}
}
