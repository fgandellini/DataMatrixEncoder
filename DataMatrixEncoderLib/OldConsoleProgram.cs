using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Datamatrix;
using ZXing.Datamatrix.Encoder;
using ZXing.Rendering;

namespace DataMatrixEncoderLib
{
    class OldConsoleProgram
    {
        private static char FNC1
        {
            get
            {
                return (char)0x1D;
            }
        }

        static void Main(string[] args)
        {
            string currentDir =
                Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            string outputFile = Path.Combine(currentDir, "dm.png");

            Encoder encoder = new Encoder();
            ContentManager contentManager = new ContentManager();

            try
            {
                encoder
                    .SetSize(200)
                    //.SetContent(contentManager.Parse(args).ToString())
                    .Encode()
                    .Save(outputFile);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error Executing Command!");
                Console.WriteLine("");
                Console.WriteLine("Details:");
                Console.WriteLine(e.ToString());
                Console.WriteLine("");
                PrintUsage();
            }
        }

        private static void PrintUsage()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.WriteLine("DataMatrix Encoder ({0})", fvi.FileVersion);
            Console.WriteLine("Usage: {0} <CIP> <EXP> <LOT>", "DME.exe"); 
        }

        private static Dictionary<string, string> ExtractAiData(string[] args)
        {
            string lot = args[0];
            string exp = args[1];
            string cip = args[2];

            return new Dictionary<string, string>() 
            {
                { "01", PrepareCip(cip) },
                { "17", PrepareExp(exp) }, 
                { "10", PrepareLot(lot) }
            };
        }

        private static string PrepareCip(string cip)
        {
            return cip.PadLeft(14, '0');
        }

        private static string PrepareExp(string exp)
        {
            return exp.PadRight(6, '0');
        }

        private static string PrepareLot(string lot)
        {
            return lot;
        }

        private static string BuildDatamatrixContent(Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FNC1);
            foreach (var d in data)
            {
                sb.Append(d.Key);
                sb.Append(d.Value);
            }
            return sb.ToString();
        }

        private static void ValidateArgs(string[] args)
        {
            if (args.Length == 3)
            {
                string lot = args[0];
                string exp = args[1];
                string cip = args[2];
                
                if (exp.Length != 4)
                {
                    throw new Exception("EXP format is wrong! Should be YYMM (es. 1709).");
                }
                if (cip.Length > 14)
                {
                    throw new Exception("CIP format is wrong! Should 14 characters maximum.");
                }
            }
            else
            {
                throw new Exception("Wrong number of commandline parameters.");
            }
        }

        private static Bitmap CreateDataMatrix(string content)
        {
            return CreateDataMatrixWriter(200, 200).Write(content);
        }

        private static IBarcodeWriter CreateDataMatrixWriter(int codeWidth, int codeHeight)
        {
            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.DATA_MATRIX,
                Encoder = new DataMatrixWriter(),
                Options = new DatamatrixEncodingOptions
                {
                    SymbolShape = SymbolShapeHint.FORCE_SQUARE,
                    Height = codeHeight,
                    Width = codeWidth,
                    Margin = 0,
                    MinSize = new Dimension(20, 20),
                    MaxSize = new Dimension(20, 20)
                },
                Renderer = new BitmapRenderer
                {
                    Background = Color.White,
                    Foreground = Color.Black
                }
            };
            return writer;
        }
    }
}
