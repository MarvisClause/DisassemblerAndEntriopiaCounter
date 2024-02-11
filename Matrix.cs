using System;
using System.Collections.Generic;

namespace DisEn
{
    [Serializable]
    public class Matrix
    {
        private List<double> matrixArray;
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

        public Matrix(int rows, int columns, double initialValue)
        {
            this.rows = rows;
            this.columns = columns;
            matrixArray = new List<double>(rows * columns);
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

        public List<double> ToArray()
        {
            return matrixArray;
        }

        public void SetElement(int row, int column, double value)
        {
            matrixArray[ConvertRowAndColumnToIndex(row, column, columns)] = value;
        }

        public double GetElement(int row, int column)
        {
            return matrixArray[ConvertRowAndColumnToIndex(row, column, columns)];
        }

        public static Matrix FromArray(List<double> array)
        {
            Matrix resultMatrix = new Matrix(array.Count, 1);
            resultMatrix.matrixArray = array;
            return resultMatrix;
        }

        public static Matrix Add(Matrix matrixA, double value)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<double>(matrixA.matrixArray);

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
                    resultMatrix.SetElement(r, c, matrixA.GetElement(r, c) + matrixB.GetElement(r, c));
                }
            }

            return resultMatrix;
        }

        public static Matrix Subtract(Matrix matrixA, double value)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<double>(matrixA.matrixArray);

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
                    resultMatrix.SetElement(r, c, matrixA.GetElement(r, c) - matrixB.GetElement(r, c));
                }
            }

            return resultMatrix;
        }

        public static Matrix Multiply(Matrix matrixA, double scale)
        {
            Matrix resultMatrix = new Matrix(matrixA.GetRows(), matrixA.GetColumns());
            resultMatrix.matrixArray = new List<double>(matrixA.matrixArray);

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
                    resultMatrix.SetElement(r, c, matrixA.GetElement(r, c) * matrixB.GetElement(r, c));
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
            matrixArray = new List<double>(resultMatrix.matrixArray);
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

        public void Randomize(double startRange, double endRange)
        {
            if (startRange > endRange)
            {
                double temp = startRange;
                startRange = endRange;
                endRange = temp;
            }

            for (int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < columns; ++c)
                {
                    SetElement(r, c, (double)(random.NextDouble() * (endRange - startRange) + startRange));
                }
            }
        }

        public void Map(Func<double, double> func)
        {
            for (int r = 0; r < GetRows(); ++r)
            {
                for (int c = 0; c < GetColumns(); ++c)
                {
                    SetElement(r, c, func(GetElement(r, c)));
                }
            }
        }

        public static Matrix Map(Matrix matrixA, Func<double, double> func)
        {
            Matrix resultMatrix = new Matrix(matrixA.rows, matrixA.columns);
            resultMatrix.matrixArray = new List<double>(matrixA.matrixArray);

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