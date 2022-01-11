using System;
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
        private Shader defaultShader, bezierShader, deformationShader;
        private Texture diffuse;
        private Texture height;
        private Texture normals;
        private Camera camera;
        private Jelly jelly;
        private Vector3 light;
        private ImGuiController controller;

        private bool showSprings;
        private bool showControlFrame = true;
        private bool showConstraintFrame = true;
        private bool showCube = true;
        private bool showTeapot;
        private bool showBunny;
        private float distress = 1;
        
        public static void Main(string[] args)
        {
            using (Program program = new Program(GameWindowSettings.Default, NativeWindowSettings.Default))
            {
                program.Title = "Jelly";
                program.Size = new Vector2i(1280, 800);
                program.Run();
            }
        }

        public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            defaultShader = new Shader(("shader.vert", ShaderType.VertexShader), ("shader.frag", ShaderType.FragmentShader));
            deformationShader = new Shader(("deformed.vert", ShaderType.VertexShader), ("deformed.frag", ShaderType.FragmentShader));
            bezierShader = new Shader(("patch.vert", ShaderType.VertexShader), ("patch.frag", ShaderType.FragmentShader), 
                ("patch.tesc", ShaderType.TessControlShader), ("patch.tese", ShaderType.TessEvaluationShader));
            camera = new PerspectiveCamera();
            jelly = new Jelly();
            light = new Vector3(0, 500, 0);
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
            deformationShader.Dispose();
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
            
            jelly.Update((float) args.Time);
            
            if(ImGui.GetIO().WantCaptureMouse) return;

            KeyboardState keyboard = KeyboardState.GetSnapshot();
            MouseState mouse = MouseState.GetSnapshot();
            
            camera.HandleInput(keyboard, mouse, (float)args.Time);
            if(mouse.IsButtonDown(MouseButton.Button2)) HandleFrameTranslation(mouse);
            if(mouse.IsButtonDown(MouseButton.Button3)) HandleFrameRotation(mouse);

            if (keyboard.IsKeyDown(Keys.Escape)) Close();
        }

        private void HandleFrameTranslation(MouseState state)
        {
            var (dx, dy) = state.Delta * 0.01f;
            jelly.ControlFrame.Translate(camera.Right * dx - camera.Up * dy);
        }
        
        private void HandleFrameRotation(MouseState state)
        {
            Vector2 delta = state.Delta;
            if(Math.Abs(delta.Length) < 1e-5) return;
            float angle = delta.Length * 0.01f;
            Vector3 axis = (-delta.X * camera.Up - delta.Y * camera.Right).Normalized();
            jelly.ControlFrame.Rotate(axis, angle);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (showCube)
            {
               bezierShader.Use();
               bezierShader.LoadFloat3("cameraPos", camera.Position);
               bezierShader.LoadFloat3("lightPos", light);
               bezierShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
               GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
               foreach (var patch in jelly.Cube.Patches)
               {
                   patch.Patch.Render();
               } 
            }
            
            if (showTeapot || showBunny)
            {
                deformationShader.Use();
                deformationShader.LoadFloat3("cameraPos", camera.Position);
                deformationShader.LoadFloat3("lightPos", light);
                deformationShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
                var points = jelly.Cube.Controls;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            deformationShader.LoadFloat3($"p[{i}][{j}][{k}]", points[i, j, k].Get);
                        }
                    }
                }
                if(showTeapot) jelly.Teapot.Render();
                if(showBunny) jelly.Bunny.Render();
            }

            if (showSprings)
            {
                GL.LineWidth(2);
                defaultShader.Use();
                defaultShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
                foreach (var spring in jelly.Springs)
                {
                    spring.Mesh.Render();
                }
                GL.LineWidth(1);
            }

            if (showControlFrame)
            {
                GL.LineWidth(3);
                defaultShader.Use();
                defaultShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
                jelly.ControlFrame.Mesh.Render();
                foreach (var spring in jelly.ControlSprings)
                {
                    spring.Mesh.Render();
                }
                GL.LineWidth(1);
            }
            
            if (showConstraintFrame)
            {
                GL.LineWidth(5);
                defaultShader.Use();
                defaultShader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
                jelly.ConstraintFrame.Mesh.Render();
                GL.LineWidth(1);
            }
            
            RenderGui();

            Context.SwapBuffers();
        }

        private void RenderGui()
        {
            ImGui.Begin("Options");
            ImGui.SliderFloat("Distress", ref distress, 0.01f, 100);
            ImGui.SliderFloat("Mass", ref jelly.Mass, 0.01f, 100);
            ImGui.SliderFloat("Elasticity", ref jelly.Elasticity, 0.01f, 100);
            ImGui.SliderFloat("Control Elasticity", ref jelly.ControlElasticity, 0.01f, 100);
            ImGui.SliderFloat("Collision Elasticity", ref jelly.CollisionElasticity, 0, 1);
            ImGui.SliderFloat("Friction", ref jelly.Friction, 0, 100);
            ImGui.Checkbox("Show Cube", ref showCube);
            ImGui.Checkbox("Show Teapot", ref showTeapot);
            ImGui.Checkbox("Show Bunny", ref showBunny);
            ImGui.Checkbox("Show Springs", ref showSprings);
            ImGui.Checkbox("Show Control Frame", ref showControlFrame);
            ImGui.Checkbox("Show Constraint Frame", ref showConstraintFrame);
            if (ImGui.Button("Stress"))
            {
                jelly.Stress(distress);
            }
            ImGui.End();
            
            controller.Render();
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