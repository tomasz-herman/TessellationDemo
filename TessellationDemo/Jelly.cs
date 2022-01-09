using System;

namespace TessellationDemo;

public class Jelly : IDisposable
{
    public BezierCube Cube { get; }

    public Jelly()
    {
        Cube = new BezierCube();
    }

    public void Dispose()
    {
        Cube?.Dispose();
    }
}