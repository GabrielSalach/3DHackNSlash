using Godot;
using Godot.Collections;

public partial class ModelRoot : Node3D
{
	private System.Collections.Generic.Dictionary<string, BoneAttachment3D> boneAttachments = new System.Collections.Generic.Dictionary<string, BoneAttachment3D>();
	[Export] private Array<BoneAttachment3D> bones;

	public override void _Ready()
	{
		foreach (BoneAttachment3D bone in bones)
		{
			boneAttachments.Add(bone.Name, bone);
		}
	}

	public BoneAttachment3D GetBoneAttachment(string name)
	{
		return boneAttachments[name];
	}
}
