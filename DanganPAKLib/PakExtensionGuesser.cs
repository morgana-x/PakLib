using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganPAKLib
{
    internal class PakExtensionGuesser
    {
        public static string GetMagicID(ref byte[] BodyFile)
        {
            string NewFileExtension;

            if (BodyFile[0] == 0xFF && BodyFile[1] == 0xFE) // = Text file.
            {
                NewFileExtension = ".unknown";
            }
            // Check the beginning of the file with the purpose of discovering its extension.
            else if (BodyFile[0] == 0xF0 && BodyFile[1] == 0x30 && BodyFile[2] == 0x60 && BodyFile[3] == 0x90) //I'm not sure about what is this.
            {
                Array.Copy(BodyFile, 4, BodyFile, 0, BodyFile.Length - 4);
                NewFileExtension = ".pak";
            }
            // ".gx3" files are doubly compressed files for PSVITA.
            else if (BodyFile.Length > 16 && ((BodyFile[13] == 0x47 && BodyFile[14] == 0x58 && BodyFile[15] == 0x33 && BodyFile[16] == 0) || (BodyFile[14] == 0x47 && BodyFile[15] == 0x58 && BodyFile[16] == 0x33 && BodyFile[17] == 0)))
            {
                if ((BodyFile[30] == 0x47 && BodyFile[31] == 0x58 && BodyFile[32] == 0x54) || (BodyFile[31] == 0x47 && BodyFile[32] == 0x58 && BodyFile[33] == 0x54))
                {
                    // ".gxt" is an image format used for PSVITA.
                    NewFileExtension = ".gx3.gxt";
                }
                else if (BodyFile[30] == 0x53 && BodyFile[31] == 0x48 && BodyFile[32] == 0x54 && BodyFile[33] == 0x58 && BodyFile[34] == 0x46)
                {
                    // ".SHTXF?" is an image format used for PSVITA.
                    NewFileExtension = ".gx3.SHTXF" + Convert.ToChar(BodyFile[35]) + ".btx";
                }
                else if (BodyFile[31] == 0x53 && BodyFile[32] == 0x48 && BodyFile[33] == 0x54 && BodyFile[34] == 0x58 && BodyFile[35] == 0x46)
                {
                    // ".SHTXF?" is an image format used for PSVITA.
                    NewFileExtension = ".gx3.SHTXF" + Convert.ToChar(BodyFile[36]) + ".btx";
                }
                else if ((BodyFile[30] == 0x53 && BodyFile[31] == 0x48 && BodyFile[32] == 0x54 && BodyFile[33] == 0x58) || (BodyFile[31] == 0x53 && BodyFile[32] == 0x48 && BodyFile[33] == 0x54 && BodyFile[34] == 0x58))
                {
                    // ".SHTX" is an image format used for PSVITA.
                    NewFileExtension = ".gx3.SHTX.btx";
                }
                else if ((BodyFile[30] == 0x44 && BodyFile[31] == 0x44 && BodyFile[32] == 0x53 && BodyFile[33] == 0x31) || (BodyFile[31] == 0x44 && BodyFile[32] == 0x44 && BodyFile[33] == 0x53 && BodyFile[34] == 0x31))
                {
                    // ".dds"  is an image format used for AE (PC).
                    NewFileExtension = ".gx3.dds.btx";
                }
                else
                {
                    NewFileExtension = ".gx3";
                }
            }
            // The ".cmp" files are compressed files.
            else if (BodyFile[0] == 0xFC && BodyFile[1] == 0xAA && BodyFile[2] == 0x55 && BodyFile[3] == 0xA7)
            {
                if ((BodyFile[13] == 0x47 && BodyFile[14] == 0x58 && BodyFile[15] == 0x54) || (BodyFile[14] == 0x47 && BodyFile[15] == 0x58 && BodyFile[16] == 0x54))
                {
                    // ".gxt" is an image format used for PSVITA.
                    NewFileExtension = ".cmp.gxt";
                }
                else if (BodyFile[13] == 0x53 && BodyFile[14] == 0x48 && BodyFile[15] == 0x54 && BodyFile[16] == 0x58 && BodyFile[17] == 0x46)
                {
                    // ".SHTXF?" is an image format used for PSVITA.
                    NewFileExtension = ".cmp.SHTXF" + Convert.ToChar(BodyFile[18]) + ".btx";
                }
                else if (BodyFile[14] == 0x53 && BodyFile[15] == 0x48 && BodyFile[16] == 0x54 && BodyFile[17] == 0x58 && BodyFile[18] == 0x46)
                {
                    // ".SHTXF?" is an image format used for PSVITA.
                    NewFileExtension = ".cmp.SHTXF" + Convert.ToChar(BodyFile[19]) + ".btx";
                }
                else if (BodyFile[13] == 0x53 && BodyFile[14] == 0x48 && BodyFile[15] == 0x54 && BodyFile[16] == 0x58)
                {
                    // ".SHTX" is an image format used for PSVITA.
                    NewFileExtension = ".cmp.SHTX.btx";
                }
                else if ((BodyFile[13] == 0x44 && BodyFile[14] == 0x44 && BodyFile[15] == 0x53 && BodyFile[16] == 0x31) || (BodyFile[14] == 0x44 && BodyFile[15] == 0x44 && BodyFile[16] == 0x53 && BodyFile[17] == 0x31))
                {
                    // ".dds"  is an image format used for AE (PC).
                    NewFileExtension = ".cmp.dds.btx";
                }
                else
                {
                    NewFileExtension = ".cmp";
                }
            }
            else if (BodyFile[0] == 0x4C && BodyFile[1] == 0x4C && BodyFile[2] == 0x46 && BodyFile[3] == 0x53)
            {
                NewFileExtension = ".llfs";
            }
            else if (BodyFile[0] == 0x4F && BodyFile[1] == 0x4D && BodyFile[2] == 0x47 && BodyFile[3] == 0x2E && BodyFile[4] == 0x30 && BodyFile[5] == 0x30)
            {
                NewFileExtension = ".gmo";
            }
            else if (BodyFile[0] == 0x47 && BodyFile[1] == 0x58 && BodyFile[2] == 0x54)
            {
                // ".gxt" is an image format used for PSVITA.
                NewFileExtension = ".gxt";
            }
            else if (BodyFile[0] == 0x53 && BodyFile[1] == 0x48 && BodyFile[2] == 0x54 && BodyFile[3] == 0x58 && BodyFile[4] == 0x46)
            {
                //  There are three types of ".btx" ".SHTX", ".SHTXFS" and ".SHTXFF". This one is used for AE (PSVITA).
                NewFileExtension = ".SHTXF" + Convert.ToChar(BodyFile[5]) + ".btx";
            }
            else if (BodyFile[0] == 0x53 && BodyFile[1] == 0x48 && BodyFile[2] == 0x54 && BodyFile[3] == 0x58)
            {
                NewFileExtension = ".SHTX.btx";
            }
            else if (BodyFile[0] == 0x74 && BodyFile[1] == 0x46 && BodyFile[2] == 0x70 && BodyFile[3] == 0x53)
            {
                // Files.font descrives the font. Letters position, height, ect...
                NewFileExtension = ".font";
            }
            else if (BodyFile[0] == 0x4D && BodyFile[1] == 0x49 && BodyFile[2] == 0x47 && BodyFile[3] == 0x2E && BodyFile[4] == 0x30 && BodyFile[5] == 0x30)
            {
                // ".gim"  is an image format used for PSP.
                NewFileExtension = ".gim";
            }
            else if (BodyFile[0] == 0x44 && BodyFile[1] == 0x44 && BodyFile[2] == 0x53 && BodyFile[3] == 0x31)
            {
                // ".dds"  is an image format used for AE (PC).
                NewFileExtension = ".dds";
            }
            else if ((BodyFile[0] == 0x01 || BodyFile[0] == 0x02) && BodyFile[1] == 0x00 && BodyFile[2] == 0x00 && BodyFile[3] == 0x00 && (BodyFile[4] == 0x10 || BodyFile[4] == 0x0C) && BodyFile[5] == 0x00 && BodyFile[6] == 0x00 && BodyFile[7] == 0x00 && BodyFile[16] == 0x70)
            {
                NewFileExtension = ".lin";
            }
            else if (BodyFile.Length < 0x70 || (BodyFile[0] == 0xFF && BodyFile[1] == 0xFE))
            {
                NewFileExtension = ".unknown";
            }
            else if ((BodyFile[0] == 0x00 && BodyFile[1] == 0x01 && BodyFile[2] == 0x01 && BodyFile[3] == 0x00) || (BodyFile[0] == 0x00 && BodyFile[1] == 0x00 && BodyFile[2] == 0x02 && BodyFile[3] == 0x00) || (BodyFile[0] == 0x01 && BodyFile[1] == 0x00 && BodyFile[2] == 0x02 && BodyFile[3] == 0x00) || (BodyFile[0] == 0x00 && BodyFile[1] == 0x00 && BodyFile[2] == 0x0A && BodyFile[3] == 0x00) || (BodyFile[0] == 0x00 && BodyFile[1] == 0x01 && BodyFile[2] == 0x09 && BodyFile[3] == 0x00) || (BodyFile[0] == 0x00 && BodyFile[1] == 0x00 && BodyFile[2] == 0x0B && BodyFile[3] == 0x00))
            {
                NewFileExtension = ".tga";
            }
            else if ((BodyFile[0] == 0x42 && BodyFile[1] == 0x4D))
            {
                NewFileExtension = ".bmp";
            }
            else if ((BodyFile[0] != 0x00 || BodyFile[1] != 0x00) && BodyFile[2] == 0x00 && BodyFile[3] == 0x00 && (BodyFile[4] != 0x00 || BodyFile[5] != 0x00) && BodyFile[6] == 0x00 && BodyFile[7] == 0x00)
            {
                NewFileExtension = ".pak";
            }
            else
            {
                NewFileExtension = ".unknown";
            }

            return NewFileExtension;
        }
    }
}
