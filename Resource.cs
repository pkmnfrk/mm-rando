﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MMRando
{

    public partial class ROMFuncs
    {

        public static void ApplyHack_File(string name, byte[] data)
        {
            BinaryReader hack_file = new BinaryReader(File.Open(name, FileMode.Open));
            int hack_len = (int)hack_file.BaseStream.Length;
            byte[] hack_content = new byte[hack_len];
            hack_file.Read(hack_content, 0, hack_len);
            hack_file.Close();
            int addr = 0;
            while (hack_content[addr] != 0xFF)
            {
                //Debug.WriteLine(addr.ToString("X4"));
                uint dest = Arr_ReadU32(hack_content, addr);
                addr += 4;
                uint len = Arr_ReadU32(hack_content, addr);
                addr += 4;
                Arr_Insert(hack_content, addr, (int)len, data, (int)dest);
                addr += (int)len;
            };
        }

        public static void ApplyHack(string name)
        {
            BinaryReader hack_file = new BinaryReader(File.Open(name, FileMode.Open));
            int hack_len = (int)hack_file.BaseStream.Length;
            byte[] hack_content = new byte[hack_len];
            hack_file.Read(hack_content, 0, hack_len);
            hack_file.Close();
            if (name.EndsWith("title-screen"))
            {
                Random R = new Random();
                int rot = R.Next(360);
                Color l;
                float h;
                for (int i = 0; i < 144 * 64; i++)
                {
                    int p = (i * 4) + 8;
                    l = Color.FromArgb(hack_content[p + 3], hack_content[p], hack_content[p + 1], hack_content[p + 2]);
                    h = l.GetHue();
                    h += rot;
                    h %= 360f;
                    l = FromAHSB(l.A, h, l.GetSaturation(), l.GetBrightness());
                    hack_content[p] = l.R;
                    hack_content[p + 1] = l.G;
                    hack_content[p + 2] = l.B;
                    hack_content[p + 3] = l.A;
                };
                l = Color.FromArgb(hack_content[0x1FE72], hack_content[0x1FE73], hack_content[0x1FE76]);
                h = l.GetHue();
                h += rot;
                h %= 360f;
                l = FromAHSB(255, h, l.GetSaturation(), l.GetBrightness());
                hack_content[0x1FE72] = l.R;
                hack_content[0x1FE73] = l.G;
                hack_content[0x1FE76] = l.B;
            };
            int addr = 0;
            while (hack_content[addr] != 0xFF)
            {
                //Debug.WriteLine(addr.ToString("X4"));
                uint dest = Arr_ReadU32(hack_content, addr);
                addr += 4;
                uint len = Arr_ReadU32(hack_content, addr);
                addr += 4;
                int f = AddrToFile(dest);
                dest -= (uint)MMFileList[f].Addr;
                CheckCompressed(f);
                Arr_Insert(hack_content, addr, (int)len, MMFileList[f].Data, (int)dest);
                addr += (int)len;
            };
        }

        public static List<int[]> GetAddresses(string name)
        {
            List<int[]> Addrs = new List<int[]>();
            BinaryReader AddrFile = new BinaryReader(File.Open(name, FileMode.Open));
            byte[] a = new byte[AddrFile.BaseStream.Length];
            AddrFile.Read(a, 0, a.Length);
            AddrFile.Close();
            int i = 0;
            while (a[i] != 0xFF)
            {
                int count = (int)Arr_ReadU32(a, i);
                int[] alist = new int[count];
                i += 4;
                for (int j = 0; j < count; j++)
                {
                    alist[j] = (int)Arr_ReadU32(a, i);
                    i += 4;
                };
                Addrs.Add(alist);
            };
            return Addrs;
        }

    }

}