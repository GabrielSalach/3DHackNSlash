using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class CameraController3D : Camera3D
{
    public static CameraController3D Instance { get; private set; }
    
    private readonly List<VirtualCamera> virtualCameras = new List<VirtualCamera>();
    private VirtualCamera currentCamera;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PushWarning("More than one CameraController3D exists");
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        currentCamera = GetHighestPriorityCamera();
        
        if (currentCamera == null)
        {
            return;
        }
        
        Position = currentCamera.Position;
        Rotation = currentCamera.Rotation;
    }

    private VirtualCamera GetHighestPriorityCamera()
    {
        if (virtualCameras.Count == 0)
        {
            return null;
        }
        
        VirtualCamera current = virtualCameras[0];
        foreach (VirtualCamera vCam in virtualCameras.Where(vCam => vCam.Priority > current.Priority))
        {
            current = vCam;
        }

        return current;
    }

    public void RegisterVirtualCamera(VirtualCamera vCam)
    {
        virtualCameras.Add(vCam);
    }
}
