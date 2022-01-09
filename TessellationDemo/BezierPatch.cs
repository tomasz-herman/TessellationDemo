using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TessellationDemo
{
    public class BezierPatch : IDisposable
    {
        private Vector3[,] controls;
        
        public Mesh Patch { get; private set; }
        public Mesh Mesh { get; private set; }

        public BezierPatch(Vector3[,] controls)
        {
            this.controls = controls;

            List<int> patchIndices = new List<int>();

            patchIndices.AddRange(
                new[]
                {
                    0 * 4 + 0,
                    1 * 4 + 0,
                    2 * 4 + 0,
                    3 * 4 + 0,

                    0 * 4 + 1,
                    1 * 4 + 1,
                    2 * 4 + 1,
                    3 * 4 + 1,
                    
                    0 * 4 + 2,
                    1 * 4 + 2,
                    2 * 4 + 2,
                    3 * 4 + 2,
                    
                    0 * 4 + 3,
                    1 * 4 + 3,
                    2 * 4 + 3,
                    3 * 4 + 3
                }
            );

            List<float> positions = new List<float>();
            List<int> meshIndices = new List<int>();
            for (int i = 0; i < controls.GetLength(0); i++)
            for (int j = 0; j < controls.GetLength(1); j++)
            {
                Vector3 pos = controls[i, j];
                positions.AddRange(new[] {pos.X, pos.Y, pos.Z});
                
                int idx = j + i * controls.GetLength(1);

                if (j + 1 < controls.GetLength(1))
                {
                    meshIndices.AddRange(new[]{idx, idx + 1});
                }

                if (i + 1 < controls.GetLength(0))
                {
                    meshIndices.AddRange(new[]{idx, idx + controls.GetLength(1)});
                }
            }
            
            Mesh = new Mesh(
                positions.ToArray(),
                meshIndices.ToArray(),
                PrimitiveType.Lines
            );
            
            Patch = new Mesh(
                positions.ToArray(), 
                patchIndices.ToArray(), 
                PrimitiveType.Patches
            );
        }

        public void Dispose()
        {
            Patch?.Dispose();
            Mesh?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}