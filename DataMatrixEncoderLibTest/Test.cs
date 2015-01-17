using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZXing;
using System.Drawing;
using DataMatrixEncoderLib;
using System.Collections.Generic;

namespace DataMatrixEncoderLibTest
{
    [TestClass]
    public class Test
    {
        Encoder encoder;
        ContentManager contentManager;

        [TestInitialize]
        public void Init()
        {
            encoder = new Encoder();
            contentManager = new ContentManager();
        }

        [TestMethod]
        public void Encoder_Should_EncodeHeadingFNC1()
        {
            string content = string.Format("{0}{1}", ContentManager.FNC1, "A");

            Bitmap bmp = Encode(content);
            Result decodedDataMatrix = Decode(bmp);

            Assert.IsNotNull(decodedDataMatrix);
            Assert.AreEqual((char)ContentManager.CODEWORD_FNC1, (char)decodedDataMatrix.RawBytes[0]);
        }

        [TestMethod]
        public void Encoder_Should_EncodeString()
        {
            string content = string.Format("A");

            Bitmap bmp = Encode(content);
            Result decodedDataMatrix = Decode(bmp);

            Assert.IsNotNull(decodedDataMatrix);
            Assert.AreEqual(content, decodedDataMatrix.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_GiveErrorParsingNullDataMatrix()
        {
            IDataMatrix nullDm = null;
            contentManager.Parse(nullDm);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_GiveErrorParsingNullFields()
        {
            IDataMatrix noArgsDm = new DataMatrix();
            noArgsDm.Fields = null;
            contentManager.Parse(noArgsDm);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_GiveErrorParsingNoFields()
        {
            IDataMatrix noArgsDm = new DataMatrix();
            contentManager.Parse(noArgsDm);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_GiveErrorParsingFewArgs()
        {
            IDataMatrix fewArgsDm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new DataMatrixField()
                }
            };
            contentManager.Parse(fewArgsDm);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_GiveErrorParsingManyArgs()
        {
            IDataMatrix manyArgsDm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new DataMatrixField(),
                    new DataMatrixField(),
                    new DataMatrixField(),
                    new DataMatrixField()
                }
            };
            contentManager.Parse(manyArgsDm);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_ValidateGTIN()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "1"),
                    new DataMatrixField(),
                    new DataMatrixField()
                }
            };
            contentManager.Parse(dm);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ContentManager_Should_ValidateEXP()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "12345678901234"),
                    new ExpDataMatrixField("EXP", "1"),
                    new DataMatrixField()
                }
            };
            contentManager.Parse(dm);
        }

        [TestMethod]
        public void ContentManager_Should_ParseArgs()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "12345678901234"),
                    new ExpDataMatrixField("EXP", "123456"),
                    new LotDataMatrixField("LOT", "1")
                }
            };
            contentManager.Parse(dm);
        }
        
        [TestMethod]
        public void ContentManager_Should_GiveParsedStringWithFNC1()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "12345678901234"),
                    new ExpDataMatrixField("EXP", "123456"),
                    new LotDataMatrixField("LOT", "LOT")
                }
            };
            string content = contentManager.Parse(dm).ToString();
            Assert.AreEqual(ContentManager.FNC1 + "01123456789012341712345610LOT", content);
        }
        
        [TestMethod]
        public void ContentManager_Should_PadLeftIncompleteGTIN()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "1234567890123"),
                    new ExpDataMatrixField("EXP", "123456"),
                    new LotDataMatrixField("LOT", "LOT")
                }
            };
            string content = contentManager.Parse(dm).ToString();
            Assert.AreEqual(ContentManager.FNC1 + "01012345678901231712345610LOT", content);
        }
        
        [TestMethod]
        public void ContentManager_Should_PadRightEXP()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "12345678901234"),
                    new ExpDataMatrixField("EXP", "1234"),
                    new LotDataMatrixField("LOT", "LOT")
                }
            };
            string content = contentManager.Parse(dm).ToString();
            Assert.AreEqual(ContentManager.FNC1 + "01123456789012341712340010LOT", content);
        }

        [TestMethod]
        public void ContentManager_Should_TrimSpacesFromFields()
        {
            IDataMatrix dm = new DataMatrix()
            {
                Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                { 
                    new GtinDataMatrixField("CIP", "  12345678901234  "),
                    new ExpDataMatrixField("EXP", "  123456  "),
                    new LotDataMatrixField("LOT", "  LOT  ")
                }
            };
            string content = contentManager.Parse(dm).ToString();
            Assert.AreEqual(ContentManager.FNC1 + "01123456789012341712345610LOT", content);
        }

        [TestMethod]
        public void Files_2014_12_23_Test()
        {
            string dir = @".\DataMatrix\";
            string[][] tests = 
            {
                new string[] { "03400938129397", "171000", "2M2265FR" },
                new string[] { "03400938129397", "170900", "2M2275FR" },
                new string[] { "03400938123135", "170900", "2M1032FR" },
                new string[] { "03400938123135", "171100", "2M1035FR" }
            };

            List<IDataMatrix> dms = new List<IDataMatrix>();
            foreach(string[] test in tests)
            {
                dms.Add(
                    new DataMatrix()
                    {
                        Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                        { 
                            new GtinDataMatrixField("CIP", test[0]), new ExpDataMatrixField("EXP", test[1]), new LotDataMatrixField("LOT", test[2])
                        }
                    }
                );
            }
            
            foreach(IDataMatrix dm in dms)
            {
                ContentManager cm = new ContentManager().Parse(dm);
                string code = cm.ToHumanReadable();
                string content = cm.ToString();

                Bitmap original = (Bitmap)Bitmap.FromFile(dir + code + ".png");
                Result result = Decode(original);

                Assert.AreEqual(result.Text, content);
            }
        }
        
        [TestMethod]
        public void Files_2014_12_23_IncompleteInput_Test()
        {
            string dir = @".\DataMatrix\";
            string[][] tests = 
            {
                new string[] { "3400938129397", "1710", "2M2265FR" },
                new string[] { "3400938129397", "1709", "2M2275FR" },
                new string[] { "3400938123135", "1709", "2M1032FR" },
                new string[] { "3400938123135", "1711", "2M1035FR" }
            };

            List<IDataMatrix> dms = new List<IDataMatrix>();
            foreach (string[] test in tests)
            {
                dms.Add(
                    new DataMatrix()
                    {
                        Fields = new System.Collections.Generic.List<IDataMatrixField>() 
                        { 
                            new GtinDataMatrixField("CIP", test[0]), new ExpDataMatrixField("EXP", test[1]), new LotDataMatrixField("LOT", test[2])
                        }
                    }
                );
            }

            foreach (IDataMatrix dm in dms)
            {
                ContentManager cm = new ContentManager().Parse(dm);
                string code = cm.ToHumanReadable();
                string content = cm.ToString();

                Bitmap original = (Bitmap)Bitmap.FromFile(dir + code + ".png");
                Result result = Decode(original);

                Assert.AreEqual(result.Text, content);
            }
        }

        #region Test Utils

        private Bitmap Encode(string content)
        {
            return encoder
                .SetSize(200)
                .SetContent(content)
                .Encode();
        }

        private Result Decode(Bitmap dataMatrix)
        {
            Bitmap dataMatrixWithBorder = AddQuietZone(dataMatrix, 20);

            IBarcodeReader reader = new BarcodeReader();
            return reader.Decode(dataMatrixWithBorder);
        }

        private Bitmap AddQuietZone(Bitmap bmp, int size)
        {
            Bitmap output = new Bitmap(bmp.Width + (2 * size), bmp.Height + (2 * size));
            Graphics g = Graphics.FromImage(output);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, output.Width, output.Height);
            g.DrawImage(bmp, size, size, bmp.Width, bmp.Height);
            return output;
        }

        #endregion Test Utils
    }
}
