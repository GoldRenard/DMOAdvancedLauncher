// ======================================================================
// DMO KEYBOARD LAYOUT CHANGER SERVICE
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace KBLCService {

    /// <summary>
    /// Служба хоткея
    /// </summary>
    public class HotkeyService {
        private HotkeyHook Hook = new HotkeyHook();

        private static string[] WindowTitles = new string[] { "DMO", "DigimonMastersOnline" };

        private Window HookWindow = new Window() {
            Width = 0,
            Height = 0,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false
        };

        private BackgroundWorker Worker = new BackgroundWorker() {
            WorkerSupportsCancellation = true
        };

        private bool IsControl = false;
        private bool IsAttach = false;
        private bool IsStarted = false;

        public HotkeyService() {
            Hook.KeyPressed += new EventHandler<HotKeyEventArgs>(HotkeyHandler);
            Worker.DoWork += WorkerBody;
            Worker.RunWorkerCompleted += WorkerCompleted;
        }

        /// <summary>
        /// Тело воркера
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы воркера</param>
        private void WorkerBody(object sender, DoWorkEventArgs e) {
            this.IsStarted = true;
            if (IsAttach) {
                String windowTitle = null;

                const int findInterval = 500;              // 500мс интервал поиска окна
                const int checkInterval = 10000;           // 10сек интервал наличия окна
                int findTimeout = 60000 / findInterval;   // 60 секунд таймаут поиска окна

                // Ожидание обнаружения окна
                while (findTimeout > 0) {
                    // Проверяем запрос остановки
                    if (Worker.CancellationPending) {
                        return;
                    }

                    IntPtr hWnd = IntPtr.Zero;
                    foreach (string title in WindowTitles) {
                        hWnd = WinAPI.FindWindow(null, title);
                        if (hWnd != IntPtr.Zero) {
                            windowTitle = title;
                            break;
                        }
                    }
                    if (hWnd != IntPtr.Zero) {
                        break;
                    }
                    Thread.Sleep(findInterval);
                    findTimeout--;
                }

                // Если окно до сих пор не было обнаружено, закрываем воркер
                if (windowTitle == null) {
                    return;
                }

                // Регистрируем обработчики кнопок
                SetMode(IsControl);

                // Проверяем наличие окна
                while (true) {
                    // Проверяем запрос остановки
                    if (Worker.CancellationPending) {
                        return;
                    }

                    if (WinAPI.FindWindow(null, windowTitle) == IntPtr.Zero) {
                        break;
                    }
                    Thread.Sleep(checkInterval);
                }
            } else {
                // Регистрируем обработчики кнопок
                SetMode(IsControl);
            }
        }

        /// <summary>
        /// Если воркер завершен и был режим прикрепления к окну, завершаем приложение
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Аргументы воркера</param>
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.IsStarted = false;
            Hook.UnregisterHotKeys();
            if (IsAttach) {
                Utils.CloseApp();
            }
        }

        /// <summary>
        /// Старт воркера
        /// </summary>
        /// <param name="IsAttach">Если True, приложение будет следить за окном. Если окно закрывается, приложение тоже закрывается.</param>
        /// <param name="IsControl">True если CTRL+SHIFT, False если ALT+SHIFT</param>
        public void Start(bool IsAttach, bool IsControl = false) {
            this.IsAttach = IsAttach;
            this.IsControl = IsControl;
            Worker.RunWorkerAsync();
        }

        /// <summary>
        /// Остановка воркера
        /// </summary>
        public void Stop() {
            Worker.CancelAsync();
        }

        /// <summary>
        /// Обработчик хука
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры</param>
        private void HotkeyHandler(object sender, HotKeyEventArgs e) {
            IntPtr hWnd = WinAPI.GetForegroundWindow();

            // Check window titles
            if (!WindowTitles.Contains(WinAPI.GetWindowTitle(hWnd))) {
                return;
            }

            if ((IsControl == true && e.IsControlHotkey) || (IsControl == false && e.IsAltHotkey)) {
                // Если 8ка, то меняем раскладку костылём с окном, если ниже, то PostMessage нужному окну с WM_INPUTLANGCHANGEREQUEST
                if (Utils.IsWindows8OrNewer()) {
                    // посылаем ивент смены раскладки в активное окно
                    WinAPI.ActivateKeyboardLayout(WinAPI.HKL_NEXT, 0);

                    // скрываем таскбар
                    WinAPI.ShowWindow(WinAPI.TaskBarHandle, WinAPI.SW_HIDE);
                    System.Threading.Thread.Sleep(50);

                    // отображаем окно хука
                    HookWindow.Show();
                    HookWindow.Activate();
                    System.Threading.Thread.Sleep(50);

                    // посылаем ивент смены раскладки
                    WinAPI.ActivateKeyboardLayout(WinAPI.HKL_NEXT, 0);

                    //скрываем окно хука и отображаем обратно таскбар
                    HookWindow.Hide();
                    System.Threading.Thread.Sleep(50);
                    WinAPI.ShowWindow(WinAPI.TaskBarHandle, WinAPI.SW_SHOW);

                    // Возвращаем фокус окну
                    WinAPI.SetForegroundWindow(hWnd);
                } else {
                    WinAPI.PostMessage(hWnd, WinAPI.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, WinAPI.ActivateKeyboardLayout(WinAPI.HKL_NEXT, 0));
                }
            }
        }

        /// <summary>
        /// Установка режима хоткея
        /// </summary>
        /// <param name="IsControl">True если CTRL+SHIFT, False если ALT+SHIFT</param>
        public void SetMode(bool IsControl) {
            this.IsControl = IsControl;
            if (!IsStarted) {
                return;
            }

            if (Application.Current.Dispatcher.CheckAccess()) {
                try {
                    if (IsControl) {
                        Hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, 0);
                    } else {
                        Hook.RegisterHotKey(ModifierKeys.Alt | ModifierKeys.Shift, 0);
                    }
                } catch (InvalidOperationException ex) {
                    MessageBox.Show(ex.Message, "DMO Keyboard Layout Changer", MessageBoxButton.OK, MessageBoxImage.Error);
                    Utils.CloseApp();
                }
            } else {
                Application.Current.Dispatcher.BeginInvoke(((Action)(() => {
                    SetMode(IsControl);
                })));
            }
        }
    }
}