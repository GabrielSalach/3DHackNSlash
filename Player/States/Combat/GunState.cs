using Godot;

[GlobalClass]
public partial class GunState : State
{
    [Export] private OrbitalCamera aimCamera;
    [Export] private float friction = 30.0f;
    [Export] private Control crosshair;
    
    [ExportCategory("Child States")]
    [Export] private MovementState aimedMovementState;
    [Export] private State aimedIdleState;
    [Export] private ShootState shootState;

    protected override State GetInitialState() => aimedIdleState;

    protected override void SetupTransitions()
    {
        AddTransition(aimedIdleState, shootState, () => Context.actionMap["shoot"].IsJustPressed);
        AddTransition(aimedIdleState, aimedMovementState,() => Context.MovementDirection.Length() > 0);
        
        AddTransition(aimedMovementState, shootState, () => Context.actionMap["shoot"].IsJustPressed);
        AddTransition(aimedMovementState, aimedIdleState, () => Context.MovementDirection.Length() <= 0);
        
        AddTransition(shootState, aimedIdleState, () => shootState.IsCompleted);
    }

    protected override void OnEnter()
    {
        Context.characterBody.Velocity = Vector3.Zero;
        aimCamera.Priority = 1;
        crosshair.Visible = true;
    }

    protected override void OnUpdate(float delta)
    {
        Vector3 cameraForward = aimCamera.GlobalTransform.Basis.Z;
        cameraForward.Y = 0; 
        cameraForward = cameraForward.Normalized();

        if (cameraForward.LengthSquared() > 0.001f)
            Context.modelRoot.LookAt(Context.modelRoot.GlobalPosition + cameraForward, Vector3.Up);
    }
    
    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 vel = Context.characterBody.Velocity;
        vel.X = Mathf.MoveToward(vel.X, 0, friction * delta);
        vel.Z = Mathf.MoveToward(vel.Z, 0, friction * delta);
        Context.characterBody.Velocity = vel;
    }

    protected override void OnExit()
    {
        crosshair.Visible = false;
        aimCamera.Priority = -1;
    }
}
