using System.Diagnostics;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ShootState : AnimationState
{
    [Export] private Ability bulletAbility;
    [Export] private PackedScene bulletTracer;
    [Export] private float bulletSpeed;
    [Export] private float gunRange;
    [Export] private VirtualCamera aimCamera;

    private CombatEntity hitEntity;
    private Node3D tracer;

    protected override void OnEnter()
    {
        base.OnEnter();
        Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2f;

        Vector3 rayOrigin    = aimCamera.Controller.ProjectRayOrigin(screenCenter);
        Vector3 rayDirection = aimCamera.Controller.ProjectRayNormal(screenCenter);
        Vector3 rayEnd       = rayOrigin + rayDirection * gunRange;
        
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollideWithBodies = true;
        query.CollideWithAreas = true;
        Dictionary hit = Context.spaceState.IntersectRay(query);

        if (hit.Count > 0)
        {
            TraceShot(hit["position"].AsVector3());
            hitEntity = NodeHelpers.GetChild<CombatEntity>(hit["collider"].As<Node>());
        }
        else
        {
            TraceShot(rayEnd);
        }
    }

    private void TraceShot(Vector3 end)
    {
        tracer = bulletTracer.Instantiate<Node3D>();
        tracer.TopLevel = true;
        AddChild(tracer);
        tracer.GlobalPosition = Context.modelRoot.GetBoneAttachment("LeftHand").GlobalPosition;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(tracer, "global_position", end, bulletSpeed);
        tween.TweenCallback(Callable.From(ApplyEffect)).SetDelay(bulletSpeed);
        tween.TweenCallback(Callable.From(DeleteBullet)).SetDelay(bulletSpeed + 0.3f);
    }

    private void ApplyEffect()
    {
        if (hitEntity != null)
        {
            bulletAbility?.Execute(Context.combatEntity, hitEntity);
            hitEntity = null;
        }
    }
    
    private void DeleteBullet()
    {
        RemoveChild(tracer);
        tracer.QueueFree();
    }
}