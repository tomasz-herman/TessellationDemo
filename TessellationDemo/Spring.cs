using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Spring : IDisposable
{
    public Ptr<Jelly.State> P0 { get; }
    public Ptr<Jelly.State> P1 { get; }
    public float Length { get; }
    public Mesh Mesh { get; }

    public Spring(Ptr<Jelly.State> p0, Ptr<Jelly.State> p1, float length)
    {
        P0 = p0;
        P1 = p1;
        Length = length;

        Mesh = new Mesh(
            new[] {
                p0.Get.Position.Get.X, p0.Get.Position.Get.Y, p0.Get.Position.Get.Z, 
                p1.Get.Position.Get.X, p1.Get.Position.Get.Y, p1.Get.Position.Get.Z 
            }, new[] { 0, 1 }, 
            PrimitiveType.Lines);
    }

    public void CalculateForces(float elasticity)
    {
        float l = Vector3.Distance(P0.Get.Position.Get, P1.Get.Position.Get) - Length;
        float force = -elasticity * l;
        if(Math.Abs(l) < 1e-5f) return;
        Vector3 p01 = (P0.Get.Position.Get - P1.Get.Position.Get).Normalized();
        Vector3 p10 = -p01;
        P0.Get.NextForce.Get += p01 * force;
        P1.Get.NextForce.Get += p10 * force;
    }

    public void Update()
    {
        Mesh.UpdateData(new[] { 
            P0.Get.Position.Get.X, P0.Get.Position.Get.Y, P0.Get.Position.Get.Z, 
            P1.Get.Position.Get.X, P1.Get.Position.Get.Y, P1.Get.Position.Get.Z 
        }, 0);
    }

    public void Dispose()
    {
        Mesh?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected bool Equals(Spring other)
    {
        return (Equals(P0, other.P0) && Equals(P1, other.P1)) || (Equals(P1, other.P0) && Equals(P0, other.P1));
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Spring)obj);
    }

    public override int GetHashCode()
    {
        return P0.GetHashCode() ^ P1.GetHashCode();
    }
}