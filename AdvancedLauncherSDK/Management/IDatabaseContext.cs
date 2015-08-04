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
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Management {

    public interface IDatabaseContext : IDisposable {

        TEntity Create<TEntity>(TEntity entity) where TEntity : BaseEntity;

        TEntity Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;

        TEntity FindById<TEntity>(long id) where TEntity : BaseEntity;

        int SaveChanges();

        #region Guild operations

        Guild FetchGuild(Server server, string name);

        Guild FindGuild(Server server, string name);

        #endregion Guild operations

        #region Digimon operations

        Digimon FindRandomDigimon(Guild guild, int minlvl);

        Digimon FindRandomDigimon(Tamer tamer, int minlvl);

        #endregion Digimon operations

        #region Tamer operations

        Tamer FindTamerByGuildAndName(Guild guild, string name);

        #endregion Tamer operations

        #region TamerType operations

        TamerType FindTamerTypeByCode(int code);

        #endregion TamerType operations

        #region DigimonType operations

        DigimonType FindRandomDigimonType();

        List<DigimonType> FindDigimonTypesByName(string name);

        List<DigimonType> FindDigimonTypesByKoreanName(string name);

        DigimonType FindDigimonTypeByCode(int code);

        List<DigimonType> FindDigimonTypesBySearchGDMO(string search);

        List<DigimonType> FindDigimonTypesBySearchKDMO(string search);

        DigimonType FindDigimonTypeBySearchGDMO(string search);

        DigimonType FindDigimonTypeBySearchKDMO(string search);

        void AddOrUpdateDigimonType(DigimonType type, bool isKorean);

        #endregion DigimonType operations

        #region Server operations

        List<Server> FindServerByServerType(Server.ServerType ServerType);

        #endregion Server operations

        string PrepareDigimonSearch(string name);
    }
}