using Godot;

[GlobalClass]
public partial class DashState : State
{
    [Export] private float dashDuration;
    [Export] private float dashSpeed;
    
    private Vector3 direction = Vector3.Zero;
    private Timer timer = new Timer();
    
    public bool Started { get; private set; }

    public override void _Ready()
    {
        timer = new Timer();
        timer.Timeout += () =>
        {
            Started = false;
            Context.characterBody.Velocity = Vector3.Zero;
        };
        timer.OneShot = true;
        AddChild(timer);
    }


    protected override void OnEnter()
    {
        Vector3 input = InputHelpers.GetMovementInputAsVector3();
        if (input != Vector3.Zero)
        {
            direction = (Context.springArm.Transform.Basis * InputHelpers.GetMovementInputAsVector3())
                .Normalized();
        }
        else
        {
            direction = Context.modelRoot.Basis.Z;
        }

        direction.Y = 0;

        Started = true;
        timer.Start(dashDuration);
    }

    protected override void OnUpdatePhysics(float delta)
    {
        Context.characterBody.Velocity += direction * dashSpeed;
    }

    override protected void OnExit()
    {
        Context.characterBody.Velocity = Vector3.Zero;
    }
}
