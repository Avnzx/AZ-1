using Godot;
using FF.Management;

#if !ISCLIENT
public partial class BaseBullet : Area3D, ICanInitialize<(uint,Vector3,string,float)> {
#else
public partial class BaseBullet : Node3D, ICanInitialize<(uint,Vector3,string,float)> {
#endif

    public void DoInitialise((uint,Vector3,string,float) argv) {
        modelID = argv.Item1;
        this.Transform = this.Transform with { Origin = argv.Item2 };
        modelPath = argv.Item3;
        bulletSpeed = argv.Item4;
        hasBeenInitialised = true;
    }

    // FIXME: If on the server check if it collides with something and kill em

    public bool hasBeenInitialised {get; private set;} = false;
    public uint modelID = 0xffffffff;
    public float bulletSpeed = 0f;

    [Export]
    public string modelPath = "";



}