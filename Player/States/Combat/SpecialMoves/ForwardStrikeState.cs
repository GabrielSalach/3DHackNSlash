using Godot;

[GlobalClass]
public partial class ForwardStrikeState : AttackState
{
    public double timeLeft;
    private bool completed;
    private Timer timer;
    public Vector3 direction;

    [Export] private float dashSpeed;
    
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
        

        timer.Start(timeLeft);
    }

    protected override void OnUpdate(float delta)
    {
        Context.characterBody.Velocity = direction * Context.animator.GetRootMotionPositionAccumulator().Length() * dashSpeed;
    }
    
    protected override void OnExit()
    {
        Context.characterBody.Velocity = Vector3.Zero;
    }
    
    public override bool IsCompleted => completed;
}
