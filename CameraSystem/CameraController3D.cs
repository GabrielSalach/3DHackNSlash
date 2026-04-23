using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass, Tool]
public partial class CameraController3D : Camera3D
{
    [Export] private VirtualCamera currentCamera;

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
        
        currentCamera = highestPriorityCamera;
        
        GlobalPosition = currentCamera.GlobalPosition;
        GlobalRotation = currentCamera.GlobalRotation;
    }
}
