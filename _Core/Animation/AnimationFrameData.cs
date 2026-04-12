using Godot;

public enum AnimationPhase
{
    ANTICIPATION,
    HIT,
    RECOVERY
}


[GlobalClass]
public partial class AnimationFrameData : Resource
{
    [Export] public AnimationPhase animationPhase;
    [Export] public Aabb hurtbox;

}
