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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model;
using Ninject;

namespace AdvancedLauncher.Management {

    public sealed class LanguageManager : ILanguageManager {
        public const string DefaultName = "en-US";

        public static LanguageModel Default = new LanguageModel();

        private Dictionary<string, LanguageModel> Collection = new Dictionary<string, LanguageModel>();

        private LanguageModel _Model = new LanguageModel();

        public LanguageModel Model {
            set {
                _Model = value;
                OnChanged();
            }
            get {
                return _Model;
            }
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        #region Save/Read/Load

        public void Save(string filename, LanguageModel model) {
            XmlSerializer writer = new XmlSerializer(typeof(LanguageModel));
            StreamWriter file = new StreamWriter(filename);
            writer.Serialize(file, model);
            file.Close();
        }

        public LanguageModel Read(string tFile) {
            LanguageModel language = null;
            Collection.TryGetValue(tFile, out language);
            if (language == null && File.Exists(tFile)) {
                XmlSerializer reader = new XmlSerializer(typeof(LanguageModel));
                using (var file = new StreamReader(tFile)) {
                    language = (LanguageModel)reader.Deserialize(file);
                    Collection.Add(tFile, language);
                }
            }
            return language;
        }

        public void Initialize() {
            if (string.IsNullOrEmpty(EnvironmentManager.Settings.LanguageFile)) {
                if (Load(CultureInfo.CurrentCulture.Name)) {
                    EnvironmentManager.Settings.LanguageFile = CultureInfo.CurrentCulture.Name;
                } else {
                    Load(GetDefaultName());
                    EnvironmentManager.Settings.LanguageFile = GetDefaultName();
                }
            } else {
                if (!Load(EnvironmentManager.Settings.LanguageFile)) {
                    Load(GetDefaultName());
                    EnvironmentManager.Settings.LanguageFile = GetDefaultName();
                }
            }
        }

        public bool Load(string tName) {
            if (tName == DefaultName) {
                Model = Default;
                return true;
            }
            LanguageModel newModel = Read(Path.Combine(EnvironmentManager.LanguagesPath, tName + ".lng"));
            if (newModel == null) {
                return false;
            } else {
                Model = newModel;
            }
            return true;
        }

        public string[] GetTranslations() {
            string[] translations = null;
            Regex regex = new Regex(@"^[a-z]{2,3}(?:-[A-Z]{2,3}(?:-[a-zA-Z]{4})?)?$");
            if (Directory.Exists(EnvironmentManager.LanguagesPath)) {
                translations = Directory.GetFiles(EnvironmentManager.LanguagesPath, "*.lng")
                    .Where(path => regex.IsMatch(Path.GetFileNameWithoutExtension(path))).ToArray();
            }
            return translations;
        }

        public string GetDefaultName() {
            return DefaultName;
        }

        #endregion Save/Read/Load

        #region Event Handlers

        public event EventHandler LanguageChanged;

        private void OnChanged() {
            if (LanguageChanged != null) {
                LanguageChanged(Model, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Event Handlers
    }
}