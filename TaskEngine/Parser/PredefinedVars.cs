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
using System.Linq;
using ZzukBot.Constants;
using ZzukBot.Game.Statics;

namespace TaskEngine.Parser
{
    public delegate Value PredefinedVarDelegate();

	/// <summary>
	/// This class contains static methods that directly correspond to
	/// all available pre-defined variables in psc files. The name of the
	/// method will be the name of the variable. All method signatures
	/// must match PredefinedVarDelegate.
	/// </summary>
	internal static class PredefinedVars
	{
        
		#region One-Liners
		public static Value MyLevel()
		{
			return new Value(ObjectManager.Instance.Player.Level.ToString());
		}

		public static Value MyClass()
		{
			return new Value(ObjectManager.Instance.Player.Class.ToString());
		}

		public static Value MyRace()
		{
			return new Value(ObjectManager.Instance.Player.Race);
		}

		public static Value MyHealth()
		{
			return new Value(ObjectManager.Instance.Player.HealthPercent);
		}

		public static Value MyMana()
		{
			return new Value(ObjectManager.Instance.Player.ManaPercent);
		}

		public static Value MyX()
		{
            return new Value(ObjectManager.Instance.Player.Position.X.ToString(CultureInfo.InvariantCulture));
		}

		public static Value MyY()
		{
            return new Value(ObjectManager.Instance.Player.Position.Y.ToString(CultureInfo.InvariantCulture));
		}

		public static Value MyZ()
		{
            return new Value(ObjectManager.Instance.Player.Position.Z.ToString(CultureInfo.InvariantCulture));
		}

		public static Value MyTarget()
		{
		    if (ObjectManager.Instance.Target == null)
		    {
		        return new Value(new Dictionary<string, string>());
		    }

		    Dictionary<string, string> targInfo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		    {
		        {"Health", ObjectManager.Instance.Target.HealthPercent.ToString()},
		        {"HealthPoints", ObjectManager.Instance.Target.Health.ToString()},
		        {"HealthMax", ObjectManager.Instance.Target.MaxHealth.ToString()},
		        {"Mana", ObjectManager.Instance.Target.ManaPercent.ToString()},
		        {"ManaPoints", ObjectManager.Instance.Target.Mana.ToString()},
		        {"ManaMax", ObjectManager.Instance.Target.MaxMana.ToString()},
		        {"IsDead", (ObjectManager.Instance.Target.Health > 0).ToString()},
		        {"IsPlayer", ObjectManager.Instance.Target.IsPlayer.ToString()},
		        {"Reaction", ObjectManager.Instance.Target.Reaction.ToString()},
		        {"Name", ObjectManager.Instance.Target.Name},
		        {"Level", ObjectManager.Instance.Target.Level.ToString()}
		    };
            
		    return new Value(targInfo);
		}

		public static Value IsStealthed()
		{
			return new Value(ObjectManager.Instance.Player.IsStealth ? 1 : 0);
		}

		public static Value IsInCombat()
		{
			return new Value(ObjectManager.Instance.Player.IsInCombat ? 1 : 0);
		}

		public static Value MyEnergy()
		{
			return new Value(ObjectManager.Instance.Player.Energy.ToString());
		}

		public static Value MyZone()
		{
			return new Value(ObjectManager.Instance.Player.RealZoneText);
		}

		public static Value MySubZone()
		{
            return new Value(ObjectManager.Instance.Player.MinimapZoneText);
		}
		#endregion

        /*
		public static Value ItemCount()
		{
			Dictionary<String, int> items = Pather.Helpers.Inventory.CreateItemCount(false);
			return new Value(items);
		}
        */
		public static Value MyGearType()
		{
			string result;
		    {
		        switch (ObjectManager.Instance.Player.Class)
		        {
		            case Enums.ClassId.Warrior:
		            case Enums.ClassId.Paladin:
		                result = ObjectManager.Instance.Player.Level < 40 ? "mail" : "plate";
                            break;
		            case Enums.ClassId.Hunter:
                    case Enums.ClassId.Shaman:
                        result = ObjectManager.Instance.Player.Level < 40 ? "leather" : "mail";
		                break;
		            case Enums.ClassId.Priest:
		            case Enums.ClassId.Mage:
		            case Enums.ClassId.Warlock:
		                result = "cloth";
		                break;
		            case Enums.ClassId.Druid:
                    case Enums.ClassId.Rogue:
		                result = "leather";
                        break;
		            default:
		                throw new ArgumentOutOfRangeException();
		        }
		    }

		    return new Value(result);
		}

