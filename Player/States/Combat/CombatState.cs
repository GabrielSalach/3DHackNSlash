using Godot;

[GlobalClass]
public partial class CombatState : State
{
    [Export] private PackedScene weapon;
    public Weapon WeaponReference { get;private set; }

    [ExportCategory("Child States")]
    [Export] private State initState;
    [Export] private AttackState lightAttackA;
    [Export] private AttackState lightAttackB;
    [Export] private DashState dashState;

    private string bufferedInput;
    

    protected override State GetInitialState() => initState;

    protected override void SetupTransitions()
    {
        AddTransition(initState, lightAttackA, () => bufferedInput == "light_attack");
        AddTransition(initState, dashState, () => Context.actionMap["dash"].IsJustPressed);
        
        AddComboChain(lightAttackA, lightAttackB, "light_attack");
        AddComboChain(lightAttackB, lightAttackA, "light_attack");
    }

    public override bool IsCompleted => activeState.IsCompleted;

    public override bool IsCancellable => Context.attackController.CurrentAnimationPhase == AnimationPhase.RECOVERY;

    protected override void OnEnter()
    {
        WeaponReference ??= weapon.Instantiate() as Weapon;
        Context.modelRoot.GetBoneAttachment("RightHand").AddChild(WeaponReference);
        Context.characterBody.Velocity = Vector3.Zero;
    }

    protected override void OnUpdate(float delta)
    {
        if (Context.actionMap["light_attack"].IsJustPressed)
        {
            bufferedInput = "light_attack";
        }
    }

    protected override void OnExit()
    {
        bufferedInput = "";
        Context.modelRoot.GetBoneAttachment("RightHand").RemoveChild(WeaponReference);
    }

    protected override void OnChildrenTransition(State from, State to)
    {
        bufferedInput = "";
    }

    /// <summary>
    /// Wrapper around transition to quickly add an attack in a combo chain
    /// </summary>
    private void AddComboChain(AttackState from, AttackState to, string action)
    {
        AddTransition(from, to, () => IsCancellable && bufferedInput == action);
    }
}
