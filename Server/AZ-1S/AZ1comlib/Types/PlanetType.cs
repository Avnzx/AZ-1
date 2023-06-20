using Godot;
using FF.Management;

#if ISCLIENT
public partial class PlanetType : CsgSphere3D, ICanInitialize<(uint,Godot.Vector3I)> {
#else
public partial class PlanetType : StaticBody3D, ICanInitialize<(uint,Godot.Vector3I)> {
#endif

    public void DoInitialise((uint ,Godot.Vector3I) argv) {
        planetID = argv.Item1;
        this.Transform = this.Transform with { Origin = argv.Item2 };
        hasBeenInitialised = true;
    }

    public bool hasBeenInitialised {get; private set;} = false;
    public uint planetID = 0xffffffff;
}