        /*
		public static Value FreeBagSlots()
		{
			int count = 0;
			int totalslots = 0;
			long[] AllBags = MyPlayer.Me.Bags;

			for (int bagNr = 0; bagNr < 5; bagNr++)
			{
				long[] Contents;
				int SlotCount;

				if (bagNr == 0)
				{
					Contents = MyPlayer.Me.BagContents;
					SlotCount = MyPlayer.Me.SlotCount;
				}
				else
				{
					GContainer bag = (GContainer)GObjectList.FindObject(AllBags[bagNr - 1]);
					if (bag != null)
					{
                        // The slots in quivers and ammo pouches shouldn't count.
                        // Apparently, there's no way to determine what kind of container
                        // we're dealing with so I just added all the containers in the game
                        // that hold arrows or ammo as of 2.4.2.
                        if (bag.ItemDefID != 34106 && bag.ItemDefID != 34099 &&
                            bag.ItemDefID != 29118 && bag.ItemDefID != 19320 && bag.ItemDefID != 8218 &&
                            bag.ItemDefID != 2663 && bag.ItemDefID != 7372 && bag.ItemDefID != 3604 &&
                            bag.ItemDefID != 3574 && bag.ItemDefID != 11363 && bag.ItemDefID != 5441 &&
                            bag.ItemDefID != 7279 && bag.ItemDefID != 2102 && bag.ItemDefID != 34105 &&
                            bag.ItemDefID != 34100 && bag.ItemDefID != 18714 && bag.ItemDefID != 29143 &&
                            bag.ItemDefID != 29144 && bag.ItemDefID != 19319 && bag.ItemDefID != 8217 &&
                            bag.ItemDefID != 2662 && bag.ItemDefID != 7371 && bag.ItemDefID != 3605 &&
                            bag.ItemDefID != 11362 && bag.ItemDefID != 3573 && bag.ItemDefID != 5439 &&
                            bag.ItemDefID != 7278 && bag.ItemDefID != 2101)
                        {
                            SlotCount = bag.SlotCount;
                            Contents = bag.BagContents;
                        }
                        else
                        {
                            SlotCount = 0;
                            Contents = null;
                        }
    				}
					else
					{
						SlotCount = 0;
						Contents = null;
					}
				}

				totalslots += SlotCount;
				for (int i = 0; i < SlotCount; i++)
				{
					if (Contents[i] == 0)
						count++;
				}
			}

			return new Value(count);
		}

		public static Value AlreadyTrained()
		{
			bool ret = false;

			int TrainLevel = 0;
			string TrainLevelS = PPather.ToonData.Get("TrainLevel");
			if (TrainLevelS != null && TrainLevelS != "")
				TrainLevel = Int32.Parse(TrainLevelS);
			int mylevel = MyPlayer.Me.Level;

			if (TrainLevel == mylevel)
				ret = true;

			return new Value(ret ? 1 : 0);
		}

		public static Value MyDurability()
		{
			float worst = 1.0f;

			GItem[] items = GObjectList.GetEquippedItems();

			foreach (GItem item in items)
			{
				if (item.DurabilityMax > 0)
				{
					float dur = (float)item.Durability;
					if (dur < worst)
						worst = dur;
				}
			}

			return new Value(worst);
		}

		public static Value MyGear()
		{
			Dictionary<String, int> dic = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

			GItem[] items = GObjectList.GetEquippedItems();

			foreach (GItem item in items)
			{
				dic.Add(item.Name, 1);
			}
			return new Value(dic);
		}

		public static Value MyBagNames()
		{
			Dictionary<String, int> dic = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

			long[] AllBags = MyPlayer.Me.Bags;
			for (int bag = 1; bag <= 4; bag++)
			{
				GContainer container = (GContainer)GObjectList.FindObject(AllBags[bag - 1]);
				if (container != null)
				{
					string ItemName = container.Name;
					int OldCount = 0;
					dic.TryGetValue(ItemName, out OldCount);
					dic.Remove(ItemName);
					dic.Add(ItemName, OldCount + 1);
				}
			}
			return new Value(dic);
		}
        */
	}
}
