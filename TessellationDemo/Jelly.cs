using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace TessellationDemo;

public class Jelly : IDisposable
{
    public BezierCube Cube { get; }

    public Spring[] Springs { get; }

    public Ptr<State>[,,] States { get; set; }

    public float Elasticity = 1.0f;
    public float Friction = 1.0f;
    public float Mass = 1.0f;

    public Jelly()
    {
        Cube = new BezierCube();
        Springs = CreateSprings();
    }

    public void Update(float deltaTime)
    {
        foreach (var spring in Springs)
        {
            spring.CalculateForces(Elasticity);
        }

        foreach (var state in States)
        {
            state.Get.CalculateFriction(Friction);
            state.Get.Integrate(deltaTime, Mass);
            state.Get.Advance();
        }

        Cube.Update();
        foreach (var spring in Springs)
        {
            spring.Update();
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
        foreach (var spring in Springs)
        {
            spring?.Dispose();
        }
    }

    private Spring[] CreateSprings()
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

        return springs.ToArray();
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