
using Godot;

[GlobalClass]
public partial class DummyState : State
{
    protected override State GetInitialState() => null;
    protected override void SetupTransitions() { }
}
