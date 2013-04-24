using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EnumToString
{
    public class FakeNumField
    {
        public string Name { get; set; }
        public Object Value;
        public FakeNum Parent { get; set; }

        public static implicit operator FakeNumField(int value)
        {
            return new FakeNumField((ulong)value);
        }

        private FakeNumField(object value)
        {
            Value = value;
        }

        public object GetRawConstantValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return Parent.ToString(Value);
        }
    }

    public partial class FakeNum
    {
        private FakeNumField[] _fields;

        public FakeNum()
        {
            var fields = new List<FakeNumField>();
            foreach (var fieldInfo in GetType().GetFields())
            {
                var f = fieldInfo.GetValue(this) as FakeNumField;
                if (f == null) 
                    continue;
                
                f.Name = fieldInfo.Name;
                f.Parent = this;

                fields.Add(f);
            }

            _fields = fields.ToArray();
        }

        public String ToString(Object value)
        {
            return InternalFormat(GetType(), value);
        }

        private String InternalFormat(Type eT, Object value)
        {
            String retval = GetName(eT, value);
            if (retval == null)
                return value.ToString();

            return retval;
        }

        public String GetName(Type enumType, Object value)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            return GetEnumName(value);
        }

        public virtual string GetEnumName(object value)
        {
            Array values = GetEnumRawConstantValues();
            int index = BinarySearch(values, value);

            Console.WriteLine("INDEX:  " + index);

            if (index >= 0)
            {
                string[] names = GetEnumNames();
                return names[index];
            }

            return null;
        }

        public virtual string[] GetEnumNames()
        {
            string[] names;
            Array values;
            GetEnumData(out names, out values);
            return names;
        }

        // Convert everything to ulong then perform a binary search.
        private static int BinarySearch(Array array, object value)
        {
            ulong[] ulArray = new ulong[array.Length];
            for (int i = 0; i < array.Length; ++i)
                ulArray[i] = ToUInt64(array.GetValue(i));

            ulong ulValue = ToUInt64(value);

            return Array.BinarySearch(ulArray, ulValue);
        }

        internal static ulong ToUInt64(Object value)
        {
            // Helper function to silently convert the value to UInt64 from the other base types for enum without throwing an exception.
            // This is need since the Convert functions do overflow checks.
            TypeCode typeCode = Convert.GetTypeCode(value);
            ulong result;

            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result = (UInt64)Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    break;

                default:
                    throw new InvalidOperationException("InvalidOperation_UnknownEnumType");
            }
            return result;
        }

        private Array GetEnumRawConstantValues()
        {
            string[] names;
            Array values;
            GetEnumData(out names, out values);
            return values;
        }

        public void GetEnumData(out string[] enumNames, out Array enumValues)
        {
            var flds = _fields;

            object[] values = new object[flds.Length];
            string[] names = new string[flds.Length];

            for (int i = 0; i < flds.Length; i++)
            {
                names[i] = flds[i].Name;
                values[i] = flds[i].GetRawConstantValue();
            }

            // Insertion Sort these values in ascending order.
            // We use this O(n^2) algorithm, but it turns out that most of the time the elements are already in sorted order and 
            // the common case performance will be faster than quick sorting this.
            
            PrintArrays(names, values);
            
            IComparer comparer = Comparer.Default;
            for (int i = 1; i < values.Length; i++)
            {
                int j = i;
                string tempStr = names[i];
                object val = values[i];
                bool exchanged = false;

                // Since the elements are sorted we only need to do one comparision, we keep the check for j inside the loop.
                while (comparer.Compare(values[j - 1], val) > 0)
                {
                    names[j] = names[j - 1];
                    values[j] = values[j - 1];
                    j--;
                    exchanged = true;
                    if (j == 0)
                        break;
                }

                if (exchanged)
                {
                    names[j] = tempStr;
                    values[j] = val;
                }

                PrintArrays(names, values);
            }

            enumNames = names;
            enumValues = values;
        }

        private void PrintArrays(Object[] names, Object[] values)
        {
            var tuples = Enumerable.Range(0, names.Length).Select(x => new { Name = names[x], Value = values[x] });
            Console.WriteLine("[{0}]", String.Join(", ", tuples.Select(x => String.Format("'{0}': {1}", x.Name, x.Value))));
        }
    }
}
