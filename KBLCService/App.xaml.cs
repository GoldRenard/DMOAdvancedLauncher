// ======================================================================
// DMO KEYBOARD LAYOUT CHANGER SERVICE
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
using System.Threading;
using System.Windows;

namespace KBLCService {

    public partial class App : Application {
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.MenuItem ControlItem, AltItem;
        private static HotkeyService Service = new HotkeyService();

        private Mutex AppMutex;

        /// <summary>
        /// Запуск приложения
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="args">Аргументы</param>
        public void OnAppStartup(object sender, StartupEventArgs args) {
            // Мы должны запускать только один экземпляр приложения
            if ((AppMutex = Utils.CreateMutex()) != null) {
                this.Exit += (s, e) => {
                    if (TrayIcon != null) {
                        TrayIcon.Visible = false;
                    }
                    Service.Stop();
                    AppMutex.ReleaseMutex();
                };
                bool IsTray = !Utils.HasParameter(args.Args, "-notray");
                bool IsAttach = Utils.HasParameter(args.Args, "-attach");

                if (IsTray) {
                    BuildIcon();
                }

                if (IsAttach) {
                    Service.Detached += () => {
                        Utils.CloseApp();
                    };
                }

                bool IsControl = "2".Equals(Utils.GetRegistryValue(@"HKEY_CURRENT_USER\Keyboard Layout\Toggle\Hotkey", "1"));
                SetMode(IsControl);
                Service.Start(IsAttach, IsControl);
            } else {
                Utils.CloseApp();
            }
        }

        /// <summary>
        /// Построение иконки трея
        /// </summary>
        private void BuildIcon() {
            TrayIcon = new System.Windows.Forms.NotifyIcon();
            TrayIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/KBLCService;component/app_icon.ico")).Stream);
            TrayIcon.Visible = true;

            System.Windows.Forms.ContextMenu cMenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem mExit = new System.Windows.Forms.MenuItem();
            mExit.Text = "Exit";

            ControlItem = new System.Windows.Forms.MenuItem();
            ControlItem.Text = "CTFL + SHIFT";
            ControlItem.RadioCheck = true;

            AltItem = new System.Windows.Forms.MenuItem();
            AltItem.Text = "ALT + SHIFT";
            AltItem.RadioCheck = true;

            cMenu.MenuItems.Add(ControlItem);
            cMenu.MenuItems.Add(AltItem);
            cMenu.MenuItems.Add("-");
            cMenu.MenuItems.Add(mExit);

            TrayIcon.Text = "Keyboard Layout Changer Service";
            TrayIcon.ContextMenu = cMenu;

            mExit.Click += (s, e) => {
                Utils.CloseApp();
            };

            ControlItem.Click += (s, e) => {
                SetMode(true);
            };
            AltItem.Click += (s, e) => {
                SetMode(false);
            };
        }

        /// <summary>
        /// Установка режима хоткея
        /// </summary>
        /// <param name="IsControl">True если CTRL+SHIFT, False если ALT+SHIFT</param>
        private void SetMode(bool IsControl) {
            Service.SetMode(IsControl);
            if (ControlItem != null) {
                ControlItem.Checked = IsControl;
            }
            if (AltItem != null) {
                AltItem.Checked = !IsControl;
            }
        }
    }
}