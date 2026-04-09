using System;
using Godot;

[GlobalClass]
public partial class Weapon : Node3D
{
	[Export] private Area3D hurtBox;
	public Action<CombatEntity> OnEntityHit;

	public override void _Ready()
	{
		hurtBox.BodyEntered += body =>
		{
			CombatEntity entity = NodeHelpers.GetChild<CombatEntity>(body);
			if (entity != null)
			{
				OnEntityHit?.Invoke(entity);
			}
		};
		DeactivateHurtBox();
	}
	
	public void ActivateHurtBox()
	{
		if(!hurtBox.IsInsideTree())
			AddChild(hurtBox);
	}

	public void DeactivateHurtBox()
	{
		if(hurtBox.IsInsideTree())
			RemoveChild(hurtBox);
	}
}
