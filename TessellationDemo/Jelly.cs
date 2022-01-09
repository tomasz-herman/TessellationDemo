using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Jelly : IDisposable
{
    public BezierCube Cube { get; }

    public Spring[] Springs { get; }
    public Spring[] ControlSprings { get; }
    public Frame ControlFrame { get; }
    public Frame ConstraintFrame { get; }

    public Ptr<State>[,,] States { get; set; }

    public float ControlElasticity = 1.0f;
    public float CollisionElasticity = 1.0f;
    public float Elasticity = 1.0f;
    public float Friction = 1.0f;
    public float Mass = 1.0f;
    
    public const float Constraint = 5;

    public Jelly()
    {
        Cube = new BezierCube();
        ControlFrame = new Frame(new Vector3(0), new Vector3(3));
        ConstraintFrame = new Frame(new Vector3(-Constraint), new Vector3(Constraint));
        (Springs, ControlSprings) = CreateSprings();
    }

    public void Update(float deltaTime)
    {
        foreach (var spring in Springs) spring.CalculateForces(Elasticity);

        foreach (var spring in ControlSprings) spring.CalculateForces(ControlElasticity);

        foreach (var state in States)
        {
            state.Get.CalculateFriction(Friction);
            state.Get.Integrate(deltaTime, Mass);
            state.Get.Advance();
        }
        
        FixCollisions();

        Cube.Update();
        ControlFrame.Update();
        foreach (var spring in Springs) spring.Update();
        foreach (var spring in ControlSprings) spring.Update();
    }

    public void FixCollisions()
    {
        foreach (var ptr in States)
        {
            State state = ptr.Get;
            Vector3 pos = state.Position.Get;
            Vector3 vel = state.Velocity.Get;
            bool collision = true;
            while (collision)
            {
                collision = false;
                switch (state.Position.Get.X)
                {
                    case < -Constraint:
                        pos.X = -Constraint;
                        vel.X = -vel.X * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                    case > Constraint:
                        pos.X = Constraint;
                        vel.X = -vel.X * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                }
                switch (state.Position.Get.Y)
                {
                    case < -Constraint:
                        pos.Y = -Constraint;
                        vel.Y = -vel.Y * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                    case > Constraint:
                        pos.Y = Constraint;
                        vel.Y = -vel.Y * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                }
                switch (state.Position.Get.Z)
                {
                    case < -Constraint:
                        pos.Z = -Constraint;
                        vel.Z = -vel.Z * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                    case > Constraint:
                        pos.Z = Constraint;
                        vel.Z = -vel.Z * CollisionElasticity;
                        state.Position.Get = pos;
                        state.Velocity.Get = vel;
                        collision = true;
                        break;
                }
            }
        }
    }

    public void Stress(float max)
    {
        Random random = new Random();
        foreach (var state in States)
        {
            float x = random.NextSingle() * 2 - 1;
            float y = random.NextSingle() * 2 - 1;
            float z = random.NextSingle() * 2 - 1;
            state.Get.Velocity.Get += new Vector3(x, y, z) * max;
        }
    }

    public void Dispose()
    {
        Cube?.Dispose();
        ControlFrame?.Dispose();
        ConstraintFrame?.Dispose();
        foreach (var spring in Springs)
        {
            spring?.Dispose();
        }
        foreach (var spring in ControlSprings)
        {
            spring?.Dispose();
        }
    }

    private (Spring[], Spring[]) CreateSprings()
    {
        States = new Ptr<State>[4, 4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    State state = new State { Position = Cube.Controls[i, j, k]};
                    States[i, j, k] = new Ptr<State>(state);
                }
            }
        }

        List<Spring> springs = new List<Spring>();
        List<Spring> controlSprings = new List<Spring>();
        float sqrt2 = (float) Math.Sqrt(2);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    springs.Add(new Spring(States[i, j, k], States[i + 1, j, k], 1));
                    springs.Add(new Spring(States[i + 1, j, k], States[i + 1, j + 1, k], 1));
                    springs.Add(new Spring(States[i + 1, j + 1, k], States[i, j + 1, k], 1));
                    springs.Add(new Spring(States[i, j + 1, k], States[i, j, k], 1));
                    
                    springs.Add(new Spring(States[i, j, k + 1], States[i + 1, j, k + 1], 1));
                    springs.Add(new Spring(States[i + 1, j, k + 1], States[i + 1, j + 1, k + 1], 1));
                    springs.Add(new Spring(States[i + 1, j + 1, k + 1], States[i, j + 1, k + 1], 1));
                    springs.Add(new Spring(States[i, j + 1, k + 1], States[i, j, k + 1], 1));
                    
                    springs.Add(new Spring(States[i, j, k], States[i, j, k + 1], 1));
                    springs.Add(new Spring(States[i + 1, j, k], States[i + 1, j, k + 1], 1));
                    springs.Add(new Spring(States[i, j + 1, k], States[i, j + 1, k + 1], 1));
                    springs.Add(new Spring(States[i + 1, j + 1, k], States[i + 1, j + 1, k + 1], 1));

                    
                    springs.Add(new Spring(States[i, j, k], States[i + 1, j, k + 1], sqrt2));
                    springs.Add(new Spring(States[i, j, k + 1], States[i + 1, j, k], sqrt2));
                    
                    springs.Add(new Spring(States[i + 1, j, k + 1], States[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(States[i + 1, j, k], States[i + 1, j + 1, k + 1], sqrt2));

                    springs.Add(new Spring(States[i, j, k], States[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(States[i + 1, j, k], States[i, j + 1, k], sqrt2));

                    springs.Add(new Spring(States[i, j, k], States[i, j + 1, k + 1], sqrt2));
                    springs.Add(new Spring(States[i, j, k + 1], States[i, j + 1, k], sqrt2));

                    springs.Add(new Spring(States[i, j, k + 1], States[i + 1, j + 1, k + 1], sqrt2));
                    springs.Add(new Spring(States[i, j + 1, k + 1], States[i + 1, j, k + 1], sqrt2));

                    springs.Add(new Spring(States[i, j + 1, k + 1], States[i + 1, j + 1, k], sqrt2));
                    springs.Add(new Spring(States[i, j + 1, k], States[i + 1, j + 1, k + 1], sqrt2));
                }
            }
        }
        
        controlSprings.Add(new Spring(ControlFrame.Controls[0, 0, 0], States[0, 0, 0], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[0, 0, 1], States[0, 0, 3], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[0, 1, 0], States[0, 3, 0], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[0, 1, 1], States[0, 3, 3], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[1, 0, 0], States[3, 0, 0], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[1, 0, 1], States[3, 0, 3], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[1, 1, 0], States[3, 3, 0], 0));
        controlSprings.Add(new Spring(ControlFrame.Controls[1, 1, 1], States[3, 3, 3], 0));

        return (springs.ToArray(), controlSprings.ToArray());
    }

    public struct State
    {
        public Ptr<Vector3> Position = new Ptr<Vector3>(new Vector3());
        public Ptr<Vector3> Velocity = new Ptr<Vector3>(new Vector3());
        public Ptr<Vector3> Force = new Ptr<Vector3>(new Vector3());
        public Ptr<Vector3> NextPosition = new Ptr<Vector3>(new Vector3());
        public Ptr<Vector3> NextVelocity = new Ptr<Vector3>(new Vector3());
        public Ptr<Vector3> NextForce = new Ptr<Vector3>(new Vector3());

        public void Advance()
        {
            Position.Get = NextPosition.Get;
            Velocity.Get = NextVelocity.Get;
            Force.Get = NextForce.Get;
            NextVelocity.Get = new Vector3();
            NextForce.Get = new Vector3();
            NextForce.Get = new Vector3();
        }

        public void CalculateFriction(float friction)
        {
            NextForce.Get += -Velocity.Get * friction;
        }

        public void Integrate(float dt, float mass)
        {
            NextVelocity.Get = Velocity.Get + dt * NextForce.Get / mass;
            NextPosition.Get = Position.Get + dt * NextVelocity.Get;
        }
    }
}