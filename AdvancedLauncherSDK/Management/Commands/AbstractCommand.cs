// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedLauncher.SDK.Management.Commands {

    /// <summary>
    /// Common command class
    /// </summary>
    /// <seealso cref="ICommandManager"/>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="AbstractExtendedCommand"/>
    public abstract class AbstractCommand : CrossDomainObject, ICommand {
        private readonly string commandName;
        private readonly string commandDescription;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="commandName">Command name to execute. See <see cref="ICommand.GetName"/></param>
        /// <param name="commandDescription">Command description for help. See <see cref="ICommand.GetDescription"/></param>
        public AbstractCommand(string commandName, string commandDescription) {
            this.commandName = commandName;
            this.commandDescription = commandDescription;
        }

        /// <summary>
        /// The command action
        /// </summary>
        /// <param name="args">Input arguments</param>
        /// <returns>Returns <B>true</B> if command successfully executed, <B>false</B> otherwise.</returns>
        public abstract bool DoCommand(string[] args);

        /// <summary>
        /// Command description for help
        /// </summary>
        /// <returns>Command description</returns>
        public virtual string GetDescription() {
            return commandDescription;
        }

        /// <summary>
        /// Command name to execute
        /// </summary>
        /// <returns>Command name</returns>
        public virtual string GetName() {
            return commandName;
        }

        /// <summary>
        /// Prints data in table view.
        /// </summary>
        /// <param name="ColumnNames">List of column headers</param>
        /// <param name="columns">List of columns data. This array should be the same size with ColumnNames param.</param>
        /// <returns></returns>
        protected List<string> PrintTable(List<string> ColumnNames, params List<string>[] columns) {
            int[] lenghts = new int[ColumnNames.Count];
            for (int i = 0; i < ColumnNames.Count; i++) {
                lenghts[i] = columns[i].Max(c => c.Length);
                if (lenghts[i] < ColumnNames[i].Length) {
                    lenghts[i] = ColumnNames[i].Length;
                }
                lenghts[i] = lenghts[i] + 2;
            }

            List<string> output = new List<string>();
            StringBuilder recordBuilder = new StringBuilder(" ");
            for (int columnName = 0; columnName < ColumnNames.Count; columnName++) {
                string value = ColumnNames[columnName];
                recordBuilder.Append(value);
                int spaceleft = lenghts[columnName] - value.Length;
                while (spaceleft > 0) {
                    recordBuilder.Append(" ");
                    spaceleft--;
                }
            }
            output.Add(recordBuilder.ToString());
            recordBuilder = new StringBuilder("");
            for (int i = 0; i < lenghts.Sum(); i++) {
                recordBuilder.Append("-");
            }
            output.Add(recordBuilder.ToString());

            for (int record = 0; record < columns[0].Count; record++) {
                recordBuilder = new StringBuilder(" ");
                for (int column = 0; column < columns.Length; column++) {
                    string value = columns[column][record];
                    recordBuilder.Append(value);
                    int spaceleft = lenghts[column] - value.Length;
                    while (spaceleft > 0) {
                        recordBuilder.Append(" ");
                        spaceleft--;
                    }
                }
                output.Add(recordBuilder.ToString());
            }
            return output;
        }
    }
}