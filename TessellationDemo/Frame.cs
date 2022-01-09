using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Frame : IDisposable
{
    public Ptr<Jelly.State>[,,] Controls { get; } = new Ptr<Jelly.State>[2, 2, 2];
    public Mesh Mesh { get; }

    public Frame(Vector3 l, Vector3 u)
    {
        Controls[0, 0, 0] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(l.X, l.Y, l.Z)) });
        Controls[0, 0, 1] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(l.X, l.Y, u.Z)) });
        Controls[0, 1, 0] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(l.X, u.Y, l.Z)) });
        Controls[0, 1, 1] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(l.X, u.Y, u.Z)) });
        Controls[1, 0, 0] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(u.X, l.Y, l.Z)) });
        Controls[1, 0, 1] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(u.X, l.Y, u.Z)) });
        Controls[1, 1, 0] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(u.X, u.Y, l.Z)) });
        Controls[1, 1, 1] = new Ptr<Jelly.State>(new Jelly.State { Position = new Ptr<Vector3>(new Vector3(u.X, u.Y, u.Z)) });

        Mesh = new Mesh(
            new[]
            {
                Controls[0, 0, 0].Get.Position.Get.X, Controls[0, 0, 0].Get.Position.Get.Y, Controls[0, 0, 0].Get.Position.Get.Z,
                Controls[0, 0, 1].Get.Position.Get.X, Controls[0, 0, 1].Get.Position.Get.Y, Controls[0, 0, 1].Get.Position.Get.Z,
                Controls[0, 1, 0].Get.Position.Get.X, Controls[0, 1, 0].Get.Position.Get.Y, Controls[0, 1, 0].Get.Position.Get.Z,
                Controls[0, 1, 1].Get.Position.Get.X, Controls[0, 1, 1].Get.Position.Get.Y, Controls[0, 1, 1].Get.Position.Get.Z,
                Controls[1, 0, 0].Get.Position.Get.X, Controls[1, 0, 0].Get.Position.Get.Y, Controls[1, 0, 0].Get.Position.Get.Z,
                Controls[1, 0, 1].Get.Position.Get.X, Controls[1, 0, 1].Get.Position.Get.Y, Controls[1, 0, 1].Get.Position.Get.Z,
                Controls[1, 1, 0].Get.Position.Get.X, Controls[1, 1, 0].Get.Position.Get.Y, Controls[1, 1, 0].Get.Position.Get.Z,
                Controls[1, 1, 1].Get.Position.Get.X, Controls[1, 1, 1].Get.Position.Get.Y, Controls[1, 1, 1].Get.Position.Get.Z
            },
            new[] { 0, 1, 0, 2, 0, 4, 1, 3, 1, 5, 2, 3, 2, 6, 3, 7, 4, 5, 4, 6, 5, 7, 6, 7 },
            PrimitiveType.Lines
        );
    }

    public void Update()
    {
        Mesh.UpdateData(new[]
        {
            Controls[0, 0, 0].Get.Position.Get.X, Controls[0, 0, 0].Get.Position.Get.Y, Controls[0, 0, 0].Get.Position.Get.Z,
            Controls[0, 0, 1].Get.Position.Get.X, Controls[0, 0, 1].Get.Position.Get.Y, Controls[0, 0, 1].Get.Position.Get.Z,
            Controls[0, 1, 0].Get.Position.Get.X, Controls[0, 1, 0].Get.Position.Get.Y, Controls[0, 1, 0].Get.Position.Get.Z,
            Controls[0, 1, 1].Get.Position.Get.X, Controls[0, 1, 1].Get.Position.Get.Y, Controls[0, 1, 1].Get.Position.Get.Z,
            Controls[1, 0, 0].Get.Position.Get.X, Controls[1, 0, 0].Get.Position.Get.Y, Controls[1, 0, 0].Get.Position.Get.Z,
            Controls[1, 0, 1].Get.Position.Get.X, Controls[1, 0, 1].Get.Position.Get.Y, Controls[1, 0, 1].Get.Position.Get.Z,
            Controls[1, 1, 0].Get.Position.Get.X, Controls[1, 1, 0].Get.Position.Get.Y, Controls[1, 1, 0].Get.Position.Get.Z,
            Controls[1, 1, 1].Get.Position.Get.X, Controls[1, 1, 1].Get.Position.Get.Y, Controls[1, 1, 1].Get.Position.Get.Z
        }, 0);
    }
    
    public void Dispose()
    {
        Mesh?.Dispose();
        GC.SuppressFinalize(this);
    }
}