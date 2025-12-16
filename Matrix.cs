namespace Transformation
{
    class Matrix
    {
        public static void Multiply(float[,]matrix1, float[,] matrix2, out float[,]resultantMatrix) 
        {
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
            {
                // matrix multiplication is not possible
                resultantMatrix = null;
                return;
            }
            (int row, int column) resultantMatrixSize = ( matrix1.GetLength(0), matrix2.GetLength(1));
            resultantMatrix = new float[resultantMatrixSize.row, resultantMatrixSize.column];

            // iterating over every row of first matrix
            for (int i =0; i<matrix1.GetLength(0); i++)
            {

                // iterating over every column of matrix
                for (int j = 0; j<matrix2.GetLength(1); j++)
                {

                    // iterating over subsequent element, i.e.,
                    // every subsequent element of first matrix row's and second matrix column's
                    // both are of same length and it is a must matrix multiplication to happen
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    { 
                        resultantMatrix[i,j] += matrix1[i,k] * matrix2[k,j];
                    }
                }
            }
        }
    }
}