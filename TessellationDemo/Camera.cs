using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TessellationDemo
{
    public abstract class Camera
    {
        public Vector3 Front { get; } = -Vector3.UnitZ;
        public Vector3 Up { get; } = Vector3.UnitY;
        public Vector3 Right { get; } = Vector3.UnitX;
        
        public float Aspect { get; set; } = 16f / 9f;

        public Vector3 Position { get; } = Vector3.UnitZ;

        public abstract void HandleInput(KeyboardState keyboard, MouseState mouse, float dt);
        public abstract Matrix4 GetViewMatrix();
        public abstract Matrix4 GetProjectionMatrix();
    }
}