using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Assimp;
using Assimp.Configs;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace TessellationDemo;

public class ModelLoader
{
    private const string ModelsPath = "TessellationDemo.Resources.";
    private const PostProcessSteps DefaultParams = PostProcessSteps.Triangulate |
                                                   PostProcessSteps.GenerateNormals |
                                                   PostProcessSteps.JoinIdenticalVertices |
                                                   PostProcessSteps.FixInFacingNormals;

    public static Mesh Load(string path, PostProcessSteps ppSteps = DefaultParams, params PropertyConfig[] configs)
        {
            var importer = new AssimpContext();
            if (configs != null)
            {
                foreach (var config in configs) 
                    importer.SetConfig(config);
            }

            try
            {
                var assembly = Assembly.GetAssembly(typeof(ModelLoader));
                Stream stream = assembly.GetManifestResourceStream(ModelsPath + path);
                Scene scene = importer.ImportFileFromStream(stream, ppSteps, Path.GetExtension(path));
                stream.Dispose();
                
                if (scene == null)
                {
                    return null;
                }

                if (scene.Meshes.Count == 0)
                {
                    return null;
                }

                return ProcessMesh(scene.Meshes[0]);
            }
            catch (AssimpException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static Mesh ProcessMesh(Assimp.Mesh mesh)
        {
            var positions = ProcessVertices(mesh);
            var normals = ProcessNormals(mesh);
            var indices = ProcessIndices(mesh);

            return new Mesh(positions.ToArray(), normals.ToArray(), indices.ToArray(), PrimitiveType.Triangles);
        }

        private static List<float> ProcessVertices(Assimp.Mesh mesh)
        {
            List<float> vertices = new List<float>();
            foreach (var v in mesh.Vertices)
            {
                vertices.Add(v.X);
                vertices.Add(v.Y);
                vertices.Add(v.Z);
            }

            return vertices;
        }

        private static List<float> ProcessNormals(Assimp.Mesh mesh)
        {
            List<float> normals = new List<float>();
            foreach (var n in mesh.Normals)
            {
                normals.Add(n.X);
                normals.Add(n.Y);
                normals.Add(n.Z);
            }

            return normals;
        }

        private static List<int> ProcessIndices(Assimp.Mesh mesh)
        {
            List<int> indices = new List<int>();
            foreach (var f in mesh.Faces)
            {
                foreach (var ind in f.Indices)
                {
                    indices.Add(ind);
                }
            }

            return indices;
        }
}