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

using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Runtime.Serialization;
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using Microsoft.DwayneNeed.Interop;

namespace AdvancedLauncher.Model {

    public class PageItemViewModel : NamedItemViewModel<PageItem> {

        public PageItemViewModel(PageItem Item, ILanguageManager LanguageManager)
            : base(Item, LanguageManager) {
        }

        public object Content {
            get {
                object Control = null;
                bool IsContract = false;
                bool IsAirspaceDecoration = false;
                try {
                    Control = Item.Content.GetControl();
                } catch (SerializationException) {
                    INativeHandleContract contract = Item.Content.GetControl(true) as INativeHandleContract;
                    if (contract != null) {
                        Control = FrameworkElementAdapters.ContractToViewAdapter(contract);
                        IsContract = true;
                        IsAirspaceDecoration = Item.Content.EnableAirspaceFix;
                    }
                }
                if (Control != null) {
                    if (IsContract && IsAirspaceDecoration) {
                        Control = new AirspaceDecorator() {
                            AirspaceMode = AirspaceMode.Redirect,
                            IsInputRedirectionEnabled = true,
                            IsOutputRedirectionEnabled = true,
                            Background = Brushes.White,
                            Content = Control
                        };
                    }
                }
                return Control;
            }
        }
    }
}