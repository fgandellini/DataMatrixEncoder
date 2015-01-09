using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMatrixEncoderLib
{
    public class ContentManager
    {
        public const char FNC1 = (char)0x1D;
        public const char CODEWORD_FNC1 = (char)232;

        public IDataMatrix Content;

        public ContentManager Parse(IDataMatrix dataMatrix)
        {
            CheckInput(dataMatrix);
            dataMatrix.Fields.ForEach(f => f.Validate().Fix());
            Content = dataMatrix;
            return this;
        }

        private void CheckInput(IDataMatrix dataMatrix)
        {
            if (dataMatrix == null || dataMatrix.Fields == null)
            {
                throw new ArgumentException("ContentManager cannot parse null data.");
            }
            if (dataMatrix.Fields.Count < 3)
            {
                throw new ArgumentException("ContentManager cannot parse less than 3 fields.");
            }
            if (dataMatrix.Fields.Count > 3)
            {
                throw new ArgumentException("ContentManager cannot parse more than 3 fields.");
            }
        }

        public override string ToString()
        {
            return ContentManager.FNC1 + ToHumanReadable();
        }

        public string ToHumanReadable()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDataMatrixField field in Content.Fields)
            {
                sb.Append(field.AiCode);
                sb.Append(field.Value);
            }
            return sb.ToString();
        }
    }
}
