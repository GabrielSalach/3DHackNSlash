
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class DebugControl : BoxContainer
{
	public static DebugControl instance;
	
	private Dictionary<string, Label> debugValues = new Dictionary<string, Label>();

	public override void _Ready()
	{
		instance = this;
	}
	
	public void SetValue(string key, string value)
	{
		if (!debugValues.TryGetValue(key, out Label label))
		{
			label = new Label();
			debugValues.Add(key, label);
			AddChild(label);
		}

		label.Text = value;
	}
}
