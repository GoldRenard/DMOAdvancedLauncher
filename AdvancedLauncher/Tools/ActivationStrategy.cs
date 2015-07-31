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
using AdvancedLauncher.UI.Windows;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Components;

namespace AdvancedLauncher.Tools {

    internal class ActivationStrategy : NinjectComponent, IActivationStrategy {

        public void Activate(IContext context, InstanceReference reference) {
            Type instanceType = reference.Instance.GetType();
            if (!instanceType.IsAssignableFrom(typeof(Splashscreen))) {
                Splashscreen splashscreen = App.Kernel.Get<Splashscreen>();
                splashscreen.SetProgress(string.Format("{0} loading...", instanceType.Name));
            }
        }

        public void Deactivate(IContext context, InstanceReference reference) {
            // nothing to do
        }
    }
}