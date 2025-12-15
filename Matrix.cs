namespace Transformation
{
    class Matrix
    {
        public static void Multiply(float[,]matrix1, float[,] matrix2, out float[,]resultantMatrix) 
        {
            //resultantMatrix = new float[,] {};
            (int row, int column) resultantMatrixSize = ( matrix1.Length, matrix2.GetLength(1));
            resultantMatrix = new float[resultantMatrixSize.row, resultantMatrixSize.column];

            for(int i =0; i<; i++)
            {
                for (int j = 0; j<; j++)
                {
                    for (int k = 0; k < matrix1.GetLength(1); k++)
                    {
                        resultantMatrix[,] = matrix1[,k] * matrix2[k,];

                        //resultantMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
                
            

                //for (int j = 0; j < matrix2.GetLength(1); j++)
                //{

                //    for (int k = 0; k < matrix1.GetLength(1); k++)
                //    {
                //        resultantMatrix[i, j] += matrix1[i, k] * matrix2[k, j];
                //    }
                //}
            }
        }
    }
}