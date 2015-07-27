using System;
using AdvancedLauncher.Model;

namespace AdvancedLauncher.Management.Interfaces {

    public interface ILanguageManager : IManager {

        void Save(string filename, LanguageModel model);

        LanguageModel Read(string tFile);

        bool Load(string tName);

        string[] GetTranslations();

        event EventHandler LanguageChanged;

        string GetDefaultName();

        LanguageModel Model {
            set;
            get;
        }
    }
}