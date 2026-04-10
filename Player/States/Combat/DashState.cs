using Godot;

[GlobalClass]
public partial class DashState : State
{
    
    [Export] private float dashDuration;
    [Export] private float dashSpeed;
    
    private Vector3 direction = Vector3.Zero;
    private Timer timer;

    private bool completed;

    protected override void OnEnter()
    {
        completed = false;
        
        if (timer == null)
        {
            timer = new Timer();
            timer.Timeout += () =>
            {
                completed = true;
            };
            timer.OneShot = true;
            AddChild(timer);
        }
        
        Vector3 input = InputHelpers.GetMovementInputAsVector3();
        if (input != Vector3.Zero)
        {
            direction = Context.MovementDirection;
        }
        else
        {
            direction = Context.modelRoot.Basis.Z;
        }

        direction.Y = 0;
        timer.Start(dashDuration);
    }

    protected override void OnUpdatePhysics(float delta)
    {
        Context.characterBody.Velocity += direction * dashSpeed;
    }

    protected override void OnExit()
    {
        Context.characterBody.Velocity = Vector3.Zero;
    }

    public override bool IsCompleted() => completed;
}
