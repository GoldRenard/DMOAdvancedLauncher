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

    /// <summary>
    /// Database context. It is wrapper interface over real DbContext.
    /// Do not forget to close context or just use it in using(...) statement.
    /// </summary>
    /// <seealso cref="IDatabaseManager"/>
    public interface IDatabaseContext : IDisposable {

        /// <summary>
        /// Creates specified entity in database
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        /// <returns>Stored entity</returns>
        TEntity Create<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Removes specified entity from database
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        /// <returns>Stored entity</returns>
        TEntity Remove<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Find specified entity by id.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="id">Entity <see cref="BaseEntity.Id"/></param>
        /// <returns>Entity if found, null otherwise</returns>
        TEntity FindById<TEntity>(long id) where TEntity : BaseEntity;

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        int SaveChanges();

        #region Guild operations

        /// <summary>
        /// Fetch the full hierarchy of guild by name and server including tamers, digimons, etc
        /// </summary>
        /// <param name="server">Guild's server</param>
        /// <param name="name">Guild's name</param>
        /// <returns>Guild or null</returns>
        Guild FetchGuild(Server server, string name);

        /// <summary>
        /// Returns guild by name and server.
        /// </summary>
        /// <param name="server">Guild's server</param>
        /// <param name="name">Guild's name</param>
        /// <returns>Guild or null</returns>
        /// <seealso cref="FetchGuild(Server, string)"/>
        Guild FindGuild(Server server, string name);

        #endregion Guild operations

        #region Digimon operations

        /// <summary>
        /// Returns random digimon by specified guild and level range
        /// </summary>
        /// <param name="guild">The guild of digimon</param>
        /// <param name="minlvl">Minimum level of digimon</param>
        /// <returns>Digimon or null</returns>
        Digimon FindRandomDigimon(Guild guild, int minlvl);

        /// <summary>
        /// Returns random digimon by specified tamer and level range
        /// </summary>
        /// <param name="tamer">The tamer of digimon</param>
        /// <param name="minlvl">Minimum level of digimon</param>
        /// <returns>Digimon or null</returns>
        Digimon FindRandomDigimon(Tamer tamer, int minlvl);

        #endregion Digimon operations

        #region Tamer operations

        /// <summary>
        /// Returns tamer instance by guild and tamer's name
        /// </summary>
        /// <param name="guild">Guild of tamer</param>
        /// <param name="name">Name of tamer</param>
        /// <returns>Tamer or null</returns>
        Tamer FindTamerByGuildAndName(Guild guild, string name);

        #endregion Tamer operations

        #region TamerType operations

        /// <summary>
        /// Returns tamer type by its code.
        /// </summary>
        /// <param name="code">Tamer's code</param>
        /// <returns></returns>
        TamerType FindTamerTypeByCode(int code);

        #endregion TamerType operations

        #region DigimonType operations

        /// <summary>
        /// Returns random digimon type
        /// </summary>
        /// <returns>Random digimon type</returns>
        DigimonType FindRandomDigimonType();

        /// <summary>
        /// Returns digimon type by global name
        /// </summary>
        /// <param name="name">Global name of digimon type</param>
        /// <returns>Digimon type</returns>
        List<DigimonType> FindDigimonTypesByName(string name);

        /// <summary>
        /// Returns digimon type by korean name
        /// </summary>
        /// <param name="name">Korean name of digimon type</param>
        /// <returns>Digimon type</returns>
        List<DigimonType> FindDigimonTypesByKoreanName(string name);

        /// <summary>
        /// Return digimon type by its code
        /// </summary>
        /// <param name="code">Digimon's code</param>
        /// <returns>Digimon type</returns>
        DigimonType FindDigimonTypeByCode(int code);

        /// <summary>
        /// Returns digimon types by its global search word (name without special symbols, etc)
        /// </summary>
        /// <param name="search">Global search word</param>
        /// <returns>Digimon types</returns>
        List<DigimonType> FindDigimonTypesBySearchGDMO(string search);

        /// <summary>
        /// Returns digimon types by its korean search word (name without special symbols, etc)
        /// </summary>
        /// <param name="search">Korean search word</param>
        /// <returns>Digimon types</returns>
        List<DigimonType> FindDigimonTypesBySearchKDMO(string search);

        /// <summary>
        /// Returns single digimon type by its global search word (name without special symbols, etc)
        /// </summary>
        /// <param name="search">Global search word</param>
        /// <returns>Digimon type</returns>
        DigimonType FindDigimonTypeBySearchGDMO(string search);

        /// <summary>
        /// Returns single digimon type by its korean search word (name without special symbols, etc)
        /// </summary>
        /// <param name="search">Korean search word</param>
        /// <returns>Digimon type</returns>
        DigimonType FindDigimonTypeBySearchKDMO(string search);

        /// <summary>
        /// Updates digimon type information with search word generation.
        /// </summary>
        /// <param name="type">Digimon type instance</param>
        /// <param name="isKorean"><b>True</b> if we should use korean search works, global otherwise.</param>
        void AddOrUpdateDigimonType(DigimonType type, bool isKorean);

        #endregion DigimonType operations

        #region Server operations

        /// <summary>
        /// Returns server list by server type
        /// </summary>
        /// <param name="ServerType">Server type</param>
        /// <returns>Server list</returns>
        List<Server> FindServerByServerType(Server.ServerType ServerType);

        #endregion Server operations

        /// <summary>
        /// Preparing search work of digimon nased on its name
        /// </summary>
        /// <param name="name">Digimon name</param>
        /// <returns>Search work</returns>
        string PrepareDigimonSearch(string name);
    }
}