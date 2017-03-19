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
using System.IO;
using System.Text;
using TaskEngine.Data;
using TaskEngine.Helpers;
using ZzukBot.Game.Statics;

namespace TaskEngine.Parser
{

	public class Preprocessor
	{
		private readonly List<string> lines;
		public TextReader Reader;
		private readonly MemoryStream memStream;
		private readonly Stack<bool> conditions;

		public Preprocessor(TextReader reader)
		{
			this.lines = new List<string>();
			this.conditions = new Stack<bool>();
			this.memStream = new MemoryStream();
			this.conditions.Push(true);
			this.Reader = reader;

			this.process();
			this.writeStream();
		}

		private Preprocessor(string file, List<string> lines, Stack<bool> conditions)
		{
			string basePath = "H:\\Task.sc";
			basePath = basePath.Substring(0, basePath.LastIndexOf('\\') + 1);
            this.Reader = this.Reader = RemoteTask.GetFile(basePath + file);
			this.lines = lines;
			this.conditions = conditions;

			this.process();
		}

		public Stream ProcessedStream
		{
			get
			{
				return this.memStream;
			}
		}

		private void process()
		{
			string line = "";
			string arg = "";

			while (this.Reader.Peek() > 0)
			{
				line = this.Reader.ReadLine().TrimStart(new char[] { '\t', ' ' });
				if (line.StartsWith("#"))
				{
					try
					{
						arg = line.Substring(line.IndexOf('<') + 1, line.IndexOf('>') - line.IndexOf('<') - 1);
					}
					catch (Exception)
					{
					} // preprocess that don't have args
				}
                #region ifclass
                if (line.StartsWith("#ifclass "))
                {
                    if (!this.conditions.Peek())
                    {
                        this.conditions.Push(false);
                        continue;
                    }

                    //this.conditions.Push(arg.ToLower().Equals(MyPlayer.Me.PlayerClass.ToString().ToLower()));
                    string strCurrentClass = ObjectManager.Instance.Player.Class.ToString().ToUpper();
                    bool push = false;

                    if (arg.IndexOf(",") > -1)
                    {
                        string[] sconditions = arg.Split(','); //<class1,class2,class3,ect>

                        foreach (string strClass in sconditions)
                        {
                            if (strClass.Trim().ToUpper() == strCurrentClass)
                            {
                                push = true;
                            }
                        }
                    }
                    else
                    {
                        if (arg.Trim().ToUpper() == strCurrentClass) //<class>
                        {
                            push = true;
                        }
                    }

                    this.conditions.Push(push);
                }
                #endregion
                #region ifrace
                else if (line.StartsWith("#ifrace "))
                {
                    if (!this.conditions.Peek())
                    {
                        this.conditions.Push(false);
                        continue;
                    }

                    //this.conditions.Push(arg.ToLower().Equals(MyPlayer.Me.PlayerRace.ToString().ToLower()));

                    string strCurrentRace = ObjectManager.Instance.Player.Race.ToUpper();
                    bool push = false;

                    if (arg.IndexOf(",") > -1)
                    {
                        string[] sconditions = arg.Split(','); //<zone1,zone2,zone3,ect>

                        foreach (string strRace in sconditions)
                        {
                            if (strRace.Trim().ToUpper() == strCurrentRace)
                            {
                                push = true;
                            }
                        }
                    }
                    else
                    {
                        if (arg.Trim().ToUpper() == strCurrentRace) //<zone>
                        {
                            push = true;
                        }
                    }

                    this.conditions.Push(push);
                }
                #endregion
                #region iflevelrange
                else if (line.StartsWith("#iflevelrange "))
                {
                    if (!this.conditions.Peek())
                    {
                        this.conditions.Push(false);
                        continue;
                    }

                    string[] slevels = arg.Split(','); // <min,max>
                    bool push = false;

                    if (slevels.Length == 2)
                    {
                        int[] levels = { int.Parse(slevels[0]), int.Parse(slevels[1]) };
                        if (levels[1] > levels[0])
                        {
                            if (ObjectManager.Instance.Player.Level >= levels[0] &&
                                ObjectManager.Instance.Player.Level <= levels[1])
                                push = true;
                        }
                    }

                    this.conditions.Push(push);
                }
                #endregion
                #region ifkeyequals
                else if (line.StartsWith("#ifkeyequals "))
                {
                    if (!this.conditions.Peek())
                    {
                        this.conditions.Push(false);
                        continue;
                    }

                    ToonState state = new ToonState();
                    bool push = false;

                    if (arg.IndexOf(",") > -1)
                    {
                        string[] sconditions = arg.Split(','); // <Key,Status[|Status]>
                        string strKey = sconditions[0];
                        string strKeyStatus = state.Get(strKey.Trim());
                        if (strKeyStatus != null)
                        {
                            if (sconditions[1].IndexOf("|") > -1)
                            {
                                string[] strKeyStatuses = sconditions[1].Split('|');

                                for (int i = 0; i <= strKeyStatuses.Length - 1; i++)
                                {
                                    if (strKeyStatuses[i].Trim() == strKeyStatus)
                                    {
                                        push = true;
                                    }
                                }
                            }
                            else
                            {
                                if (strKeyStatus.Trim() == sconditions[1].Trim())
                                {
                                    push = true;
                                }
                            }
                        }
                        else
                        {
                            if (sconditions[1].Trim() == "NotExists")
                            {
                                push = true;
                            }
                        }
                    }
                    else
                    {
                        if (state.Get(arg.Trim()) != null)
                        {
                            push = true;
                        }
                    }

                    this.conditions.Push(push);
                }
                #endregion
                #region ifzone
                else if (line.StartsWith("#ifzone "))
                {
                    if (!this.conditions.Peek())
                    {
                        this.conditions.Push(false);
                        continue;
                    }

                    string strCurrentZone = ObjectManager.Instance.Player.RealZoneText.Trim().ToUpper();
                    bool push = false;

                    if (arg.IndexOf(",") > -1)
                    {
                        string[] sconditions = arg.Split(','); //<zone1,zone2,zone3,ect>

                        foreach (string strZone in sconditions)
                        {
                            if (strZone.Trim().ToUpper() == strCurrentZone)
                            {
                                push = true;
                            }
                        }
                    }
                    else
                    {
                        if (arg.Trim().ToUpper() == strCurrentZone) //<zone>
                        {
                            push = true;
                        }
                    }

                    this.conditions.Push(push);
                }
                #endregion
				else if (this.conditions.Peek() && line.StartsWith("#include "))
				{

					new Preprocessor(arg, this.lines, this.conditions);
				}
				else if (line.Equals("#endif"))
				{
					this.conditions.Pop();
				}
				else if (this.conditions.Peek())
					this.lines.Add(line);
			}
		}
        /*
		private void writeStream()
		{
			char[] chars;
			foreach (string line in lines)
			{
				chars = line.ToCharArray();
				foreach (char c in chars)
				{
					this.memStream.WriteByte((byte)c);
				}
				this.memStream.WriteByte((byte)13);
				this.memStream.WriteByte((byte)10);
			}
			this.memStream.Seek(0, SeekOrigin.Begin);
		}
        */
        private void writeStream()
        {
            foreach (string line in this.lines)
            {
                string ln = line + "\n";
                byte[] bytes = Encoding.UTF8.GetBytes(ln);
                for (int i = 0; i < Encoding.UTF8.GetByteCount(ln); i++)
                {
                    this.memStream.WriteByte(bytes[i]);
                }
            }
            this.memStream.Seek(0, SeekOrigin.Begin);
        }
	}
}
