using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Spring : IDisposable
{
    public Ptr<Vector3> P0 { get; }
    public Ptr<Vector3> P1 { get; }
    public float Length { get; }
    public Mesh Mesh { get; }

    public Spring(Ptr<Vector3> p0, Ptr<Vector3> p1, float length)
    {
        P0 = p0;
        P1 = p1;
        Length = length;

        Mesh = new Mesh(
            new[] { p0.Get.X, p0.Get.Y, p0.Get.Z, p1.Get.X, p1.Get.Y, p1.Get.Z }, 
            new[] { 0, 1 }, 
            PrimitiveType.Lines);
    }

    public void Update()
    {
        Mesh.UpdateData(new[] { P0.Get.X, P0.Get.Y, P0.Get.Z, P1.Get.X, P1.Get.Y, P1.Get.Z }, 0);
    }

    public void Dispose()
    {
        Mesh?.Dispose();
        GC.SuppressFinalize(this);
    }
}