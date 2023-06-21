using Godot;
using FF.Management;

#if !ISCLIENT
public partial class ModelType : StaticBody3D, ICanInitialize<(uint,Vector3,string)> {
#else
public partial class ModelType : Node3D, ICanInitialize<(uint,Vector3,string)> {
#endif

    public void DoInitialise((uint,Vector3,string) argv) {
        modelID = argv.Item1;
        this.Transform = this.Transform with { Origin = argv.Item2 };
        modelPath = argv.Item3;
        hasBeenInitialised = true;
    }

    public bool hasBeenInitialised {get; private set;} = false;
    public uint modelID = 0xffffffff;

    [Export]
    public string modelPath = "";
}