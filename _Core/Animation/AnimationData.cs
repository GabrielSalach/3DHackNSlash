using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class AnimationData : Resource
{
    [Export] public int anticipationEndFrame;
    [Export] public int hitEndFrame;

    public Animation _animation;
    
    private Array<AnimationFrameData> _frameData = [];

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = new Array<Dictionary>();
        
        
        properties.Add(new Dictionary()
        {
            { "name", "animation" },
            { "type", (int)Variant.Type.Object },
            { "hint",  (int)PropertyHint.ResourceType},
            { "hint_string", "Animation"},
            { "usage", (int)PropertyUsageFlags.Default}
        });
        properties.Add(new Dictionary()
        {
            { "name", "frameData" },
            { "type", (int)Variant.Type.Array },
            { "hint", (int)PropertyHint.ArrayType},
            { "hint_string", "AnimationFrameData"},
            { "usage",(int)PropertyUsageFlags.Default}
        });
        
        return properties;
    }

    public override Variant _Get(StringName property)
    {
        if (property == "animation")
        {
            return _animation;
        }

        if (property == "frameData")
        {
            return _frameData;
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "animation")
        {
            _animation = (Animation)value;
            if (_animation != null)
            {
                int _totalFrames = Mathf.RoundToInt(_animation.Length / _animation.Step) + 1;
                _frameData.Resize(_totalFrames);
            }
            NotifyPropertyListChanged();
            return true;
        }

        if (property == "frameData")
        {
            if (value.As<Array<AnimationFrameData>>().Count == _frameData.Count)
            {
                _frameData = (Array<AnimationFrameData>)value;
                NotifyPropertyListChanged();
                return true;
            }
        }

        return false;
    }
}
