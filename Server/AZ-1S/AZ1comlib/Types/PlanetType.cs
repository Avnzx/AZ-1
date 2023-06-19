using Godot;
using FF.Management;

#if ISCLIENT
public partial class PlanetType : CsgSphere3D, ICanInitialize {
#else
public partial class PlanetType : StaticBody3D, ICanInitialize {
#endif

    public void DoInitialise(params object[] argv) {
        planetID = (int) argv[0];
        hasBeenInitialised = true;
    }

    public bool hasBeenInitialised {get; private set;}
    int planetID;
}