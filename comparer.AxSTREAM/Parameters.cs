using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SW.Common
{
    public class Parameter
    {
        private static readonly System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo() { NumberGroupSeparator = "." };

        public Parameter(string val)
        {
            Value = val;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        //         public static explicit operator int?(Parameter element);
        //         public static explicit operator uint(Parameter element);
        //         public static explicit operator bool?(Parameter element);
        //         public static explicit operator bool(Parameter element);
        public static explicit operator int(Parameter element)
        {
            return GetInteger(element.Value);
        }
        //         public static explicit operator long(Parameter element);
        //         public static explicit operator long?(Parameter element);
        //         public static explicit operator ulong(Parameter element);
        //         public static explicit operator uint?(Parameter element);
        public static explicit operator string(Parameter element)
        {
            return element.Value;
        }
        //         public static explicit operator Guid?(Parameter element);
        //         public static explicit operator Guid(Parameter element);
        //         public static explicit operator ulong?(Parameter element);
        //         public static explicit operator float(Parameter element);
        //         public static explicit operator float?(Parameter element);
        public static explicit operator double(Parameter element)
        {
            return GetDouble(element.Value);
        }
        //         public static explicit operator double?(Parameter element);
        //         public static explicit operator decimal(Parameter element);
        //         public static explicit operator decimal?(Parameter element);
        //         public static explicit operator DateTime(Parameter element);
        //         public static explicit operator TimeSpan?(Parameter element);
        //         public static explicit operator TimeSpan(Parameter element);
        //         public static explicit operator DateTime?(Parameter element);
        //         public static explicit operator DateTimeOffset(Parameter element);
        //         public static explicit operator DateTimeOffset?(Parameter element);
        internal static double GetDouble(string value, double defaultValue = double.NaN)
        {
            if (double.TryParse(value, NumberStyles.Any, provider, out double result))
            {
                return result;
            }

            return defaultValue;
        }

        internal static int GetInteger(string value, int defaultValue = -1)
        {
            if (int.TryParse(value, out int iValue))
            {
                return iValue;
            }

            return defaultValue;
        }

        public string AsString()
        {
            return Value;
        }

        public double AsDouble()
        {
            return Parameter.GetDouble(Value);
        }

        public int AsInteger()
        {
            return Parameter.GetInteger(Value);
        }


        public Parameters Parameters
        {
            get
            {
                Parameters p = new Parameters();
                p.SetString(ToString());
                return p;
            }
        }
    }

    public class Parameters : IEnumerable<Parameter>
    {
        private readonly char[] _split_chars = new char[] { ' ', '\t', ',' };
        private IEnumerable<Parameter> _strings = null;

        public Parameters()
        {

        }

        public Parameters(string[] arrs)
        {
            _strings = arrs.Select(s => new Parameter(s));


        }

        public Parameters(IEnumerable<string> lst_str, string tag)
        {
            _strings = lst_str.Select(s => new Parameter(s));
            Tag = tag;
        }




        public Parameters(char[] split_chars)
        {
            _split_chars = split_chars;
        }

        public Parameters(string s, char[] split_chars)
        {
            _split_chars = split_chars;
            SetString(s);
        }

        public Parameters(string s, string tag, char[] split_chars)
        {
            _split_chars = split_chars;
            SetString(s);
            Tag = tag;
        }

        public string Tag { get; set; }

        public void SetString(string s)
        {
            _strings = Split(s, _split_chars);//s.Split(_split_chars, StringSplitOptions.RemoveEmptyEntries).Select(st=>new Parameter(st));
        }
        public void SetString(string[] s)
        {
            _strings = s.Select(st => new Parameter(st));
        }

        public void SetString(IEnumerable<string> s)
        {
            _strings = s.Select(st => new Parameter(st));
        }

        public static IEnumerable<Parameter> Split(string s, char[] separators)
        {
            List<Parameter> list = new List<Parameter>();
            if (!string.IsNullOrEmpty(s))
            {
                int startIndex = 0;
                int length = s.Length;
                while (startIndex < length)
                {
                    if (s[startIndex] == '!')
                    {
                        if (startIndex + 1 < length && s[startIndex + 1] == '[')
                        {

                            int idx = s.IndexOf(']', startIndex);
                            list.Add(new Parameter(s.Substring(startIndex + 2, idx - 2 - startIndex)));
                            startIndex = idx + 1;
                            continue;
                        }
                        else
                        {
                            int idx = s.IndexOf('\n', startIndex);
                            if (idx == -1)
                            {
                                startIndex = length;
                                break;
                            }
                            startIndex = idx + 1;
                            continue;
                        }
                    }
                    int i = s.IndexOfAny(separators, startIndex);
                    if (i == -1)
                    {
                        list.Add(new Parameter(s.Substring(startIndex, length - startIndex)));
                        break;
                    }
                    if (i - startIndex != 0)
                    {
                        list.Add(new Parameter(s.Substring(startIndex, i - startIndex)));
                    }

                    startIndex = i + 1;
                }
            }
            return list;
        }

        public int Count()
        {
            return _strings != null ? _strings.Count() : 0;
        }

        public double AsDouble(int index, double defaultValue)
        {
            return Parameter.GetDouble(AsString(index), defaultValue);
        }

        public double AsDouble(int index)
        {
            return Parameter.GetDouble(AsString(index));
        }

        public int AsInteger(int index)
        {
            return Parameter.GetInteger(AsString(index));
        }

        public int AsInteger(int index, int default_int)
        {
            return Parameter.GetInteger(AsString(index), default_int);
        }

        public string AsString(int index)
        {
            return AsString(index, "");
        }

        public string AsString(int index, string defstr)
        {
            string name = index < Count() ? (string)_strings.ElementAt(index) : defstr;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = defstr;
            }

            return name;
        }


        #region IEnumerable<string> Implementation + index
        public Parameter this[int index] => _strings.ElementAt(index);

        IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator()
        {
            return _strings.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _strings.GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(0xFFFF);
            foreach (string s in this)
            {
                sb.AppendLine(s);
            }

            string str = "";
            try
            {
                str = sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }

            return str;
        }
    }

}
