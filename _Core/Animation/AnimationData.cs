using Godot;

[GlobalClass]
public partial class AnimationData : Resource
{
    [Export] public Animation animation;
    [Export] public int anticipationEndFrame;
    [Export] public int hitEndFrame;

}
