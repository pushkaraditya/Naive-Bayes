using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    public static class Utility
    {
        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.String:
                default:
                    return false;
            }
        }
    }
}
