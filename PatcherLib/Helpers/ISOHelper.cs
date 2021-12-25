﻿using PatcherLib.Datatypes;
using PatcherLib.Iso;
using System;

namespace PatcherLib.Helpers
{
    public static class ISOHelper
    {
        public static string GetSectorName(Enum sector)
        {
            Type type = sector.GetType();

            if (type == typeof(PsxIso.Sectors))
                return PsxIso.GetSectorName((PsxIso.Sectors)sector);
            else if (type == typeof(PspIso.Sectors))
                return PspIso.GetSectorName((PspIso.Sectors)sector);
            else if (type == typeof(FFTPack.Files))
                return PspIso.GetFileName((FFTPack.Files)sector);
            else
                return string.Empty;
        }

        public static string GetSectorName(int sector, Context context)
        {
            return GetSectorName(GetSector(sector, context));
        }

        public static Enum GetSector(string sectorName, Context context)
        {
            if (context == Context.US_PSP)
            {
                PspIso.Sectors pspSector;
                if (!Utilities.Utilities.TryParseEnum<PspIso.Sectors>(sectorName, out pspSector))
                {
                    FFTPack.Files fftpackSector;
                    if (!Utilities.Utilities.TryParseEnum<FFTPack.Files>(sectorName, out fftpackSector))
                        throw new ArgumentException("Invalid sector name!");
                    else
                        return fftpackSector;
                }
                else
                    return pspSector;
            }
            else
            {
                PsxIso.Sectors psxSector;
                if (!Utilities.Utilities.TryParseEnum<PsxIso.Sectors>(sectorName, out psxSector))
                    throw new ArgumentException("Invalid sector name!");
                else
                    return psxSector;
            }
        }

        public static Enum GetSectorHex(string hex, Context context)
        {
            return GetSector(Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber), context);
        }

        public static Enum GetSector(int value, Context context)
        {
            return (Enum)Enum.ToObject(GetSectorType(context), value);
        }

        public static Type GetSectorType(Context context)
        {
            return (context == Context.US_PSP) ? typeof(PspIso.Sectors) : typeof(PsxIso.Sectors);
        }

        public static int GetFileToRamOffset(Enum sector, Context context)
        {
            Type type = sector.GetType();

            if (type == typeof(PsxIso.Sectors))
                return PsxIso.GetRamOffset((PsxIso.Sectors)sector);
            else if (type == typeof(PspIso.Sectors))
                return PspIso.GetRamOffset((PspIso.Sectors)sector);
            else
                return -1;
        }

        public static string GetModifiedPathName(string name)
        {
            int backslashIndex = name.IndexOf('_');
            int dotIndex = name.LastIndexOf('_');

            if (backslashIndex == dotIndex)
            {
                return name.Replace('_', '.');
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(name.Length);
                sb.Append(name.Substring(0, backslashIndex));
                sb.Append(@"\");
                sb.Append(name.Substring(backslashIndex + 1, dotIndex - backslashIndex - 1));
                sb.Append(".");
                sb.Append(name.Substring(dotIndex + 1));
                return sb.ToString();
            }

            //return name.Remove(backslashIndex).Insert(backslashIndex, @"\").Remove(dotIndex).Insert(dotIndex, ".");
        }
    }
}
