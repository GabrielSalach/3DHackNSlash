using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass, Tool]
public partial class CameraController3D : Camera3D
{
    [Export] private VirtualCamera currentCamera;
    [Export] private float blendTime;

    public CameraController3D()
    {
        TopLevel = true;
    }

    private VirtualCamera GetHighestPriorityCamera()
    {
        List<VirtualCamera> vCams = GetTree().GetNodesInGroup("VirtualCameras").Cast<VirtualCamera>().ToList();

        if (vCams.Count == 0)
        {
            return null;
        }

        return vCams.MaxBy(vCam => vCam.Priority);
    }

    public override void _Process(double delta)
    {
        VirtualCamera highestPriorityCamera = GetHighestPriorityCamera();
        if (highestPriorityCamera == null)
        {
            return;
        }

        if (currentCamera == null)
        {
            currentCamera = highestPriorityCamera;
        }

        if (currentCamera == highestPriorityCamera)
        {
            GlobalPosition = currentCamera.GlobalPosition;
            GlobalRotation = currentCamera.GlobalRotation;
            Fov = currentCamera.FOV;
        }
        else
        {
            Tween tween = GetTree().CreateTween();
            tween.SetParallel();
            tween.TweenProperty(this, "global_position", highestPriorityCamera.GlobalPosition, blendTime);
            tween.TweenProperty(this, "global_rotation", highestPriorityCamera.GlobalRotation, blendTime);
            tween.TweenProperty(this, "fov", highestPriorityCamera.FOV, blendTime);
            tween.Finished += () =>
            {
                currentCamera = highestPriorityCamera;
            };
        }
        
    }
}
