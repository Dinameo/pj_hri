using UnityEngine;

public class Matrix
{
    public static float[,] Multiply(float[,] A, float[,] B)
    {
        int rowsA = A.GetLength(0);
        int colsA = A.GetLength(1);
        int rowsB = B.GetLength(0);
        int colsB = B.GetLength(1);

        if (colsA != rowsB)
        {
            Debug.LogError("Không thể nhân ma trận!");
            return null;
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                for (int k = 0; k < colsA; k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return result;
    }
    public static float[,] TransformMatrix(float ai, float alphaDeg, float di, float thetaDeg)
    {
        float thetaRad = thetaDeg * Mathf.Deg2Rad;
        float alphaRad = alphaDeg * Mathf.Deg2Rad;
        float t11 = Mathf.Cos(thetaRad);
        float t12 = -Mathf.Sin(thetaRad) * Mathf.Cos(alphaRad);
        float t13 = Mathf.Sin(thetaRad) * Mathf.Sin(alphaRad);
        float t14 = ai * Mathf.Cos(thetaRad);

        float t21 = Mathf.Sin(thetaRad);
        float t22 = Mathf.Cos(thetaRad) * Mathf.Cos(alphaRad);
        float t23 = -Mathf.Cos(thetaRad) * Mathf.Sin(alphaRad);
        float t24 = ai * Mathf.Sin(thetaRad);

        float t31 = 0;
        float t32 = Mathf.Sin(alphaRad);
        float t33 = Mathf.Cos(alphaRad);
        float t34 = di;

        float t41 = 0;
        float t42 = 0;
        float t43 = 0;
        float t44 = 1;

        return new float[4,4] {{t11, t12, t13, t14}, {t21, t22, t23, t24}, {t31, t32, t33, t34}, {t41, t42, t43, t44}};
    }
    public static string MatrixToString(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append(matrix[i, j].ToString("F3")).Append("\t");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
    public static float[,] Transpose(float[,] T)
    {
        int rows = T.GetLength(0);
        int cols = T.GetLength(1);
        float[,] trans = new float[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                trans[j, i] = T[i, j];
            }
        }
        return trans;
    }
    public static float[,] InverseHomogeneous(float[,] T)
    {
        float[,] R = new float[3, 3] {
            {T[0, 0], T[0, 1], T[0, 2]},
            {T[1, 0], T[1, 1], T[1, 2]},
            {T[2, 0], T[2, 1], T[2, 2]}
        };
        float[,] Rt = Transpose(R);
        float x = T[0, 3];
        float y = T[1, 3];
        float z = T[2, 3];
        float[,] inv = new float[4, 4] {
            {Rt[0, 0], Rt[0, 1], Rt[0, 2], -(Rt[0, 0] * x + Rt[0, 1] * y + Rt[0, 2] * z)},
            {Rt[1, 0], Rt[1, 1], Rt[1, 2], -(Rt[1, 0] * x + Rt[1, 1] * y + Rt[1, 2] * z)},
            {Rt[2, 0], Rt[2, 1], Rt[2, 2], -(Rt[2, 0] * x + Rt[2, 1] * y + Rt[2, 2] * z)},
            {0, 0, 0, 1}
        };
        return inv;
    }
}