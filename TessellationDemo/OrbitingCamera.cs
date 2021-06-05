using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TessellationDemo
{
    public class OrbitingCamera : Camera
    {
        private float _fov = MathHelper.PiOver3;
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set => _fov = MathHelper.DegreesToRadians(value);
        }

        public Vector3 Center { get; set; } = Vector3.Zero;
        
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 100f;
        public float Sensitivity { get; set; } = 0.15f;
        
        public override void HandleInput(KeyboardState keyboard, MouseState mouse, float dt)
        {
            if (mouse.IsButtonDown(MouseButton.Button1) && mouse.WasButtonDown(MouseButton.Button1))
            {
                Vector2 delta = mouse.Delta;
                // Rotate
            }
            if (mouse.IsButtonDown(MouseButton.Button2) && mouse.WasButtonDown(MouseButton.Button2))
            {
                Vector2 delta = mouse.Delta;
                // Move
            }
        }

        public override Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public override Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, Aspect, NearPlane, FarPlane);
        }
    }
}