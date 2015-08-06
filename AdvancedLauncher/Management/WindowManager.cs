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
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Windows;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Commands;
using AdvancedLauncher.UI.Pages;
using AdvancedLauncher.UI.Windows;
using MahApps.Metro.Controls;
using Microsoft.DwayneNeed.Interop;
using Ninject;

namespace AdvancedLauncher.Management {

    public class WindowManager : CrossDomainObject, IWindowManager {
        private bool MainMenuSeparatorAdded = false;

        private bool IsStarted = false;

        private bool IsCurrentContract = false;

        private TransitionType DefaultTransition = TransitionType.Left;

        private ConcurrentStack<IWindow> WindowStack {
            get;
            set;
        } = new ConcurrentStack<IWindow>();

        private IWindow CurrentWindow {
            get;
            set;
        }

        public ObservableCollection<SDK.Management.Windows.MenuItem> MenuItems {
            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
            get;
            private set;
        } = new ObservableCollection<SDK.Management.Windows.MenuItem>();

        public ObservableCollection<PageItem> PageItems {
            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
            get;
            private set;
        } = new ObservableCollection<PageItem>();

        #region Base Controls

        private MainWindow MainWindow {
            get;
            set;
        }

        private PageItem MainPage {
            get;
            set;
        }

        private PageItem Gallery {
            get;
            set;
        }

        private PageItem Community {
            get;
            set;
        }

        private PageItem Personalization {
            get;
            set;
        }

        private List<NamedItem> OwnedItems = new List<NamedItem>();

        #endregion Base Controls

        #region Injection

        [Inject]
        public Logger Logger {
            get; set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get;
            set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get;
            set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        #endregion Injection

        public void Initialize() {
            EnvironmentManager.FileSystemLocked += OnFileSystemLocked;
            ProfileManager.ProfileChanged += OnProfileChanged;
            ProfileManager.ProfileLocked += OnProfileLocked;
        }

        public void Start() {
            if (IsStarted) {
                return;
            }
            this.MainWindow = App.Kernel.Get<MainWindow>(); // do not inject it directly, we should not export it as public property
            Application.Current.MainWindow = MainWindow;
            DefaultTransition = MainWindow.transitionLayer.Transition;
            MainWindow.Loaded += (s, e) => {
                BuildMenu();
                //ShowWindow(new PagesWindow().Container);
                ShowWindow(Logger.Container);
            };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                App.Kernel.Get<Splashscreen>().Close();
                MainWindow.Show();
                OnProfileChanged(null, null); // force update tab availability
            }
            IsStarted = true;
        }

