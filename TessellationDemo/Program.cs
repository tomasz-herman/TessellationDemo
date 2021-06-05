using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace TessellationDemo
{
    public class Program : GameWindow
    {
        private Shader shader;
        private Texture diffuse;
        private Texture height;
        private Texture normals;
        private Camera camera;
        private BezierPatch patch;
        
        public static void Main(string[] args)
        {
            using (Program program = new Program(GameWindowSettings.Default, NativeWindowSettings.Default))
            {
                program.Title = "Tesselation Demo";
                program.Size = new Vector2i(800, 600);
                program.Run();
            }
        }

        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            shader = new Shader(("shader.vert", ShaderType.VertexShader), ("shader.frag", ShaderType.FragmentShader));
            diffuse = new Texture("diffuse.png");
            height = new Texture("height.png");
            normals = new Texture("normals.png");
            camera = new PerspectiveCamera();
            patch = BezierPatch.Example();

            GL.ClearColor(0.4f, 0.7f, 0.9f, 1.0f);
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Disable(EnableCap.CullFace);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            shader.Dispose();
            diffuse.Dispose();
            normals.Dispose();
            height.Dispose();
            patch.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            camera.Aspect = (float) Size.X / Size.Y;
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState keyboard = KeyboardState.GetSnapshot();
            MouseState mouse = MouseState.GetSnapshot();
            
            camera.HandleInput(keyboard, mouse, (float)args.Time);

            if (keyboard.IsKeyDown(Keys.Escape)) Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            shader.Use();
            shader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
            patch.Mesh.Render();

            Context.SwapBuffers();
        }
    }
}