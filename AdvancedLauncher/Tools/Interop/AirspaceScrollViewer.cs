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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdvancedLauncher.Tools.Interop {

    /// <summary>
    ///     A scrollviewer that uses an AirspaceDecorator to contain the
    ///     ScrollContentPresenter.
    /// </summary>
    /// <remarks>
    ///     All of the interesting details are in the template for this class.
    ///     The template uses an AirspaceDecorator to contain the
    ///     ScrollContentPresenter for the ScrollViewer.
    /// </remarks>
    public class AirspaceScrollViewer : ScrollViewer {

        static AirspaceScrollViewer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AirspaceScrollViewer), new FrameworkPropertyMetadata(typeof(AirspaceScrollViewer)));
        }

        #region AirspaceMode

        public static readonly DependencyProperty AirspaceModeProperty = DependencyProperty.Register(
            /* Name:                 */ "AirspaceMode",
            /* Value Type:           */ typeof(AirspaceMode),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ AirspaceMode.None));

        /// <summary>
        ///     The airspace mode for this instance.
        /// </summary>
        public AirspaceMode AirspaceMode {
            get {
                return (AirspaceMode)GetValue(AirspaceModeProperty);
            }
            set {
                SetValue(AirspaceModeProperty, value);
            }
        }

        #endregion AirspaceMode

        #region ClippingBackground

        public static readonly DependencyProperty ClippingBackgroundProperty = DependencyProperty.Register(
            /* Name:                 */ "ClippingBackground",
            /* Value Type:           */ typeof(Brush),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ null));

        /// <summary>
        ///     The brush to paint the background when the airspace mode is
        ///     set to clipping.
        /// </summary>
        public Brush ClippingBackground {
            get {
                return (Brush)GetValue(ClippingBackgroundProperty);
            }
            set {
                SetValue(ClippingBackgroundProperty, value);
            }
        }

        #endregion ClippingBackground

        #region ClippingCopyBitsBehavior

        public static readonly DependencyProperty ClippingCopyBitsBehaviorProperty = DependencyProperty.Register(
            /* Name:                 */ "ClippingCopyBitsBehavior",
            /* Value Type:           */ typeof(CopyBitsBehavior),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ CopyBitsBehavior.Default));

        /// <summary>
        ///     The behavior of copying bits when the airspace mode is set to
        ///     clipping.
        /// </summary>
        public CopyBitsBehavior ClippingCopyBitsBehavior {
            get {
                return (CopyBitsBehavior)GetValue(ClippingCopyBitsBehaviorProperty);
            }
            set {
                SetValue(ClippingCopyBitsBehaviorProperty, value);
            }
        }

        #endregion ClippingCopyBitsBehavior

        #region RedirectionVisibility

        public static readonly DependencyProperty RedirectionVisibilityProperty = DependencyProperty.Register(
            /* Name:                 */ "RedirectionVisibility",
            /* Value Type:           */ typeof(RedirectionVisibility),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ RedirectionVisibility.Hidden));

        /// <summary>
        ///     The visibility of the redirection surface.
        /// </summary>
        public RedirectionVisibility RedirectionVisibility {
            get {
                return (RedirectionVisibility)GetValue(RedirectionVisibilityProperty);
            }
            set {
                SetValue(RedirectionVisibilityProperty, value);
            }
        }

        #endregion RedirectionVisibility

        #region IsOutputRedirectionEnabled

        public static readonly DependencyProperty IsOutputRedirectionEnabledProperty = DependencyProperty.Register(
            /* Name:                 */ "IsOutputRedirectionEnabled",
            /* Value Type:           */ typeof(bool),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ false));

        /// <summary>
        ///     Whether or not output redirection is enabled.
        /// </summary>
        public bool IsOutputRedirectionEnabled {
            get {
                return (bool)GetValue(IsOutputRedirectionEnabledProperty);
            }
            set {
                SetValue(IsOutputRedirectionEnabledProperty, value);
            }
        }

        #endregion IsOutputRedirectionEnabled

        #region OutputRedirectionPeriod

        public static readonly DependencyProperty OutputRedirectionPeriodProperty = DependencyProperty.Register(
            /* Name:                 */ "OutputRedirectionPeriod",
            /* Value Type:           */ typeof(TimeSpan),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ TimeSpan.FromMilliseconds(30)));

        /// <summary>
        ///     The period of time to update the output redirection.
        /// </summary>
        public TimeSpan OutputRedirectionPeriod {
            get {
                return (TimeSpan)GetValue(OutputRedirectionPeriodProperty);
            }
            set {
                SetValue(OutputRedirectionPeriodProperty, value);
            }
        }

        #endregion OutputRedirectionPeriod

        #region IsInputRedirectionEnabled

        public static readonly DependencyProperty IsInputRedirectionEnabledProperty = DependencyProperty.Register(
            /* Name:                 */ "IsInputRedirectionEnabled",
            /* Value Type:           */ typeof(bool),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ false));

        /// <summary>
        ///     Whether or not input redirection is enabled.
        /// </summary>
        public bool IsInputRedirectionEnabled {
            get {
                return (bool)GetValue(IsInputRedirectionEnabledProperty);
            }
            set {
                SetValue(IsInputRedirectionEnabledProperty, value);
            }
        }

        #endregion IsInputRedirectionEnabled

        #region InputRedirectionPeriod

        public static readonly DependencyProperty InputRedirectionPeriodProperty = DependencyProperty.Register(
            /* Name:                 */ "InputRedirectionPeriod",
            /* Value Type:           */ typeof(TimeSpan),
            /* Owner Type:           */ typeof(AirspaceScrollViewer),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ TimeSpan.FromMilliseconds(30)));

        /// <summary>
        ///     The period of time to update the input redirection.
        /// </summary>
        public TimeSpan InputRedirectionPeriod {
            get {
                return (TimeSpan)GetValue(InputRedirectionPeriodProperty);
            }
            set {
                SetValue(InputRedirectionPeriodProperty, value);
            }
        }

        #endregion InputRedirectionPeriod
    }
}