﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace PersonaFont
{
    class Logging
    {
        public Logging(string FileName)
        {
            FileStream FS = new FileStream(FileName, FileMode.OpenOrCreate);
            FS.Position = FS.Length;
            LOG = new StreamWriter(FS);
            LOG.AutoFlush = true;
        }
        public Logging()
        {

        }
        StreamWriter LOG = new StreamWriter(new MemoryStream());

        public void W(string Line)
        {
            if (LOG != null)
            {
                if (string.IsNullOrEmpty(Line))
                    LOG.WriteLine();
                else
                    LOG.WriteLine(DateTime.Now + ": " + Line);
            }
        }
    }

    class Static
    {
        public static Logging LOG = new Logging();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("-----Font decompressor/compressor by Meloman19-----");
            Console.WriteLine("-------------------Persona 3/4/5-------------------");
            Console.WriteLine("----------Based on RikuKH3's decompressor----------");
            Console.WriteLine("---------------------------------------------------");
            string command = "";
            if (args.Length == 0)
            {
                check_command(ref command);
            }
            else if (args.Length == 1)
            {
                command = args[0];
            }
            else if (args.Length == 2)
            {
                command = args[0];
                if (args[1] == "-l")
                {
                    try
                    {
                        Static.LOG = new Logging("PersonaFont.log");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.ReadLine();
                    }
                }
            }

            if (command == "decom")
                decom();
            else if (command == "com")
                com();
            else
                Console.WriteLine("Unknown command");
        }

        private static bool check_command(ref
            string command)
        {
            while ((command != "decom") & (command != "com"))
            {
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("--Decompress [decom], Compress [com], Exit [exit]--");
                Console.WriteLine("---------------------------------------------------");
                Console.Write("Command: ");
                command = Console.ReadLine();
                Console.WriteLine("");
            }

            if (command == "exit")
            {
                return false;
            }
            if ((command == "decom") & (File.Exists(@"FONT0.FNT") == false))
            {
                Static.LOG.W("ERROR: " + "Missing 'FONT0.FNT'");
                Static.LOG.W("");
                Console.WriteLine("Missing 'FONT0.FNT'");
                Console.ReadKey();
                return false;
            }
            if (command == "com")
            {
                if (File.Exists(@"FONT0.FNT") == false)
                {
                    Static.LOG.W("ERROR: " + "Missing 'FONT0.FNT'");
                    Static.LOG.W("");
                    Console.WriteLine("Missing 'FONT0.FNT'");
                    Console.ReadKey();
                    return false;
                }

                if (File.Exists(@"FONT0 WIDTH TABLE.XML") == false)
                {
                    Static.LOG.W("ERROR: " + "Missing 'FONT0 WIDTH TABLE.XML'");
                    Static.LOG.W("");
                    Console.WriteLine("Missing 'FONT0 WIDTH TABLE.XML'");
                    Console.ReadKey();
                    return false;
                }

                if (File.Exists(@"FONT0.bmp") == false)
                {
                    Static.LOG.W("ERROR: " + "Missing 'FONT0.BMP'");
                    Static.LOG.W("");
                    Console.WriteLine("Missing 'FONT0.BMP'");
                    Console.ReadKey();
                    return false;
                }
            }

            return true;
        }

        private static void decom()
        {
            try
            {
                FileStream FONT = new FileStream(@"FONT0.FNT", FileMode.Open, FileAccess.Read);
                Font Add = new Font(FONT);

                FONT.Position = Add.GlyphCutTable_Pos;
                WidthTable.WriteToFile(FONT.ReadMemoryStream(Add.GlyphCutTable_Size));

                FONT.Position = Add.CompressedFontBlock_Pos;

                int temp = 0;
                for (int k = 0; k < Add.CompressedFontBlock_Size; k += 2)
                {
                    int s4 = FONT.ReadUshort();
                    for (int i = 0; i < 16; i++)
                    {
                        temp = Add.Dictionary[temp, s4 % 2];
                        s4 = s4 >> 1;

                        if (Add.Dictionary[temp, 0] == 0)
                        {

                            Add.FontDec.WriteByte((byte)(Add.Dictionary[temp, 1]));
                            temp = 0;
                        }
                    }
                }
                Static.LOG.W("Get decompressed font");

                FONT.Position = Add.MainHeaderSize;
                Add.Save2BMP(ref FONT);
                Console.WriteLine("Success");
            }
            catch (Exception e)
            {
                Static.LOG.W("ERROR: " + e.ToString());
                return;
            }
        }

        private static void com()
        {
            FileStream FONT = new FileStream(@"FONT0.FNT", FileMode.Open, FileAccess.Read);
            Static.LOG.W("Open FONT0.FNT");
            Font Add = new Font(FONT);

            try
            {
                MemoryStream FontDecRev = Add.FontDecRev();
                FileStream FONT_COMPRESS_FILE = new FileStream(@"FONT0 NEW.FNT", FileMode.Create);
                Static.LOG.W("Create \"FONT0 NEW.FNT\"");
                MemoryStream FONT_COMPRESS = new MemoryStream();

                int DictPart = Add.FindDictPart(Add.Dictionary);

                bool boolean = true;
                FontDecRev.Position = 0;

                while (boolean)
                {
                    if (FontDecRev.Position == FontDecRev.Length) { boolean = false; }
                    else
                    {
                        int s4 = FontDecRev.ReadByte();
                        int i = 1;

                        while (Add.Dictionary[i, 1] != s4)
                        {
                            i++;
                            if (Add.Dictionary[i - 1, 1] == 0)
                            {
                                if ((s4 >> 4) > ((s4 << 4) >> 4))
                                {
                                    s4 = s4 - (1 << 4);
                                }
                                else
                                {
                                    s4 = s4 - 1;
                                }
                                i = 1;
                            }
                        }
                        int v0 = i;
                        while (v0 != 0)
                        {
                            v0 = Add.FindDictIndex(v0, DictPart, ref FONT_COMPRESS);
                        }
                    }
                }
                Static.LOG.W("Image packed");

                FONT.Position = 0;
                while (FONT.Position < Add.CompressedFontBlock_Pos)
                {
                    FONT_COMPRESS_FILE.WriteByte((byte)FONT.ReadByte());
                }

                int GlyphSize = 0;
                do
                {
                    int i = 0;
                    string str = "";
                    while ((i < 8) & (FONT_COMPRESS.Position != 0))
                    {
                        FONT_COMPRESS.Position--;
                        str = Convert.ToString(FONT_COMPRESS.ReadByte()) + str;
                        FONT_COMPRESS.Position--;
                        i++;
                    }
                    str = str.PadLeft(8, '0');
                    FONT_COMPRESS_FILE.WriteByte(Convert.ToByte(str, 2));
                    GlyphSize++;
                } while (FONT_COMPRESS.Position != 0);

                FONT_COMPRESS_FILE.WriteByte(0);
                GlyphSize++;


                FONT_COMPRESS_FILE.Position = Add.DictionaryHeader_Pos + 8;
                FONT_COMPRESS_FILE.WriteInt(GlyphSize);

                Add.WriteGlyphPosition(FONT_COMPRESS_FILE);

                FONT_COMPRESS_FILE.Position = FONT_COMPRESS_FILE.Position - 4;
                int temp2 = FONT_COMPRESS_FILE.ReadInt();
                FONT_COMPRESS_FILE.Position = Add.DictionaryHeader_Pos + 12;
                FONT_COMPRESS_FILE.WriteInt(temp2);

                FONT_COMPRESS_FILE.Position = 0x4;
                FONT_COMPRESS_FILE.WriteInt(((int)FONT_COMPRESS_FILE.Length - (7 + Add.GlyphCutTable_Size + Add.UnknownSize)));

                //   File.Delete(@"GLYPH POSITION NEW");

                FONT_COMPRESS_FILE.Position = Add.GlyphCutTable_Pos;
                FONT_COMPRESS_FILE.WriteMemoryStream(WidthTable.WriteToFont());
                Static.LOG.W("Complete");
                Static.LOG.W("");
                Console.WriteLine("Success");
            }
            catch (Exception e)
            {
                Static.LOG.W("ERROR:" + e.ToString());
                return;
            }
        }
    }
}