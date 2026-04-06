using Godot;

[GlobalClass]
public partial class CombatState : State
{
    [Export] private State initState;
    [Export] private AttackState lightAttackA;
    [Export] private AttackState lightAttackB;
    [Export] private AttackState heavyAttack;
    [Export] private DashState dashState;

    [Export] private PackedScene weapon;
    private Node weaponReference;
    
    protected override State GetInitialState() => initState;

    protected override void SetupTransitions()
    {
        AddTransition(initState, lightAttackA, () => Input.IsActionJustPressed("light_attack"));
        AddTransition(lightAttackA, lightAttackB,
            () => lightAttackA.CurrentPhase == AnimationPhase.RECOVERY && Input.IsActionJustPressed("light_attack"));
        AddTransition(lightAttackB, lightAttackA,
            () => lightAttackB.CurrentPhase == AnimationPhase.RECOVERY && Input.IsActionJustPressed("light_attack"));
        
        AddTransition(initState, heavyAttack, () => Input.IsActionJustPressed("heavy_attack"));
        AddTransition(lightAttackA, heavyAttack,
            () => lightAttackA.CurrentPhase == AnimationPhase.RECOVERY && Input.IsActionJustPressed("heavy_attack"));
        AddTransition(lightAttackB, heavyAttack,
            () => lightAttackB.CurrentPhase == AnimationPhase.RECOVERY && Input.IsActionJustPressed("heavy_attack"));
        
        AddTransition(initState, dashState, () => Input.IsActionJustPressed("dash"));
    }

    public override bool IsCompleted()
    {
        return activeState.IsCompleted();
    }

    public override bool IsCancellable => activeState is AttackState { CurrentPhase: AnimationPhase.RECOVERY };

    protected override void OnEnter()
    {
        Context.characterBody.Velocity = Vector3.Zero;
        weaponReference = weapon.Instantiate();
        Context.modelRoot.rightHand.AddChild(weaponReference);
    }

    protected override void OnExit()
    {
        Context.modelRoot.rightHand.RemoveChild(weaponReference);
        weaponReference.QueueFree();
    }
}
