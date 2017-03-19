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
	delegate Value FcallDelegate(params Value[] args);

	/// <summary>
	/// This class contains static methods that directly correspond to
	/// the available Functions in a psc file. The name of the
	/// method will be the name of the available function. All method 
	/// signatures must match FcallDelegate.
	/// </summary>
	static class Fcalls
	{
        //public static Value QuestStatus(params Value[] args)
        //{
        //    String q = args[0].GetStringValue();
        //    String status = PPather.GetQuestStatus(q);
        //    if (status == null)
        //        return Pather.Parser.Value.NilValue;
        //    return new Value(status);
        //}

        //public static Value BGQueued(params Value[] args)
        //{
        //    String bf = args[0].GetStringValue();
        //    if (bf.Length > 0)
        //    {
        //        MiniMapBattlefieldFrameState bfState = Tasks.BGQueueTaskManager.GetQueueState(bf);
        //        bool isQueued = false;
        //        if (bfState == MiniMapBattlefieldFrameState.Queue ||
        //            bfState == MiniMapBattlefieldFrameState.CanEnter ||
        //            bfState == MiniMapBattlefieldFrameState.Inside)
        //        {
        //            isQueued = true;
        //        }
        //        return new Value(isQueued ? 1 : 0);
        //    }
        //    else
        //    {
        //        PPather.WriteLine("*** Warning - BGQueued() called without a battleground name!");
        //        PPather.WriteLine("*** Did you mean to use $BGQueued?");
        //    }
        //    return null;
        //}

        //public static Value NearTo(params Value[] args)
        //{
        //    Location cloc = args[0].GetLocationValue();
        //    if (cloc == null)
        //        return new Value(0);
        //    float howClose = args[1].GetFloatValue();
        //    Location mloc = new Location(MyPlayer.Me.Location);
        //    float d = mloc.GetDistanceTo(cloc);
        //    return new Value((d <= howClose) ? 1 : 0);
        //}

        //public static Value GetState(params Value[] args)
        //{
        //    String key = args[0].GetStringValue();
        //    if (key != "" && key != null)
        //    {
        //        String state = PPather.GetToonState(key);
        //        if (state != null && state != "") return new Value(state);
        //    }
        //    return new Value(-1);
        //}

        //public static Value QuestExists(params Value[] args)
        //{
        //    string quest = args[0].GetStringValue();
        //    if (quest != "" && quest != null)
        //    {
        //        string val = PPather.ToonData.Get("Quest:" + quest);
        //        if (val != null && val != "") return new Value(1);
        //    }
        //    return new Value(0);
        //}

        //public static Value HaveBuff(params Value[] args)
        //{
        //    string cbuff = args[0].GetStringValue();
        //    if (cbuff != "")
        //    {
        //        Helpers.Buff Buffs = new Helpers.Buff();
        //        if (Buffs.HaveBuff(cbuff))
        //            return new Value(1);
        //    }
        //    return new Value(0);
        //}

        //public static Value QuestMobKillCount(params Value[] args)
        //{
        //    int questID = args[0].GetIntValue();
        //    string mob = args[1].GetStringValue();

        //    if (!string.IsNullOrEmpty(mob))
        //        return new Value(PPather.adventureLog.GetQuestKillCount(questID, mob));
            
        //    return new Value(0);
        //}
	}
}