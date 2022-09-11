using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RayTracer.Debugging;
using RayTracer.Utility;

using Utils = RayTracer.Utility.Utilities;

namespace RayTracer.Maths
{
    /// <summary>
    /// A standard transformation matrix with customisable size.
    /// </summary>
    public class Matrix : IEnumerable
    {
        public readonly static Matrix identity = new Matrix(4, 4) {
            _internalArray = new float[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1}
            }
        };

        private float[,] _internalArray;
        public int Dimensions { get; private set; }
        public int ElementsPerDimension { get; set; }
        public bool Invertible => CalculateDeterminant() != 0;

        public Matrix Inverse { //Should cache this.
            get {
                float determinant = CalculateDeterminant();
                if (determinant == 0) return this;

                Matrix inverse = new Matrix(Dimensions, ElementsPerDimension);
                for (int x = 0; x < Dimensions; x++) {
                    for (int y = 0; y < ElementsPerDimension; y++)
                        inverse[y, x] = CalculateCofactor(x, y) / determinant;
                }
                return inverse;
            }
        }

        public float this[int x, int y] {
            get => _internalArray[x, y];
            set => _internalArray[x, y] = value;
        }
        public float this[int index] {
            get {
                int x = index % ElementsPerDimension;
                int y = (int)Math.Floor((double)index / ElementsPerDimension);
                return _internalArray[x, y];
            }
            set {
                int x = index % ElementsPerDimension;
                int y = (int)Math.Floor((double)index / ElementsPerDimension);
                _internalArray[x, y] = value;
            }
        }

        public Matrix(int width, int height) {
            if (width != height)
                throw new Exception("Invalid Matrix size! A Matrix must be uniform!");

            _internalArray = new float[height, width];
            ElementsPerDimension = width;
            Dimensions = height;
        }

        public Matrix() {
            _internalArray = new float[0, 0];
        }

        public Matrix Transpose() {
            Matrix m = new Matrix(Dimensions, ElementsPerDimension);
            for (int x = 0; x < Dimensions; x++) {
                for (int y = 0; y < ElementsPerDimension; y++) {
                    m[x, y] = this[y, x];
                }
            }
            return m;
        }

        public Matrix GetSubMatrix(int row, int column) {
            Matrix m = new Matrix(Dimensions - 1, ElementsPerDimension - 1);

            int dx = 0, dy = 0;
            for (int x = 0; x < Dimensions; x++) {
                if (x == row) continue;
                for (int y = 0; y < ElementsPerDimension; y++) {
                    if (y == column) continue;
                    m[dx, dy++] = this[x, y];
                }
                dy = 0;
                ++dx;
            }

            return m;
        }

        public float CalculateMinor(int row, int column) {
            Matrix sub = GetSubMatrix(row, column);
            return sub.CalculateDeterminant();
        }

        public float CalculateDeterminant() {
            if (Dimensions == 2)
                return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
            else
            {
                float determinant = 0f;
                for (int y = 0; y < ElementsPerDimension; y++)
                    determinant += this[0, y] * CalculateCofactor(0, y);
                return determinant;
            }
        }

        public float CalculateCofactor(int row, int column) {
            float minor = CalculateMinor(row, column);
            if ((row + column) % 2 != 0)
                return -minor;
            else
                return minor;
        }

