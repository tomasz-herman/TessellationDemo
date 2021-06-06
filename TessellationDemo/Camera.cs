using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TessellationDemo
{
    public abstract class Camera
    {
        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;
        public Vector3 Right { get; private set; } = Vector3.UnitX;
        public float Aspect { get; set; } = 16f / 9f;
        public Vector3 Position { get; private set; } = new Vector3(0, 0, 5);
        public float Sensitivity { get; set; } = 0.0015f;
        public float Speed { get; set; } = 5.0f;
        
        private float _pitch;
        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                UpdateVectors();
            }
        }
        
        private float _yaw = -MathHelper.PiOver2;
        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                UpdateVectors();
            }
        }

        public abstract void HandleInput(KeyboardState keyboard, MouseState mouse, float dt);
        public abstract Matrix4 GetViewMatrix();
        public abstract Matrix4 GetProjectionMatrix();
        
        public void Rotate(float dpitch, float dyaw)
        {
            _pitch += dpitch;
            _yaw += dyaw;
            UpdateVectors();
        }

        public void Move(float dx, float dy, float dz)
        {
            Position += Front * dz + Up * dy + Right * dx;
        }
        
        private void UpdateVectors()
        {
            Front = new Vector3
            {
                X = (float) Math.Cos(Pitch) * (float) Math.Cos(Yaw),
                Y = (float) Math.Sin(Pitch),
                Z = (float) Math.Cos(Pitch) * (float) Math.Sin(Yaw)
            };

            Front = Vector3.Normalize(Front);
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public Matrix4 GetProjectionViewMatrix()
        {
            return GetViewMatrix() * GetProjectionMatrix();
        }
    }
}