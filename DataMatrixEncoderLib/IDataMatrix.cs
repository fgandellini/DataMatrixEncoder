using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMatrixEncoderLib
{
    public interface IDataMatrix
    {
        Size Size { get; set; }
        List<IDataMatrixField> Fields { get; set; }
    }

    public interface IDataMatrixField
    {
        string Name { get; set; }
        string Value { get; set; }
        string AiCode { get; set; }
        string AiName { get; set; }
        int MinLength { get; set; }
        int MaxLength { get; set; }
        IDataMatrixField Validate();
        IDataMatrixField Fix();
    }
}
