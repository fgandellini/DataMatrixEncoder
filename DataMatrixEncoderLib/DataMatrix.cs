using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMatrixEncoderLib
{
    public class DataMatrix : IDataMatrix
    {
        public Size Size { get; set; }
        public List<IDataMatrixField> Fields { get; set; }
    }

    public class DataMatrixField : IDataMatrixField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string AiCode { get; set; }
        public string AiName { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public DataMatrixField(string name = "", string value = "", string aiCode="", string aiName="", int minLength=0, int maxLength=0)
        {
            this.Name = name;
            this.Value = value;
            this.AiCode = aiCode;
            this.AiName = aiName;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
        }

        public virtual IDataMatrixField Validate()
        {
            if (this.Value.Length < this.MinLength || this.Value.Length > this.MaxLength)
            {
                throw new ArgumentException(
                    string.Format(
                    "Formato {0} non valido: sono richiesti da {1} a {2} caratteri. Ne sono stati inseriti {3}.",
                    this.Name, this.MinLength, this.MaxLength, this.Value.Length));
            }
            return this;
        }

        public virtual IDataMatrixField Fix()
        {
            return this;
        }
    }

    public class GtinDataMatrixField : DataMatrixField
    {
        public GtinDataMatrixField(string name = "", string value = "")
            : base(name, value, "01", "GTIN", 13, 14)
        {
        }

        public override IDataMatrixField Fix()
        {
            this.Value = PadLeft(this.Value, this.MaxLength);
            return this;
        }

        private string PadLeft(string data, int totalLength)
        {
            if (totalLength < int.MaxValue)
            {
                return data.PadLeft(totalLength, '0');
            }
            return data;
        }
    }

    public class ExpDataMatrixField : DataMatrixField
    {
        public ExpDataMatrixField(string name = "", string value = "")
            : base(name, value, "17", "EXP", 4, 6)
        {
        }

        public override IDataMatrixField Fix()
        {
            this.Value = PadRight(this.Value, this.MaxLength);
            return this;
        }

        private string PadRight(string data, int totalLength)
        {
            if (totalLength < int.MaxValue)
            {
                return data.PadRight(totalLength, '0');
            }
            return data;
        }
    }

    public class LotDataMatrixField : DataMatrixField
    {
        public LotDataMatrixField(string name = "", string value = "")
            : base(name, value, "10", "LOT", 0, int.MaxValue)
        {
        }
    }
}
