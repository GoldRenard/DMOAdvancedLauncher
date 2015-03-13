using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database {

    public static class MergeHelper {

        public static bool Merge(Guild guild) {
            if (guild == null) {
                return false;
            }

            Guild storedGuild = MainContext.Instance.Guilds.FirstOrDefault(g =>
                g.Server.Id == guild.Server.Id && g.Name == guild.Name);

            // If we don't have this guild yet, add it
            if (storedGuild == null) {
                MainContext.Instance.Guilds.Add(guild);
                MainContext.Instance.SaveChanges();
                return true;
            }

            bool result = MergeGuild(guild, storedGuild);
            if (result) {
                MainContext.Instance.SaveChanges();
            }
            return result;
        }

        private static bool MergeGuild(Guild mergeGuild, Guild storedGuild) {
            MergeEntity(mergeGuild, storedGuild, false, "Rep", "Rank", "UpdateTime");

            // Syncronize the tamer sets by name
            List<Tamer> tamersToDelete = storedGuild.Tamers.Where(t1 => mergeGuild.Tamers.FirstOrDefault(t2 => t2.Name == t1.Name) == null).ToList();
            List<Tamer> newTamers = mergeGuild.Tamers.Where(t1 => storedGuild.Tamers.FirstOrDefault(t2 => t2.Name == t1.Name) == null).ToList();
            foreach (Tamer tamer in tamersToDelete) {
                storedGuild.Tamers.Remove(tamer);
                MainContext.Instance.Tamers.Remove(tamer);
            }
            foreach (Tamer tamer in newTamers) {
                tamer.Guild = storedGuild;
                storedGuild.Tamers.Add(tamer);
            }

            foreach (Tamer storedTamer in storedGuild.Tamers) {
                if (!MergeTamer(mergeGuild.Tamers.Single(t => t.Name == storedTamer.Name), storedTamer)) {
                    return false;
                }
            }

            if (mergeGuild.IsDetailed && !storedGuild.IsDetailed) {
                storedGuild.IsDetailed = true;
            }
            return true;
        }

        private static bool MergeTamer(Tamer mergeTamer, Tamer storedTamer) {
            MergeEntity(mergeTamer, storedTamer, false, "Level", "Rank", "Type", "IsMaster");

            // Syncronize the digimon sets by digimon type (one digimon per-type limitation)
            List<Digimon> digimonsToDelete = storedTamer.Digimons.Where(d1 => mergeTamer.Digimons.FirstOrDefault(d2 => d2.Type.Id == d1.Type.Id) == null).ToList();
            List<Digimon> newDigimons = mergeTamer.Digimons.Where(d1 => storedTamer.Digimons.FirstOrDefault(d2 => d2.Type.Id == d1.Type.Id) == null).ToList();
            foreach (Digimon digimon in digimonsToDelete) {
                storedTamer.Digimons.Remove(digimon);
                MainContext.Instance.Digimons.Remove(digimon);
            }
            foreach (Digimon digimon in newDigimons) {
                digimon.Tamer = storedTamer;
                storedTamer.Digimons.Add(digimon);
            }

            foreach (Digimon storedDigimon in storedTamer.Digimons) {
                if (!MergeDigimon(mergeTamer.Digimons.Single(d => d.Type.Id == storedDigimon.Type.Id), storedDigimon)) {
                    return false;
                }
            }
            return true;
        }

        private static bool MergeDigimon(Digimon mergeDigimon, Digimon storedDigimon) {
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