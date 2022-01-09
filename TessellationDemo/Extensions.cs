using OpenTK.Mathematics;

namespace TessellationDemo
{
    public static class Extensions
    {
        public static Vector3[,] Slice(this Vector3[,,] array, int dim, int index, bool reverse = false)
        {
            switch (dim)
            {
                case 0:
                {
                    Vector3[,] result = new Vector3[4, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            result[i, j] = array[index, i, reverse ? j : 3 -  j];
                        }
                    }

                    return result;
                }
                case 1:
                {
                    Vector3[,] result = new Vector3[4, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            result[i, j] = array[reverse ? 3 - i : i, index, j];
                        }
                    }

                    return result;
                }
                case 2:
                {
                    Vector3[,] result = new Vector3[4, 4];
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            result[i, j] = array[reverse ? i : 3 - i, j, index];
                        }
                    }

                    return result;
                }
            }

            return null;
        }
    }
}