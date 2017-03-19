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

namespace TaskEngine.Data
{
    public class ToonState
    {
        // Simple saved storage or key=>value pairs

        Dictionary<string, string> dic = new Dictionary<string, string>();
        string toonName = null;
        bool changed = false;

        public void Save()
        {
            lock (this)
            {
                if (!this.changed)
                    return;
                if (this.toonName == null)
                    return;
                string filename = "Plugins\\PPather\\ToonInfo\\" + this.toonName + ".txt";
                try
                {
                    System.IO.TextWriter s = System.IO.File.CreateText(filename);
                    foreach (string key in this.dic.Keys)
                    {
                        string val = this.dic[key];
                        s.WriteLine(key + "|" + val);
                    }
                    s.Close();
                }
                catch (Exception e)
                {
                    Logger.Log("!Error:Exception writing toon state data: " + e);
                }
                this.changed = false;
            }

        }

        public void Load()
        {
            lock (this)
            {
                if (this.toonName == null)
                    return;
                this.dic = new Dictionary<string, string>();

                // Load from file
                Logger.Log("Loading toon data...");
                try
                {
                    string filename = "Plugins\\PPather\\ToonInfo\\" + this.toonName + ".txt";
                    System.IO.TextReader s = System.IO.File.OpenText(filename);

                    int nr = 0;
                    string line;
                    while ((line = s.ReadLine()) != null)
                    {
                        char[] splitter = { '|' };
                        string[] st = line.Split(splitter);
                        if (st.Length == 2)
                        {
                            string key = st[0];
                            string val = st[1];
                            this.Set(key, val);
                            nr++;
                        }
                    }
                    Logger.Log("Loaded " + nr + " toon state data");

                    s.Close();
                }
                catch (Exception)
                {
                    Logger.Log("No toon data to read yet, but no worries :)");
                }
                this.changed = false;
            }
        }

        public void SetToonName(string name)
        {
            this.Save();
            this.toonName = name;
            this.Load();
        }

        public string Get(string key)
        {
            string val;
            if (this.dic.TryGetValue(key, out val))
            {
                return val;
            }
            return null;
        }

        public void Set(string key, string name)
        {
            if (this.dic.ContainsKey(key))
                this.dic.Remove(key);
            this.dic.Add(key, name);
            this.changed = true;
        }

        public List<string> GetKeysContaining(string str)
        {
            List<string> l = new List<string>();
            foreach (string key in this.dic.Keys)
                if (key.Contains(str))
                    l.Add(key);
            return l;
        }
    }
}