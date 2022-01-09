using Dear_ImGui_Sample;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
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
        private Jelly jelly;
        private Vector3 light;
        private ImGuiController controller;

        private bool edgesOnly;
        
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
            jelly = new Jelly();
            light = new Vector3(0, 5, 0);
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);

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
            jelly.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            camera.Aspect = (float) Size.X / Size.Y;
            GL.Viewport(0, 0, Size.X, Size.Y);
            controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            controller.Update(this, (float) args.Time);
            
            if(ImGui.GetIO().WantCaptureMouse) return;

            KeyboardState keyboard = KeyboardState.GetSnapshot();
            MouseState mouse = MouseState.GetSnapshot();
            
            camera.HandleInput(keyboard, mouse, (float)args.Time);

            if (keyboard.IsKeyDown(Keys.Escape)) Close();
            if (keyboard.IsKeyPressed(Keys.F)) edgesOnly = !edgesOnly;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, edgesOnly ? PolygonMode.Line : PolygonMode.Fill);

            bezierShader.Use();
            bezierShader.LoadFloat3("cameraPos", camera.Position);
            bezierShader.LoadFloat3("lightPos", light);
            bezierShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            foreach (var patch in jelly.Cube.Patches)
            {
                patch.Patch.Render();
            }

            GL.LineWidth(2);
            defaultShader.Use();
            defaultShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
            foreach (var patch in jelly.Cube.Patches)
            {
                patch.Mesh.Render();
            }
            GL.LineWidth(1);
            
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            ImGui.ShowDemoWindow();
            controller.Render();

            Context.SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            controller.MouseScroll(e.Offset);
        }
    }
}