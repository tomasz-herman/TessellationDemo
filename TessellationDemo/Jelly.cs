using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Jelly : IDisposable
{
    public BezierCube Cube { get; }

    public Spring[] Springs { get; }

    public Jelly()
    {
        Cube = new BezierCube();
        Springs = CreateSprings();
    }

    public void Update()
    {
        Cube.Update();
        foreach (var spring in Springs)
        {
            spring.Update();
        }
    }

    public void Dispose()
    {
        Cube?.Dispose();
        foreach (var spring in Springs)
        {
            spring?.Dispose();
        }
    }

    private Spring[] CreateSprings()
    {
        List<Spring> springs = new List<Spring>();
        float sqrt2 = (float) Math.Sqrt(2);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    springs.Add(new Spring(Cube.Controls[i, j, k], Cube.Controls[i + 1, j, k], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k], Cube.Controls[i + 1, j + 1, k], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j + 1, k], Cube.Controls[i, j + 1, k], 1));
                    springs.Add(new Spring(Cube.Controls[i, j + 1, k], Cube.Controls[i, j, k], 1));
                    
                    springs.Add(new Spring(Cube.Controls[i, j, k + 1], Cube.Controls[i + 1, j, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k + 1], Cube.Controls[i + 1, j + 1, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j + 1, k + 1], Cube.Controls[i, j + 1, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i, j + 1, k + 1], Cube.Controls[i, j, k + 1], 1));
                    
                    springs.Add(new Spring(Cube.Controls[i, j, k], Cube.Controls[i, j, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k], Cube.Controls[i + 1, j, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i, j + 1, k], Cube.Controls[i, j + 1, k + 1], 1));
                    springs.Add(new Spring(Cube.Controls[i + 1, j + 1, k], Cube.Controls[i + 1, j + 1, k + 1], 1));

                    
                    springs.Add(new Spring(Cube.Controls[i, j, k], Cube.Controls[i + 1, j, k + 1], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i, j, k + 1], Cube.Controls[i + 1, j, k], sqrt2));
                    
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k + 1], Cube.Controls[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k], Cube.Controls[i + 1, j + 1, k + 1], sqrt2));

                    springs.Add(new Spring(Cube.Controls[i, j, k], Cube.Controls[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i + 1, j, k], Cube.Controls[i, j + 1, k], sqrt2));

                    springs.Add(new Spring(Cube.Controls[i, j, k], Cube.Controls[i, j + 1, k + 1], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i, j, k + 1], Cube.Controls[i, j + 1, k], sqrt2));

                    springs.Add(new Spring(Cube.Controls[i, j, k + 1], Cube.Controls[i + 1, j + 1, k + 1], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i, j + 1, k + 1], Cube.Controls[i + 1, j, k + 1], sqrt2));

                    springs.Add(new Spring(Cube.Controls[i, j + 1, k + 1], Cube.Controls[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(Cube.Controls[i, j + 1, k], Cube.Controls[i + 1, j + 1, k + 1], sqrt2));
                }
            }
        }

        return springs.ToArray();
    }
}