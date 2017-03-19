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

    Copyright Pontus Borg 2008
  
 */

using System.IO;

namespace TaskEngine.Helpers
{

    public class RemoteTask
    {
        public delegate MemoryStream RemoteFile(string fileName);
        public delegate bool RemoteFileExist(string fileName);

        public static RemoteFile GetRemoteFile;
        public static RemoteFileExist GetRemoteFileExist;
        public static bool RemoteEnabled = false;
        public static string RemoteFolder = "";

        public static bool FileExist(string fileName)
        {
            fileName = fileName.Replace(@"\/", @"\");

            if (RemoteEnabled == false)
                return File.Exists(fileName);

            if (GetRemoteFileExist == null)
                return false;

            return GetRemoteFileExist(fileName);
        }

        public static StreamReader GetFile(string fileName)
        {
            fileName = fileName.Replace(@"\/", @"\");
            if (RemoteEnabled == false)
                return new StreamReader(fileName);

            if (GetRemoteFile == null)
                return null;

            return new StreamReader(GetRemoteFile(fileName));
        }

    }
}
