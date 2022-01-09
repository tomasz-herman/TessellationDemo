using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace TessellationDemo
{
    public class Mesh : IDisposable
    {
        public int Vao { get; private set; }
        private List<int> Vbos { get; } = new ();
        public PrimitiveType Type { get; }
        public int Count { get; }

        public Mesh(float[] positions, int[] indices, PrimitiveType type)
        {
            Type = type;
            Count = indices.Length;
            Load(positions, null, indices);
        }
        
        public Mesh(float[] positions, float[] normals, int[] indices, PrimitiveType type)
        {
            Type = type;
            Count = indices.Length;
            Load(positions, normals, indices);
        }
        
        public void Load(float[] positions, float[] normals, int[] indices) {
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);
            LoadData(positions, 0, 3);
            if(normals != null) LoadData(normals, 1, 3);
            LoadIndices(indices);
            GL.BindVertexArray(0);
        }
        
        public void LoadData(float[] data, int index, int size) {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Vbos.Add(vbo);
        }

        public void UpdateData(float[] data, int index)
        {
            var vbo = Vbos[index];
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, data.Length * sizeof(float), data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        
        public void LoadIndices(int[] data) {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(int), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Vbos.Add(vbo);
        }

        public void Render()
        {
            GL.BindVertexArray(Vao);
            GL.DrawElements(Type, Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(Vao);
            foreach (var vbo in Vbos)
            {
                GL.DeleteBuffer(vbo);
            }
            Vbos.Clear();
            GC.SuppressFinalize(this);
        }
    }
}