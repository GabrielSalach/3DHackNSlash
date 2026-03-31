
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class PhysicsCharacterBody : CharacterBody3D
{
    private Queue<Vector3> velocityQueue = new Queue<Vector3>();
    [Export]
    private float Friction = 5.0f;

    public void AddVelocity(Vector3 velocity)
    {
        velocityQueue.Enqueue(velocity);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        
        while (velocityQueue.Count > 0)
        {
            Vector3 nextVelocity = velocityQueue.Dequeue();
            velocity += nextVelocity;
        }
        
        Velocity = velocity;
        MoveAndSlide();
    }

    public void ApplyFriction(float friction)
    {
        Vector3 velocity = Vector3.Zero;
        velocity.X = Mathf.MoveToward(Velocity.X, 0, 0.1f);
        velocity.Z = Mathf.MoveToward(Velocity.Z, 0, 0.1f);
        velocity = -velocity.Normalized() * friction;
        AddVelocity(velocity);
    }
}