        public static Matrix Translation(float x, float y, float z)
        {
            //return new Matrix() //Cool initialisation but slow as it relies on 4 Add() calls which Resize the internal array.
            //{
            //    { 1, 0, 0, x},
            //    { 0, 1, 0, y},
            //    { 0, 0, 1, z},
            //    { 0, 0, 0, 1}
            //}; 

            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { 1, 0, 0, x},
                    { 0, 1, 0, y},
                    { 0, 0, 1, z},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static Matrix Scale(float x, float y, float z) {
            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { x, 0, 0, 0},
                    { 0, y, 0, 0},
                    { 0, 0, z, 0},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static Matrix RotateX(float radians) {
            float sin = MathF.Sin(radians);
            float cos = MathF.Cos(radians);
            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { 1, 0, 0, 0},
                    { 0, cos, -sin, 0},
                    { 0, sin, cos, 0},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static Matrix RotateY(float radians) {
            float sin = MathF.Sin(radians);
            float cos = MathF.Cos(radians);
            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { cos, 0, sin, 0},
                    { 0, 1, 0, 0},
                    { -sin, 0, cos, 0},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static Matrix RotateZ(float radians)
        {
            float sin = MathF.Sin(radians);
            float cos = MathF.Cos(radians);
            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { cos, -sin, 0, 0},
                    { sin, cos, 0, 0},
                    { 0, 0, 1, 0},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static Matrix Shear(float xy, float xz, float yx, float yz, float zx, float zy) {
            return new Matrix(4, 4)
            {
                _internalArray = new float[,] {
                    { 1, xy, xz, 0},
                    { yx, 1, yz, 0},
                    { zx,zy, 1, 0},
                    { 0, 0, 0, 1}
                }
            };
        }

        public static bool operator ==(Matrix matrix, Matrix comp){
            if (matrix.Dimensions != comp.Dimensions || matrix.ElementsPerDimension != comp.ElementsPerDimension)
                return false;

            for (int x = 0; x < matrix.Dimensions; x++)
            {
                for (int y = 0; y < matrix.ElementsPerDimension; y++) {
                    if (!matrix[x, y].IsApproximately(comp[x, y]))
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(Matrix matrix, Matrix comp)
            => !(matrix == comp);

        //Somehow this approach is faster than direct mathematics
        public static Matrix operator *(Matrix matrix, Matrix mult)
        {
            Matrix m;

            //if (matrix._is4x4 && mult._is4x4)
            //{
            //    m = new Matrix(4, 4);
            //    m[0, 0] = matrix[0, 0] * mult[0, 0] + matrix[0, 1] * mult[1, 0] + matrix[0, 2] * mult[2, 0] + matrix[0, 3] * mult[3, 0];
            //    m[0, 1] = matrix[0, 0] * mult[0, 1] + matrix[0, 1] * mult[1, 1] + matrix[0, 2] * mult[2, 1] + matrix[0, 3] * mult[3, 1];
            //    m[0, 2] = matrix[0, 0] * mult[0, 2] + matrix[0, 1] * mult[1, 2] + matrix[0, 2] * mult[2, 2] + matrix[0, 3] * mult[3, 2];
            //    m[0, 3] = matrix[0, 0] * mult[0, 3] + matrix[0, 1] * mult[1, 3] + matrix[0, 2] * mult[2, 3] + matrix[0, 3] * mult[3, 3];

            //    m[1, 0] = matrix[1, 0] * mult[0, 0] + matrix[1, 1] * mult[1, 0] + matrix[1, 2] * mult[2, 0] + matrix[1, 3] * mult[3, 0];
            //    m[1, 1] = matrix[1, 0] * mult[0, 1] + matrix[1, 1] * mult[1, 1] + matrix[1, 2] * mult[2, 1] + matrix[1, 3] * mult[3, 1];
            //    m[1, 2] = matrix[1, 0] * mult[0, 2] + matrix[1, 1] * mult[1, 2] + matrix[1, 2] * mult[2, 2] + matrix[1, 3] * mult[3, 2];
            //    m[1, 3] = matrix[1, 0] * mult[0, 3] + matrix[1, 1] * mult[1, 3] + matrix[1, 2] * mult[2, 3] + matrix[1, 3] * mult[3, 3];

            //    m[2, 0] = matrix[2, 0] * mult[0, 0] + matrix[2, 1] * mult[1, 0] + matrix[2, 2] * mult[2, 0] + matrix[2, 3] * mult[3, 0];
            //    m[2, 1] = matrix[2, 0] * mult[0, 1] + matrix[2, 1] * mult[1, 1] + matrix[2, 2] * mult[2, 1] + matrix[2, 3] * mult[3, 1];
            //    m[2, 2] = matrix[2, 0] * mult[0, 2] + matrix[2, 1] * mult[1, 2] + matrix[2, 2] * mult[2, 2] + matrix[2, 3] * mult[3, 2];
            //    m[2, 3] = matrix[2, 0] * mult[0, 3] + matrix[2, 1] * mult[1, 3] + matrix[2, 2] * mult[2, 3] + matrix[2, 3] * mult[3, 3];

            //    m[3, 0] = matrix[3, 0] * mult[0, 0] + matrix[3, 1] * mult[1, 0] + matrix[3, 2] * mult[2, 0] + matrix[3, 3] * mult[3, 0];
            //    m[3, 1] = matrix[3, 0] * mult[0, 1] + matrix[3, 1] * mult[1, 1] + matrix[3, 2] * mult[2, 1] + matrix[3, 3] * mult[3, 1];
            //    m[3, 2] = matrix[3, 0] * mult[0, 2] + matrix[3, 1] * mult[1, 2] + matrix[3, 2] * mult[2, 2] + matrix[3, 3] * mult[3, 2];
            //    m[3, 3] = matrix[3, 0] * mult[0, 3] + matrix[3, 1] * mult[1, 3] + matrix[3, 2] * mult[2, 3] + matrix[3, 3] * mult[3, 3];
            //}
            m = new Matrix(matrix.ElementsPerDimension, matrix.Dimensions);
            for (int x = 0; x < matrix.Dimensions; x++)
            {
                for (int y = 0; y < matrix.ElementsPerDimension; y++)
                {
                    float value = 0f;
                    for (int dx = 0; dx < matrix.Dimensions; dx++)
                        value += matrix[x, dx] * mult[dx, y];
                    m[x, y] = value;
                }
            }
            return m;
        }

        public static Float4 operator *(Float4 float4, Matrix matrix)
        {
            Float4 f = new Float4(); //Somehow this approach is faster than direct mathematics
            for (int x = 0; x < matrix.Dimensions; x++)
            {
                for (int y = 0; y < matrix.ElementsPerDimension; y++)
                {
                    float value = 0f;
                    for (int dx = 0; dx < 4; dx++)
                        value += matrix[x, dx] * float4[dx];
                    if (x < 4)
                        f[x] = value;
                }
            }

            return f;
        }

        public static Float4 operator *(Matrix matrix, Float4 float4)
            => float4 * matrix;

        #region Inherited Methods
        public IEnumerator GetEnumerator()
            => _internalArray.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        //Hacky way of initialising matrices like multidimensional arrays. This is REALLY bad so it's only used for initialising test matrices.
        /// <summary>
        /// Adds a new dimension to the Matrix. This function should not be called!
        /// </summary>
        /// <param name="values">Row of valuues to add to the Matrix</param>
        internal void Add(params float[] values) { 
            Dimensions++;
            if (ElementsPerDimension == 0)
                ElementsPerDimension = values.Length;
            else {
                if (values.Length != ElementsPerDimension)
                    throw new Exception($"Invalid element length at Matrix Dimension [{Dimensions}] Index: [{Dimensions - 1}]");
            }

            Utils.ResizeTwoDimensionalArray(ref _internalArray , Dimensions, ElementsPerDimension);
            for (int i = 0; i < ElementsPerDimension; i++)
                _internalArray[Dimensions - 1, i] = values[i];
        }

        public override string ToString()
            => ToString(false);

        public string ToString(bool displayIndices) { //This loop is backwards because I'm an idiot
            StringBuilder dimBuilder = new StringBuilder();
            for (int x = 0; x < Dimensions; x++)
            {
                dimBuilder.Append("[");
                for (int y = 0; y < ElementsPerDimension; y++)
                    dimBuilder.Append($"{(displayIndices ? $"({y},{x})->" : "")}{this[x, y]}{(y == ElementsPerDimension - 1 ? "" : ", ")}");
                dimBuilder.Append("]");
                if (x != Dimensions - 1)
                    dimBuilder.AppendLine();
            }

            return dimBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix m)
                return m == this;
            return false;
        }

        public override int GetHashCode()
            => _internalArray.GetHashCode() ^ Dimensions ^ ElementsPerDimension;
        #endregion
    }
}