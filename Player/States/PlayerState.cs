
using Godot;

[GlobalClass]
public partial class PlayerState : State
{
    [Export] SpringArm3D springArm;
    [Export] public float MouseSensitivity = 0.003f;
    [Export] public float MinPitch = -40.0f;
    [Export] public float MaxPitch = 60.0f;
    
    [ExportCategory("Child States")]
    [Export] private GroundedState groundedState;
    [Export] private AirborneState airborneState;
    [Export] private CombatState combatState;
    [Export] private GunState gunState;
    

    private float _yaw ;
    private float _pitch;


    protected override State GetInitialState() => airborneState;
    protected override void SetupTransitions()
    {
        AddTransition(groundedState, airborneState, () => !Context.characterBody.IsOnFloor());
        AddTransition(groundedState, combatState, InputHelpers.DidAttackThisFrame);
        AddTransition(groundedState, gunState, () => Context.actionMap["aim"].IsPressed);
        
        AddTransition(airborneState, groundedState, () => Context.characterBody.IsOnFloor());
        AddTransition(airborneState, combatState, InputHelpers.DidAttackThisFrame);
        
        AddTransition(combatState, groundedState, () => Context.characterBody.IsOnFloor() && (
            combatState.IsCompleted
            || (Context.MovementDirection != Vector3.Zero && combatState.IsCancellable)
        ));
        AddTransition(combatState, airborneState, () => !Context.characterBody.IsOnFloor() && combatState.IsCompleted);
        
        AddTransition(gunState, groundedState, () => !Context.actionMap["aim"].IsPressed);
    }

    protected override void OnEnter()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    protected override void OnUpdate(float delta)
    {
        springArm.Rotation = new Vector3(_pitch, _yaw, 0);
    }

    protected override void OnChildrenTransition(State from, State to)
    {
        if (from == airborneState && to == groundedState)
        {
            groundedState.fromAirborne = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            _yaw -= motion.Relative.X * MouseSensitivity;

            _pitch -= motion.Relative.Y * MouseSensitivity;
            _pitch = Mathf.Clamp(_pitch,
                Mathf.DegToRad(MinPitch),
                Mathf.DegToRad(MaxPitch));
        }

        if (@event is InputEventKey { Keycode: Key.Escape })
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}
