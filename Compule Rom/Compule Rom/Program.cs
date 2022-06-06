using System;
using System.IO;
using System.Threading.Tasks;


namespace Compile_Rom
{
    class Program
    {
        // micro ops
        static UInt16 INTCLEAR =    0x0001;
        static UInt16 REGWRITE =    0x0002;
        static UInt16 REGADDR =     0x0004;
        static UInt16 MEMW =        0x0008;
        static UInt16 MEMR =        0x0010;
        static UInt16 x06 =       0x0020;
        static UInt16 x07 =         0x0040;
        static UInt16 x08 =         0x0080;
        static UInt16 x09 =         0x0100;
        static UInt16 x10 =         0x0200;
        static UInt16 x11 =         0x0400;
        static UInt16 x12 =         0x0800;
        static UInt16 x13 =         0x1000;
        static UInt16 x14 =    0x2000;
        static UInt16 IRLOAD =      0x4000;
        static UInt16 x16 =         0x8000;




        static Object[,] rom = new Object[16, 3] {
            { REGWRITE,                  MEMR|IRLOAD, "MOV r{{dst}}, r{{src1}}, {{c:cond}} => 0x0`3 @ src1`3 @ c`3 @ dst`3 @ 0x{0}" },                  // 0000 MOV Reg to reg 0010
            { REGADDR|MEMR|REGWRITE,     MEMR|IRLOAD, "LOAD r{{dst}}, r{{src1}}, {{c:cond}} => 0x0`3 @ src1`3 @ c`3 @ dst`3 @ 0x{0}" },                 // 0001 LOAD Reg from memory 0011
            { REGWRITE,                  MEMR|IRLOAD, "MOV r{{dst}}, {{value}} => value`9 @ dst`3 @ 0x{0}" },                                           // 0010 MOV Immediate into Reg  0100
            { REGADDR|MEMW,              MEMR|IRLOAD, "STOR r{{src1}}, r{{src2}}, {{c:cond}} => src2`3 @ src1`3 @ c`3 @ 0x0`3 @ 0x{0}" },               // 0011 STOR to memory 0101
            { REGWRITE,                  MEMR|IRLOAD, "ALUA r{{dst}}, r{{src1}}, r{{src2}}, {{m: math}}  => src2`3 @ src1`3 @ m`3 @ 0x0`3 @ 0x{0}" },   // 0100 ALU Arithmetic
            { REGWRITE,                  MEMR|IRLOAD, "ALUL r{{dst}}, r{{src1}}, r{{src2}}, {{l: logic}}  => src2`3 @ src1`3 @ l`3 @ 0x0`3 @ 0x{0}" },  // 0101 ALU Logical/Shift 0001
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { 0, 0, "" },
            { REGWRITE,                  MEMR|IRLOAD, "NOP => 0x{0}" },         // 1110 NOP used for conditions
            { REGWRITE,                  MEMR|IRLOAD, "INT => 0x{0}" },         // 1111 INT Interrupt handler 0xEF (last 16 bytes or ROM)
        };

        static string[] romarray = new string[17]; // Plus one line for header

        static string[] asmarray = new string[17]; // Plus one line for header

        static void Main(string[] args)
        {
            // First line
            romarray[0] = "v3.0 hex words addressed";
            string romhexValue = "";
            ulong romopValue = 0x00;


            int asmruledefnumlines = 0;
            string asmhexValue = "";
            string asmrule = "";
            string rulehexValue = "";
            char pad = '0';
            for (int x = 0; x<16; x=x+1)             // Iterate through each line of the ROM
            {
                romhexValue = (x*8).ToString("X");                // Get the hex address that goes at the beginning of the line
                romarray[(x)+1] = romhexValue.PadLeft(3, pad) + ": ";    // Store the string in the array, add padding
                // Iterate through each row of the ROM
                for (int y = 0; y<2; y++)
                {
                    romopValue = Convert.ToUInt64(rom[x, y]);
                    asmhexValue = romopValue.ToString("X");
                    romarray[(x)+1] += asmhexValue.PadLeft(4, pad) + " ";
                    if ((y == 1) & (!Object.Equals(rom[x, y + 1].ToString(),"")))
                    {
                        asmrule = Convert.ToString(rom[x, y+1]);
                        rulehexValue = (x).ToString("X");
                        asmrule = string.Format(asmrule, rulehexValue.PadLeft(1, pad)); // no padding maybe?
                        asmarray[(asmruledefnumlines)] += asmrule;
                        asmruledefnumlines++;
                    }
                }
            }

            Array.Resize(ref asmarray, asmruledefnumlines);

            System.IO.File.WriteAllLines(@"C:\Users\zen\Documents\GitHub\Ranger\control.txt", romarray);
            System.IO.File.WriteAllLines(@"C:\Users\zen\Documents\GitHub\Ranger\asmdefine.txt", asmarray);
        }
    }
}
