// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AdvancedLauncher.Environment.Containers
{
    [XmlType(TypeName = "Settings")]
    public class Settings : INotifyPropertyChanged
    {
        [XmlElement("Language")]
        public string LangFile;
        [XmlElement("DefaultProfile")]
        public int pDefault;

        [XmlArray("Profiles"), XmlArrayItem(typeof(Profile), ElementName = "Profile")]
        public ObservableCollection<Profile> pCollection { get; set; }

        #region Constructors

        public Settings()
        {
            this.pCollection = new ObservableCollection<Profile>();
        }

        public Settings(Settings s)
        {
            this.LangFile = s.LangFile;
            this.pDefault = s.pDefault;
            this.pCollection = new ObservableCollection<Profile>();
            foreach (Profile p in s.pCollection)
                pCollection.Add(new Profile(p));
        }

        public void Merge(Settings sNew)
        {
            this.LangFile = sNew.LangFile;
            this.pDefault = sNew.pDefault;
            this.pCollection = new ObservableCollection<Profile>();

            //Add clones of instances
            foreach (Profile p in sNew.pCollection)
                pCollection.Add(new Profile(p));
            OnCollectionChanged();

            //Updations corrent Profile
            Profile prof = pCollection.FirstOrDefault(i => i.pId == pCurrent.pId);
            if (prof == null)
                pCurrent = pCollection[0];
            else
                pCurrent = prof;
        }

        #endregion

        #region Collection Manipulating Section

        public void AddNewProfile()
        {
            Profile pNew = new Profile() { pName = "NewProfile" };
            foreach (Profile p in pCollection)
                if (p.pId > pNew.pId)
                    pNew.pId = p.pId;
            pNew.pId++;
            pCollection.Add(pNew);
            OnCollectionChanged();
        }

        public void DeleteProfile(Profile pDel)
        {
            if (pCollection.Count > 1)
            {
                bool IsCurrent = pDel.pId == pCurrent.pId;
                bool IsDefault = pDel.pId == pDefault;
                pCollection.Remove(pDel);
                OnCollectionChanged();
                if (IsCurrent)
                    pCurrent = pCollection[0];
                if (IsDefault)
                    pDefault = pCollection[0].pId;
            }
        }

        private Profile _pCurrent = null;
        [XmlIgnore]
        public Profile pCurrent
        {
            get
            {
                if (_pCurrent == null)
                {
                    Profile prof = pCollection.FirstOrDefault(i => i.pId == pDefault);
                    if (prof == null)
                        _pCurrent = pCollection[0];
                    else
                        _pCurrent = prof;

                }
                return _pCurrent;
            }
            set
            {
                _pCurrent = value;
                OnCurrentChanged();
            }
        }

        #endregion

        #region Events Section

        public delegate void ProfileLockedChangedHandler(bool IsLocked);
        public event ProfileLockedChangedHandler ProfileLocked;
        public void OnProfileLocked(bool IsLocked)
        {
            if (ProfileLocked != null)
                ProfileLocked(IsLocked);
        }

        public delegate void FileSystemLockedChangedHandler(bool IsLocked);
        public event FileSystemLockedChangedHandler FileSystemLocked;
        public void OnFileSystemLocked(bool IsLocked)
        {
            if (FileSystemLocked != null)
                FileSystemLocked(IsLocked);
        }

        public delegate void ClosingLockedChangedHandler(bool IsLocked);
        public event ClosingLockedChangedHandler ClosingLocked;
        public void OnClosingLocked(bool IsLocked)
        {
            if (ClosingLocked != null)
                ClosingLocked(IsLocked);
        }

        public delegate void ProfileChangedHandler();
        public event ProfileChangedHandler ProfileChanged;
        protected void OnCurrentChanged()
        {
            if (ProfileChanged != null)
                ProfileChanged();
        }

        public event ProfileChangedHandler CollectionChanged;
        protected void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
