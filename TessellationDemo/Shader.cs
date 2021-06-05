using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace TessellationDemo
{
    public class Shader : IDisposable
    {
        private int _handle;
        private bool _disposed;

        public Shader(params (string path, ShaderType type)[] paths)
        {
            var sources = new List<(string source, ShaderType type)>();
            foreach (var (path, type) in paths)
            {
                sources.Add((ReadSource(path), type: type));
            }

            var shaders = new List<int>();
            foreach (var (source, type) in sources)
            {
                shaders.Add(CreateShader(source, type));
            }

            foreach (var shader in shaders)
            {
                CompileShader(shader);
            }
            
            CreateProgram(shaders.ToArray());
            
            CleanupShaders(shaders.ToArray());
        }

        private string ReadSource(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"TessellationDemo.Resources.{path}");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private int CreateShader(string source, ShaderType type)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            return shader;
        }

        private void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            
            var log = GL.GetShaderInfoLog(shader);
            if (log != string.Empty) Console.WriteLine(log);
        }

        private void CreateProgram(params int[] shaders)
        {
            _handle = GL.CreateProgram();

            foreach (var shader in shaders)
            {
                GL.AttachShader(_handle, shader);
            }
            
            GL.LinkProgram(_handle);
        }
        
        private void CleanupShaders(params int[] shaders)
        {
            foreach (var shader in shaders)
            {
                GL.DetachShader(_handle, shader);
                GL.DeleteShader(shader);
            }
        }
        
        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                GL.DeleteProgram(_handle);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            GL.DeleteProgram(_handle);
        }
    }
}