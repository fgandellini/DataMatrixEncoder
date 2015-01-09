using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Datamatrix;
using ZXing.Datamatrix.Encoder;
using ZXing.Rendering;

namespace DataMatrixEncoderLib
{
    public class Encoder
    {
        public int Size { get; private set; }
        public string Content { get; private set; }

        public Encoder SetSize(int size)
        {
            this.Size = size;
            return this;
        }

        public Encoder SetContent(string content)
        {
            this.Content = content;
            return this;
        }

        public Bitmap Encode()
        {
            return (Bitmap)CreateDataMatrixWriter(Size, Size)
               .Write(Content)
               .Clone();
        }

        private IBarcodeWriter CreateDataMatrixWriter(int width, int height)
        {
            IBarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.DATA_MATRIX,
                Encoder = new DataMatrixWriter(),
                Options = new DatamatrixEncodingOptions
                {
                    SymbolShape = SymbolShapeHint.FORCE_SQUARE,
                    Height = height,
                    Width = width,
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
