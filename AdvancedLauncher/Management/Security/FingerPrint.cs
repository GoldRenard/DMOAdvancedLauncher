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
using System.Security.Cryptography;
using System.Text;

namespace AdvancedLauncher.Management.Security {

    /// <summary>
    /// Generates a 16 byte Unique Identification code of a computer
    /// Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
    /// </summary>
    internal sealed class FingerPrint {

        [Flags]
        public enum FingerPart {
            CPU = 1,
            BIOS = 2,
            BASE = 4,
            UUID = 8,
            VIDEO = 16,
            MAC = 32,
            DISK = 64
        }

        private static string fprint = string.Empty;

        public static string Value(FingerPart p, bool IsHashed) {
            if (string.IsNullOrEmpty(fprint)) {
                fprint = string.Empty;
                if (p.HasFlag(FingerPart.CPU))
                    fprint += cpuId();
                if (p.HasFlag(FingerPart.BIOS))
                    fprint += biosId();
                if (p.HasFlag(FingerPart.BASE))
                    fprint += baseId();
                if (p.HasFlag(FingerPart.UUID))
                    fprint += uuId();
                if (p.HasFlag(FingerPart.VIDEO))
                    fprint += videoId();
                if (p.HasFlag(FingerPart.MAC))
                    fprint += macId();
                if (p.HasFlag(FingerPart.DISK))
                    fprint += diskId();
                if (IsHashed)
                    fprint = GetHash(fprint);
            }
            return fprint;
        }

        private static string GetHash(string s) {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(byte[] bt) {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++) {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }

        #region Original Device ID Getting Code

        //Return a hardware identifier
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue) {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc) {
                if (mo[wmiMustBeTrue].ToString() == "True") {
                    //Only get the first one
                    if (result == "") {
                        try {
                            result = mo[wmiProperty].ToString();
                            break;
                        } catch {
                        }
                    }
                }
            }
            return result;
        }

        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty) {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc) {
                //Only get the first one
                if (result == "") {
                    try {
                        result = mo[wmiProperty].ToString();
                        break;
                    } catch {
                    }
                }
            }
            return result;
        }

        private static string cpuId() {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (String.Empty.Equals(retVal)) {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (String.Empty.Equals(retVal)) {
                    retVal = identifier("Win32_Processor", "Name");
                    if (String.Empty.Equals(retVal)) { //If no Name, use Manufacturer
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }

        //BIOS Identifier
        private static string biosId() {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }

        //Main physical hard drive ID
        private static string diskId() {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }

        //Motherboard ID
        private static string baseId() {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }

        //Motherboard ID
        private static string uuId() {
            return identifier("Win32_ComputerSystemProduct", "UUID");
        }

        //Primary video controller ID
        private static string videoId() {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }

        //First enabled network card ID
        private static string macId() {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }

        #endregion Original Device ID Getting Code
    }
}