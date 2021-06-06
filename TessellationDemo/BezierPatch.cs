using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TessellationDemo
{
    public class BezierPatch : IDisposable
    {
        private Vector3[,] _surface;
        private List<Vector3> _points;
        public Mesh Patch { get; private set; }
        public Mesh Mesh { get; private set; }

        public static BezierPatch Create(Func<int, int, float> heights = null)
        {
            heights ??= (i, j) => 0;
            
            BezierPatch patch = new BezierPatch();

            patch._surface = new Vector3[13, 13];

            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    patch._surface[j, i] = new Vector3(i - 6, heights(j, i), j - 6);
                }
            }

            patch._points = new List<Vector3>();

            for (int i = 0; i < 12; i += 3)
            {
                for (int j = 0; j < 12; j += 3)
                {
                    patch._points.AddRange(
                        new[]
                        {
                            patch._surface[i, j],
                            patch._surface[i + 1, j],
                            patch._surface[i + 2, j],
                            patch._surface[i + 3, j],
                            
                            patch._surface[i, j + 1],
                            patch._surface[i + 1, j + 1],
                            patch._surface[i + 2, j + 1],
                            patch._surface[i + 3, j + 1],
                            
                            patch._surface[i, j + 2],
                            patch._surface[i + 1, j + 2],
                            patch._surface[i + 2, j + 2],
                            patch._surface[i + 3, j + 2],
                            
                            patch._surface[i, j + 3],
                            patch._surface[i + 1, j + 3],
                            patch._surface[i + 2, j + 3],
                            patch._surface[i + 3, j + 3]
                        }
                    );
                }
            }
            
            List<float> patchPositions = patch._points.SelectMany(vec => new []{ vec.X, vec.Y, vec.Z}).ToList();
            List<int> patchIndices = Enumerable.Range(0, patch._points.Count).ToList();
            
            patch.Patch = new Mesh(
                patchPositions.ToArray(), 
                patchIndices.ToArray(), 
                PrimitiveType.Patches
            );

            List<float> meshPositions = new List<float>();
            List<int> meshIndices = new List<int>();
            for (int i = 0; i < patch._surface.GetLength(0); i++)
            for (int j = 0; j < patch._surface.GetLength(1); j++)
            {
                Vector3 pos = patch._surface[i, j];
                meshPositions.AddRange(new[] {pos.X, pos.Y, pos.Z});
                
                int idx = j + i * patch._surface.GetLength(1);

                if (j + 1 < patch._surface.GetLength(1))
                {
                    meshIndices.AddRange(new[]{idx, idx + 1});
                }

                if (i + 1 < patch._surface.GetLength(0))
                {
                    meshIndices.AddRange(new[]{idx, idx + patch._surface.GetLength(1)});
                }
            }
            
            patch.Mesh = new Mesh(
                meshPositions.ToArray(),
                meshIndices.ToArray(),
                PrimitiveType.Lines
            );

            return patch;
        }

        public void Dispose()
        {
            Patch?.Dispose();
            Mesh?.Dispose();
            _surface = null;
            _points.Clear();
            GC.SuppressFinalize(this);
        }
    }
}