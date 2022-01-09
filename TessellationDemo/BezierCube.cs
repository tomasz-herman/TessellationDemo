using System;
using OpenTK.Mathematics;

namespace TessellationDemo
{
    public class BezierCube : IDisposable
    {
        public Vector3[,,] Controls { get; } = new Vector3[4, 4, 4];
        
        public BezierPatch[] Patches { get; } = new BezierPatch[6];

        public BezierCube()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Controls[i, j, k] = new Vector3(i, j, k);
                    }
                }
            }

            Controls[0, 0, 0] = new Vector3(-1, 1, -1);
            Controls[3, 3, 3] = new Vector3(4, 2, 4);

            Patches[0] = new BezierPatch(Controls.Slice(0, 0));
            Patches[1] = new BezierPatch(Controls.Slice(0, 3, true));
            Patches[2] = new BezierPatch(Controls.Slice(1, 0));
            Patches[3] = new BezierPatch(Controls.Slice(1, 3, true));
            Patches[4] = new BezierPatch(Controls.Slice(2, 0));
            Patches[5] = new BezierPatch(Controls.Slice(2, 3, true));
        }

        public void Dispose()
        {
            foreach (var patch in Patches)
            {
                patch?.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}