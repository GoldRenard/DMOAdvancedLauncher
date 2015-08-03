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

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Permissions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.Database.Context {

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class ContextWrapper : CrossDomainObject, IDatabaseContext {
        private readonly MainContext Context;

        public ContextWrapper(MainContext Context) {
            this.Context = Context;
        }

        #region Guild operations

        public Guild FetchGuild(Server server, string name) {
            return Context.Guilds
                .Include(g => g.Server)
                .Include(g => g.Tamers)
                .Include(g => g.Tamers.Select(t => t.Guild))
                .Include(g => g.Tamers.Select(t => t.Type))
                .Include(g => g.Tamers.Select(t => t.Digimons))
                .Include(g => g.Tamers.Select(t => t.Digimons.Select(d => d.Tamer)))
                .Include(g => g.Tamers.Select(t => t.Digimons.Select(d => d.Type)))
                .FirstOrDefault(g => g.Server.Id == server.Id && g.Name == name);
        }

        public Guild FindGuild(Server server, string name) {
            return Context.Guilds
                .FirstOrDefault(g => g.Server.Id == server.Id && g.Name == name);
        }

        #endregion Guild operations

        #region Digimon operations

        public Digimon FindRandomDigimon(Guild guild, int minlvl) {
            return Context.Digimons.Where(e => e.Tamer.Guild.Id == guild.Id && e.Level >= minlvl)
                .OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public Digimon FindRandomDigimon(Tamer tamer, int minlvl) {
            return Context.Digimons.Where(e => e.Tamer.Id == tamer.Id && e.Level >= minlvl)
                .OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        #endregion Digimon operations

        #region Tamer operations

        public Tamer FindTamerByGuildAndName(Guild guild, string name) {
            return Context.Tamers.FirstOrDefault(e => e.Guild.Id == guild.Id && e.Name == name);
        }

        #endregion Tamer operations

        #region TamerType operations

        public TamerType FindTamerTypeByCode(int code) {
            return Context.TamerTypes.FirstOrDefault(e => e.Code == code);
        }

        #endregion TamerType operations

        #region DigimonType operations

        public DigimonType FindRandomDigimonType() {
            return Context.DigimonTypes.OrderBy(c => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public List<DigimonType> FindDigimonTypesByName(string name) {
            return Context.DigimonTypes.Where(e => e.Name == name).ToList();
        }

        public List<DigimonType> FindDigimonTypesByKoreanName(string name) {
            return Context.DigimonTypes.Where(e => e.NameKorean == name).ToList();
        }

        public DigimonType FindDigimonTypeByCode(int code) {
            return Context.DigimonTypes.FirstOrDefault(e => e.Code == code);
        }

        public List<DigimonType> FindDigimonTypesBySearchGDMO(string search) {
            return Context.DigimonTypes.Where(e => e.SearchGDMO == search).ToList();
        }

        public List<DigimonType> FindDigimonTypesBySearchKDMO(string search) {
            return Context.DigimonTypes.Where(e => e.SearchKDMO == search).ToList();
        }

        public DigimonType FindDigimonTypeBySearchGDMO(string search) {
            return Context.DigimonTypes.FirstOrDefault(e => e.SearchGDMO == search);
        }

        public DigimonType FindDigimonTypeBySearchKDMO(string search) {
            return Context.DigimonTypes.FirstOrDefault(e => e.SearchKDMO == search);
        }

        public void AddOrUpdateDigimonType(DigimonType type, bool isKorean) {
            DigimonType ordinal = Context.DigimonTypes.FirstOrDefault(e => e.Code == type.Code);
            if (ordinal == null) {
                if (isKorean) {
                    type.NameKorean = type.Name;
                    type.SearchKDMO = PrepareDigimonSearch(type.Name);
                } else {
                    type.SearchGDMO = PrepareDigimonSearch(type.Name);
                }
                Context.DigimonTypes.Add(type);
                return;
            }

            if (isKorean) {
                ordinal.NameKorean = type.Name;
                ordinal.SearchKDMO = PrepareDigimonSearch(type.Name);
            } else {
                ordinal.SearchGDMO = PrepareDigimonSearch(type.Name);
            }
        }

        public string PrepareDigimonSearch(string name) {
            if (name == null) {
                return null;
            }
            return name
                .Replace(" ", String.Empty)
                .Replace("(", String.Empty)
                .Replace(")", String.Empty)
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Replace("-", String.Empty)
                .ToLower();
        }

        #endregion DigimonType operations

        #region Server operations

        public List<Server> FindServerByServerType(Server.ServerType ServerType) {
            return Context.Servers.Where(i => i.Type == ServerType).ToList();
        }

        #endregion Server operations

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    Context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int SaveChanges() {
            return Context.SaveChanges();
        }

        #endregion IDisposable Support
    }
}