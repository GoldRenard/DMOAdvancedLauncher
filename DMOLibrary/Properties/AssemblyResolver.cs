// ======================================================================
// DMOLibrary
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.Reflection;

namespace DMOLibrary
{
    public class AssemblyResolver
    {
        public static void HandleUnresovledAssemblies()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        private static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Replace("System.Data.SQLite", string.Empty) != args.Name)
            {
                var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), "x");
                if (IntPtr.Size == 8)
                    path += "64";
                else
                    path += "86";
                path = System.IO.Path.Combine(path, "System.Data.SQLite.DLL");
                Assembly assembly = Assembly.LoadFrom(path);
                return assembly;
            }
            return null;
        }
    }
}
