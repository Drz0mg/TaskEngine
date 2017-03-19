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
using System.Reflection;
using TaskEngine.Helpers;

namespace TaskEngine.Parser
{
	public class RootNode : NodeTask
	{
		#region Static stuff
		// need empty method so we can call it to ensure the
		// static constructor is called early
		public static void Init()
		{
		}

		static Dictionary<string, FcallDelegate> fcallMap = new Dictionary<string, FcallDelegate>(StringComparer.InvariantCultureIgnoreCase);
		static Dictionary<string, PredefinedVarDelegate> predefvarMap = new Dictionary<string, PredefinedVarDelegate>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// The static constructor initializes mappings between strings representing
		/// function/variable names within a psc file and the associated C# method
		/// to get the corresponding value.
		/// 
		/// Since this is achieved via reflection, new Functions/variables can be
		/// added simply by adding a corresponding method to Fcalls or PredefinedVars
		/// with no further modification required.
		/// </summary>
		static RootNode()
		{
			BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

			// init fcallMap
			Type delegateType = typeof(FcallDelegate);
			string s = "";

			try
			{
				foreach (MethodInfo mi in typeof(Fcalls).GetMethods(flags))
				{
					s += mi.Name + ", ";
					fcallMap[mi.Name] = (FcallDelegate)Delegate.CreateDelegate(delegateType, mi, true);
				}

				//System.Windows.Forms.MessageBox.Show(s);
			}
			catch (Exception e)
			{
				Logger.Log(s + "\n\n" + e.GetType().FullName + "\n" + e.Message);
			}


			// init predefvarMap
			delegateType = typeof(PredefinedVarDelegate);
			s = "";

			try
			{
				foreach (MethodInfo mi in typeof(PredefinedVars).GetMethods(flags))
				{
					s += mi.Name + ", ";
					predefvarMap[mi.Name] = (PredefinedVarDelegate)Delegate.CreateDelegate(delegateType, mi, true);
				}

				//System.Windows.Forms.MessageBox.Show(s);
			}
			catch (Exception e)
			{
				Logger.Log(s + "\n\n" + e.GetType().FullName + "\n" + e.Message);
			}
		}

		#endregion

		public RootNode()
			: base(null)
		{
			this.Type = "Oneshot";
		}

		public override Value GetValueOfFcall(string def, List<Value> parms)
		{
			Logger.Log("fcall " + def + "(" + parms + ")");

			FcallDelegate fn = null; // the function we're going to call

			fcallMap.TryGetValue(def, out fn);

			if (null != fn)
			{
				return fn(parms.ToArray());
			}

			return null;
		}

        public override Value GetValueOfId(string def)
        {
            PredefinedVarDelegate fn = null;
            predefvarMap.TryGetValue(def, out fn);
            if (null != fn)
            {
                return fn();
            }
            //PatherMessageBox.Show(def);
            return new Value(DefaultSettings.GetConfigString(def));
        }

		public override NodeExpression GetExpressionOfId(string def)
		{
			return null; // dynamic value, not defined my expression
		}
	}
}
