// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database {

    public static class MergeHelper {
        private static object MERGE_LOCKER = new object();

        public static bool Merge(Guild guild) {
            lock (MERGE_LOCKER) {
                if (guild == null) {
                    return false;
                }

                using (MainContext context = new MainContext()) {
                    Guild storedGuild = context.Guilds.FirstOrDefault(g =>
                        g.Server.Id == guild.Server.Id && g.Name == guild.Name);

                    // If we don't have this guild yet, add it
                    if (storedGuild == null) {
                        guild.Server = context.Servers.First(e => e.Id == guild.Server.Id);
                        foreach (Tamer tamer in guild.Tamers) {
                            if (tamer.Type != null) {
                                tamer.Type = context.TamerTypes.First(e => e.Id == tamer.Type.Id);
                            }
                            foreach (Digimon digimon in tamer.Digimons) {
                                digimon.Type = context.DigimonTypes.First(e => e.Id == digimon.Type.Id);
                            }
                        }
                        context.Guilds.Add(guild);
                        context.SaveChanges();
                        return true;
                    }

                    // of we have two guilds with the same time - do nothing
                    if (storedGuild.UpdateTime != null) {
                        if (storedGuild.UpdateTime.Equals(guild.UpdateTime)) {
                            return true;
                        }
                    }

                    bool result = MergeGuild(context, guild, storedGuild);
                    if (result) {
                        context.SaveChanges();
                    }
                    return result;
                }
            }
        }

        private static bool MergeGuild(MainContext context, Guild mergeGuild, Guild storedGuild) {
            MergeEntity(mergeGuild, storedGuild, false, "Rep", "Rank", "UpdateTime");

            // Syncronize the tamer sets by name
            List<Tamer> tamersToDelete = storedGuild.Tamers.Where(t1 => mergeGuild.Tamers.FirstOrDefault(t2 => t2.Name == t1.Name) == null).ToList();
            List<Tamer> newTamers = mergeGuild.Tamers.Where(t1 => storedGuild.Tamers.FirstOrDefault(t2 => t2.Name == t1.Name) == null).ToList();
            foreach (Tamer tamer in tamersToDelete) {
                storedGuild.Tamers.Remove(tamer);
                context.Tamers.Remove(tamer);
            }
            foreach (Tamer tamer in newTamers) {
                tamer.Guild = storedGuild;
                storedGuild.Tamers.Add(tamer);
            }

            foreach (Tamer storedTamer in storedGuild.Tamers) {
                if (!MergeTamer(context, mergeGuild.Tamers.Single(t => t.Name == storedTamer.Name), storedTamer)) {
                    return false;
                }
            }

            if (mergeGuild.IsDetailed && !storedGuild.IsDetailed) {
                storedGuild.IsDetailed = true;
            }
            return true;
        }

        private static bool MergeTamer(MainContext context, Tamer mergeTamer, Tamer storedTamer) {
            MergeEntity(mergeTamer, storedTamer, false, "Level", "Rank", "Type", "IsMaster");
            if (storedTamer.Type != null) {
                storedTamer.Type = context.TamerTypes.First(t => t.Id == storedTamer.Type.Id);
            }

            // Syncronize the digimon sets by digimon type (one digimon per-type limitation)
            List<Digimon> digimonsToDelete = storedTamer.Digimons.Where(d1 => mergeTamer.Digimons.FirstOrDefault(d2 => d2.Type.Id == d1.Type.Id) == null).ToList();
            List<Digimon> newDigimons = mergeTamer.Digimons.Where(d1 => storedTamer.Digimons.FirstOrDefault(d2 => d2.Type.Id == d1.Type.Id) == null).ToList();
            foreach (Digimon digimon in digimonsToDelete) {
                storedTamer.Digimons.Remove(digimon);
                context.Digimons.Remove(digimon);
            }
            foreach (Digimon digimon in newDigimons) {
                digimon.Tamer = storedTamer;
                storedTamer.Digimons.Add(digimon);
            }

            foreach (Digimon storedDigimon in storedTamer.Digimons) {
                if (!MergeDigimon(context, mergeTamer.Digimons.Single(d => d.Type.Id == storedDigimon.Type.Id), storedDigimon)) {
                    return false;
                }
            }
            return true;
        }

        private static bool MergeDigimon(MainContext context, Digimon mergeDigimon, Digimon storedDigimon) {
            if (mergeDigimon.Tamer.Guild.IsDetailed) {
                MergeEntity(mergeDigimon, storedDigimon, false, "Rank", "Level", "Name", "SizeCm", "SizePc", "SizeRank");
            } else {
                MergeEntity(mergeDigimon, storedDigimon, false, "Rank", "Level");
            }
            return true;
        }

        public static void MergeEntity<TIn, TOut>(TIn input, TOut output, bool exclude = false, params string[] properties)
            where TIn : BaseEntity
            where TOut : BaseEntity {
            if ((input == null) || (output == null)) return;
            Type inType = input.GetType();
            Type outType = output.GetType();
            foreach (PropertyInfo info in inType.GetProperties()) {
                PropertyInfo outfo = ((info != null) && info.CanRead)
                    ? outType.GetProperty(info.Name, info.PropertyType)
                    : null;
                if (outfo != null && outfo.CanWrite && (outfo.PropertyType.Equals(info.PropertyType))) {
                    if (properties != null) {
                        if (exclude && !properties.Contains(info.Name)) {
                            outfo.SetValue(output, info.GetValue(input, null), null);
                        } else if (properties.Contains(info.Name)) {
                            outfo.SetValue(output, info.GetValue(input, null), null);
                        }
                    } else {
                        outfo.SetValue(output, info.GetValue(input, null), null);
                    }
                }
            }
        }
    }
}