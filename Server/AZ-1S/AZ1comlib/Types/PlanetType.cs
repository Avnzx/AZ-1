using Godot;
using FF.Management;

#if ISCLIENT
public partial class PlanetType : CsgSphere3D, ICanInitialize {
#else
public partial class PlanetType : StaticBody3D, ICanInitialize {
#endif

    public void DoInitialise(params object[] argv) {
        planetID = (uint) argv[0];
        this.Transform = this.Transform with { Origin = (Godot.Vector3I) argv[1] };
        hasBeenInitialised = true;
    }

    public bool hasBeenInitialised {get; private set;} = false;
    uint planetID;
}