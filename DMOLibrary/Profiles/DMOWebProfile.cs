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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DMOLibrary.Profiles
{
    public abstract class DMOWebProfile
    {
        protected DMODatabase Database = null;
        protected DownloadStatus download_status = new DownloadStatus();

        #region EVENTS
        /* Complete results:
         * 0 - all good
         * 1 - can't connect to database
         * 2 - web access error
         * 404 - guild not found
         * 3 - can't get guild info
         * 4 - web page is not supported or guild not found
         * */

        protected System.Windows.Threading.Dispatcher owner_dispatcher;

        public delegate void DownloadHandler(object sender);
        public delegate void DownloadCompleteHandler(object sender, DMODownloadResultCode code, guild result);
        public delegate void DownloadStatusChangedHandler(object sender, DownloadStatus status);
        public event DownloadHandler DownloadStarted;
        public event DownloadCompleteHandler DownloadCompleted;
        public event DownloadStatusChangedHandler StatusChanged;
        protected virtual void OnStarted()
        {
            IsBusy = true;
            if (DownloadStarted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadHandler((sender) =>
                    {
                        DownloadStarted(sender);
                    }), this);
                }
                else
                    DownloadStarted(this);
            }
        }
        protected virtual void OnCompleted(DMODownloadResultCode code, guild result)
        {
            if (DownloadCompleted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadCompleteHandler((sender, code_, result_) =>
                    {
                        DownloadCompleted(sender, code_, result_);
                    }), this, code, result);
                }
                else
                    DownloadCompleted(this, code, result);
            }
            IsBusy = false;
        }
        protected virtual void OnStatusChanged(DMODownloadStatusCode code, string info, int p, int pm)
        {
            download_status.code = code;
            download_status.info = info;
            download_status.progress = p;
            download_status.max_progress = pm;

            if (StatusChanged != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadStatusChangedHandler((sender, status_) =>
                    {
                        StatusChanged(sender, status_);
                    }), this, download_status);
                }
                else
                    StatusChanged(this, download_status);
            }
        }
        #endregion

        protected bool IsBusy = false;

        public void GetGuildAsync(System.Windows.Threading.Dispatcher owner_dispatcher, string g_name, server serv, bool isDetailed, int ActualDays)
        {
            this.owner_dispatcher = owner_dispatcher;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s1, e2) => { GetGuild(g_name, serv, isDetailed, ActualDays); };
            bw.RunWorkerAsync();
        }
        public digimon GetRandomDigimon(server serv, string g_name, int minlvl)
        {
            digimon d = new digimon();
            if (Database.OpenConnection())
            {
                d = Database.RandomDigimon(serv, g_name, minlvl);
                Database.CloseConnection();
            }
            return d;
        }

        public digimon GetRandomDigimon(server serv, string g_name, string t_name, int minlvl)
        {
            digimon d = new digimon();
            if (Database.OpenConnection())
            {
                d = Database.RandomDigimon(serv, g_name, t_name, minlvl);
                Database.CloseConnection();
            }
            return d;
        }

        public digimon_type GetRandomDigimonType()
        {
            digimon_type d = new digimon_type();
            if (Database.OpenConnection())
            {
                d = Database.RandomDigimonType();
                Database.CloseConnection();
            }
            return d;
        }

        public abstract guild GetGuild(string g_name, server serv, bool isDetailed, int ActualDays);
        protected abstract bool GetGuildInfo(ref guild g, bool isDetailed);
        protected abstract List<digimon> GetDigimons(tamer tamer, bool isDetailed);
        protected abstract bool StarterInfo(ref digimon digimon, string tamer_name);
        protected abstract bool DigimonInfo(ref digimon digimon, string tamer_name);
    }
}
