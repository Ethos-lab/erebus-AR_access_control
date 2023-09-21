using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace SortCS.Kalman
{
    public class Vector
    {
        public double[] _values;

        public Vector(params double[] values)
        {
            _values = values;
        }

        public Vector(int size)
        {
            _values = new double[size];
        }

        public int Length => _values.Length;

        public double this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }

        public static Vector operator -(Vector first, Vector second)
        {
            //Debug.Assert(first.Length == second.Length, "Vectors should be of equal size");
            var arr1 = first._values;
            var arr2 = second._values;
            var res_arr = new double[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                res_arr[i] = arr1[i] - arr2[i];
            }
            Vector res_vec = new Vector(res_arr);
            return res_vec;

            //return new Vector(first._values.Zip(second._values, (a, b) => a - b).ToArray());
        }

        public static Vector operator +(Vector first, Vector second)
        {
            //Debug.Assert(first.Length == second.Length, "Vectors should be of equal size");
            var arr1 = first._values;
            var arr2 = second._values;
            var res_arr = new double[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                res_arr[i] = arr1[i] + arr2[i];
            }
            Vector res_vec = new Vector(res_arr);
            return res_vec;

            //return new Vector(first._values.Zip(second._values, (a, b) => a + b).ToArray());
        }

        public double Dot(Vector other)
        {
            //Debug.Assert(_values.Length == other._values.Length, "Vectors should be of equal length.");
            //Debug.Assert(_values.Length > 0, "Vectors must have at least one element.");

            var arr1 = _values;
            var arr2 = other._values;
            double sum = 0;
            for (int i = 0; i < arr1.Length; i++)
            {
                sum += (arr1[i] * arr2[i]);
            }
            return sum;
            //return _values.Zip(other._values, (a, b) => a * b).Sum();
        }

        public override string ToString()
        {
            return string.Join(", ", _values.Select(v => v.ToString("###0.00", CultureInfo.InvariantCulture)));
        }

        internal Vector Append(params double[] extraElements)
        {
            return new Vector(_values.Concat(extraElements).ToArray());
        }

        internal double[] ToArray()
        {
            return _values.ToArray();
        }
    }
}