using System;
using System.Collections.Generic;

namespace DisEn
{
    public class Matrix
    {
        private List<float> matrixArray;
        private int rows;
        private int columns;

        private static readonly Random random = new Random();

        private static int ConvertRowAndColumnToIndex(int row, int column, int totalColumns)
        {
            return (row * totalColumns) + column;
        }

        public Matrix() : this(0, 0)
        {
        }

        public Matrix(int rows, int columns) : this(rows, columns, 0)
        {
        }

        public Matrix(int rows, int columns, float initialValue)
        {
            this.rows = rows;
            this.columns = columns;
            matrixArray = new List<float>(rows * columns);
            for (int i = 0; i < rows * columns; i++)
            {
                matrixArray.Add(initialValue);
            }
        }

        public int GetRows()
        {
            return rows;
        }

        public int GetColumns()
        {
            return columns;
        }

        public List<float> ToArray()
        {
            return matrixArray;
        }

        public void SetElement(int row, int column, float value)
        {
            matrixArray[ConvertRowAndColumnToIndex(row, column, columns)] = value;
        }

        public float GetElement(int row, int column)
        {
            return matrixArray[ConvertRowAndColumnToIndex(row, column, columns)];
        }

        public static Matrix FromArray(List<float> array)
        {
            Matrix resultMatrix = new Matrix(array.Count, 1);
            resultMatrix.matrixArray = array;
            return resultMatrix;
        }

        public static Matrix Add(Matrix matrixA, float value)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<float>(matrixA.matrixArray);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) + value);
                }
            }

            return resultMatrix;
        }

        public static Matrix Add(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.GetRows() != matrixB.GetRows() || matrixA.GetColumns() != matrixB.GetColumns())
            {
                return new Matrix(0, 0);
            }

            Matrix resultMatrix = new Matrix(matrixA.rows, matrixA.columns);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) + matrixB.GetElement(r, c));
                }
            }

            return resultMatrix;
        }

        public static Matrix Subtract(Matrix matrixA, float value)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<float>(matrixA.matrixArray);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) - value);
                }
            }

            return resultMatrix;
        }

        public static Matrix Subtract(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.GetRows() != matrixB.GetRows() || matrixA.GetColumns() != matrixB.GetColumns())
            {
                return new Matrix(0, 0);
            }

            Matrix resultMatrix = new Matrix(matrixA.rows, matrixA.columns);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) - matrixB.GetElement(r, c));
                }
            }

            return resultMatrix;
        }

        public static Matrix Multiply(Matrix matrixA, float scale)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<float>(matrixA.matrixArray);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) * scale);
                }
            }

            return resultMatrix;
        }

        public static Matrix Multiply(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.GetColumns() != matrixB.GetRows())
            {
                return new Matrix(0, 0);
            }

            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixB.GetColumns());

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    for (int it = 0; it < matrixA.GetColumns(); ++it)
                    {
                        resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) + matrixA.GetElement(r, it) * matrixB.GetElement(it, c));
                    }
                }
            }

            return resultMatrix;
        }

        public static Matrix ElementWiseMultiplication(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.GetRows() != matrixB.GetRows() || matrixA.GetColumns() != matrixB.GetColumns())
            {
                return new Matrix(0, 0);
            }

            Matrix resultMatrix = new Matrix(matrixA.rows, matrixA.columns);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, resultMatrix.GetElement(r, c) * matrixB.GetElement(r, c));
                }
            }

            return resultMatrix;
        }

        public void Transpose()
        {
            Matrix resultMatrix = new Matrix(columns, rows);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, GetElement(c, r));
                }
            }

            rows = resultMatrix.GetRows();
            columns = resultMatrix.GetColumns();
            matrixArray = new List<float>(resultMatrix.matrixArray);
        }

        public static Matrix Transpose(Matrix matrixA)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetColumns(), matrixA.GetRows());

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, matrixA.GetElement(c, r));
                }
            }

            return resultMatrix;
        }

        public void Randomize(float startRange, float endRange)
        {
            if (startRange > endRange)
            {
                float temp = startRange;
                startRange = endRange;
                endRange = temp;
            }

            for (int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < columns; ++c)
                {
                    SetElement(r, c, (float)(random.NextDouble() * (endRange - startRange) + startRange));
                }
            }
        }

        public void Map(Func<float, float> func)
        {
            for (int r = 0; r < GetRows(); ++r)
            {
                for (int c = 0; c < GetColumns(); ++c)
                {
                    SetElement(r, c, func(GetElement(r, c)));
                }
            }
        }

        public static Matrix Map(Matrix matrixA, Func<float, float> func)
        {
            Matrix resultMatrix = new Matrix(matrixA.rows, matrixA.columns);
            resultMatrix.matrixArray = new List<float>(matrixA.matrixArray);

            for (int r = 0; r < resultMatrix.GetRows(); ++r)
            {
                for (int c = 0; c < resultMatrix.GetColumns(); ++c)
                {
                    resultMatrix.SetElement(r, c, func(resultMatrix.GetElement(r, c)));
                }
            }

            return resultMatrix;
        }
    }
}