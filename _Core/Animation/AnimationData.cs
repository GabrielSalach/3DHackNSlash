using Godot;
using System;

[GlobalClass]
public partial class AnimationData : Resource
{
    [Export] private Animation animation;
    [Export] private int anticipationEndFrame;
    [Export] private int hitEndFrame;

}
