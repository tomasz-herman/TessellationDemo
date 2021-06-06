﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace TessellationDemo
{
    public class Program : GameWindow
    {
        private Shader defaultShader, bezierShader;
        private Texture diffuse;
        private Texture height;
        private Texture normals;
        private Camera camera;
        private BezierPatch flatPatch;
        private BezierPatch bumpyPatch;
        private Vector3 light;

        private bool showMesh = false;
        private bool edgesOnly = false;
        private bool showFlatPatch = false;
        
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

            defaultShader = new Shader(("shader.vert", ShaderType.VertexShader), ("shader.frag", ShaderType.FragmentShader));
            bezierShader = new Shader(("patch.vert", ShaderType.VertexShader), ("patch.frag", ShaderType.FragmentShader), 
                ("patch.tesc", ShaderType.TessControlShader), ("patch.tese", ShaderType.TessEvaluationShader));
            diffuse = new Texture("diffuse.png");
            height = new Texture("height.png");
            normals = new Texture("normals.png");
            camera = new PerspectiveCamera();
            flatPatch = BezierPatch.Create();
            bumpyPatch = BezierPatch.Create((i, j) => i % 3 == 0 ? 0 : (i / 3 % 2 == 0 ? 1 : -1));
            light = new Vector3(0, 5, 0);

            GL.ClearColor(0.4f, 0.7f, 0.9f, 1.0f);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            defaultShader.Dispose();
            bezierShader.Dispose();
            diffuse.Dispose();
            normals.Dispose();
            height.Dispose();
            flatPatch.Dispose();
            bumpyPatch.Dispose();
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
            if (keyboard.IsKeyPressed(Keys.R)) showMesh = !showMesh;
            if (keyboard.IsKeyPressed(Keys.F)) edgesOnly = !edgesOnly;
            if (keyboard.IsKeyPressed(Keys.T)) showFlatPatch = !showFlatPatch;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, edgesOnly ? PolygonMode.Line : PolygonMode.Fill);

            bezierShader.Use();
            bezierShader.LoadFloat3("cameraPos", camera.Position);
            bezierShader.LoadFloat3("lightPos", light);
            height.Use(TextureUnit.Texture0);
            bezierShader.LoadInteger("heightTex", 0);
            diffuse.Use(TextureUnit.Texture1);
            bezierShader.LoadInteger("colorTex", 1);
            normals.Use(TextureUnit.Texture2);
            bezierShader.LoadInteger("normalTex", 2);
            bezierShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            if(showFlatPatch) flatPatch.Patch.Render();
            else bumpyPatch.Patch.Render();

            if (showMesh)
            {
                defaultShader.Use();
                defaultShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
                if(showFlatPatch) flatPatch.Mesh.Render();
                else bumpyPatch.Mesh.Render();
            }

            Context.SwapBuffers();
        }
    }
}