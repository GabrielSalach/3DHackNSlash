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
    [Export] private AttackState heavyAttack;
    [Export] private DashState dashState;

    protected override State GetInitialState() => initState;

    protected override void SetupTransitions()
    {
        AddTransition(initState, lightAttackA, () => Context.actionMap["light_attack"].IsJustPressed);
        AddTransition(lightAttackA, lightAttackB,
            () => lightAttackA.CurrentPhase == AnimationPhase.RECOVERY && Context.actionMap["light_attack"].IsJustPressed);
        AddTransition(lightAttackB, lightAttackA,
            () => lightAttackB.CurrentPhase == AnimationPhase.RECOVERY && Context.actionMap["light_attack"].IsJustPressed);
        
        AddTransition(initState, heavyAttack, () => Context.actionMap["heavy_attack"].IsJustPressed);
        AddTransition(lightAttackA, heavyAttack,
            () => lightAttackA.CurrentPhase == AnimationPhase.RECOVERY && Context.actionMap["heavy_attack"].IsJustPressed);
        AddTransition(lightAttackB, heavyAttack,
            () => lightAttackB.CurrentPhase == AnimationPhase.RECOVERY && Context.actionMap["heavy_attack"].IsJustPressed);
        
        AddTransition(initState, dashState, () => Context.actionMap["dash"].IsJustPressed);
    }

    public override bool IsCompleted()
    {
        return activeState.IsCompleted();
    }

    public override bool IsCancellable => activeState is AttackState { CurrentPhase: AnimationPhase.RECOVERY };

    protected override void OnEnter()
    {
        WeaponReference ??= weapon.Instantiate() as Weapon;
        Context.modelRoot.GetBoneAttachment("RightHand").AddChild(WeaponReference);
        Context.characterBody.Velocity = Vector3.Zero;
    }

    protected override void OnExit()
    {
        Context.modelRoot.GetBoneAttachment("RightHand").RemoveChild(WeaponReference);
    }
}