        private void BuildMenu() {
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "Settings", FindResource<Canvas>("appbar_settings"), new Thickness(5, 5, 5, 5), new ModelCommand((p) => {
                MainWindow.SettingsFlyout.Width = MainWindow.ProfileSwitcher.ActualWidth + MainWindow.FLYOUT_WIDTH_MIN;
                MainWindow.SettingsFlyout.IsOpen = true;
            })));
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "Console", FindResource<Canvas>("appbar_app"), new Thickness(5, 7, 5, 7), new ModelCommand((p) => {
                ShowWindow(Logger.Container);
                MainWindow.MenuFlyout.IsOpen = false;
            })));
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "About", FindResource<Canvas>("appbar_information"), new Thickness(9, 4, 9, 4), new ModelCommand((p) => {
                ShowWindow(App.Kernel.Get<About>().Container);
                MainWindow.MenuFlyout.IsOpen = false;
            })));

            MainPage = new PageItem(LanguageManager, "MainWindow_NewsTab", new MainPage());
            Gallery = new PageItem(LanguageManager, "MainWindow_GalleryTab", new Gallery());
            Community = new PageItem(LanguageManager, "MainWindow_CommunityTab", new Community());
            Personalization = new PageItem(LanguageManager, "MainWindow_PersonalizationTab", new Personalization());

            PageItems.Add(MainPage);
            PageItems.Add(Gallery);
            PageItems.Add(Community);
            PageItems.Add(Personalization);

            OwnedItems.AddRange(MenuItems);
            OwnedItems.AddRange(PageItems);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public void ShowWindow(IWindow window) {
            if (window == null) {
                throw new ArgumentException("Window argument cannot be null");
            }

            bool IsContract = false;
            object Control = null;
            try {
                Control = window.GetControl();
            } catch (SerializationException e) {
                INativeHandleContract contract = window.GetControl(true) as INativeHandleContract;
                if (contract != null) {
                    Control = FrameworkElementAdapters.ContractToViewAdapter(contract);
                    IsContract = true;
                }
            }
            if (Control != null) {
                if (CurrentWindow != null) {
                    WindowStack.Push(CurrentWindow);
                }

                if (IsContract) {
                    Control = new AirspaceDecorator() {
                        AirspaceMode = AirspaceMode.Redirect,
                        IsInputRedirectionEnabled = true,
                        IsOutputRedirectionEnabled = true,
                        Background = Brushes.White,
                        Content = Control
                    };
                }

                CurrentWindow = window;
                CurrentWindow.OnShow();
                //MainWindow.transitionLayer.Transition = IsCurrentContract || IsContract ? TransitionType.Normal : DefaultTransition;
                MainWindow.transitionLayer.Content = Control;
                IsCurrentContract = IsContract;
            }
        }

        public void GoHome() {
            IWindow homeWindow = CurrentWindow;
            while (WindowStack.Count > 0) {
                WindowStack.TryPop(out homeWindow);
            }
            this.CurrentWindow = null;
            ShowWindow(homeWindow);
        }

        public void GoBack() {
            if (WindowStack.Count > 0) {
                IWindow previous;
                WindowStack.TryPop(out previous);
                this.CurrentWindow = null;
                ShowWindow(previous);
            }
        }

        public void GoBack(IWindow window) {
            if (window == null) {
                throw new ArgumentException("Window argument cannot be null");
            }
            if (window.Equals(CurrentWindow) && WindowStack.Count > 0) {
                WindowStack.TryPop(out window);
                this.CurrentWindow = null;
                ShowWindow(window);
            }
        }

        public void AddMenuItem(SDK.Management.Windows.MenuItem menuItem) {
            if (!MainMenuSeparatorAdded) {
                MenuItems.Add(SDK.Management.Windows.MenuItem.Separator);
                MainMenuSeparatorAdded = true;
            }
            MenuItems.Add(menuItem);
        }

        public bool RemoveMenuItem(SDK.Management.Windows.MenuItem menuItem) {
            if (OwnedItems.Contains(menuItem)) {
                throw new ArgumentException("You are not allowed to remove default MenuItem");
            }
            return MenuItems.Remove(menuItem);
        }

        public void AddPageItem(PageItem pageItem) {
            PageItems.Add(pageItem);
        }

        public bool RemovePageItem(PageItem pageItem) {
            if (OwnedItems.Contains(pageItem)) {
                throw new ArgumentException("You are not allowed to remove default PageItem");
            }
            return PageItems.Remove(pageItem);
        }

        public T FindResource<T>(string name) {
            return (T)MainWindow.FindResource(name);
        }

        #region Event handlers

        private void OnProfileLocked(object sender, LockedEventArgs e) {
            foreach (SDK.Management.Windows.MenuItem item in MenuItems) {
                item.NotifyEnabled();
            }
        }

        private void OnFileSystemLocked(object sender, LockedEventArgs e) {
            if (Personalization == null) {
                return;
            }
            if (e.IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                Personalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (ConfigurationManager.CheckGame(ProfileManager.CurrentProfile.GameModel)) {
                    Personalization.IsEnabled = true;
                }
            }
        }

        private void OnProfileChanged(object sender, BaseEventArgs e) {
            GameModel model = ProfileManager.CurrentProfile.GameModel;
            bool gameAvailable = ConfigurationManager.CheckGame(model);

            if (Community != null) {
                Community.IsEnabled = ConfigurationManager.GetConfiguration(model).IsWebAvailable;
            }
            if (Gallery != null) {
                Gallery.IsEnabled = gameAvailable;
            }
            if (Personalization != null) {
                Personalization.IsEnabled = gameAvailable;
            }
        }

        #endregion Event handlers
    }
}