using Godot;

#if ISCLIENT
public class PlanetType : CsgSphere3D {
#else
public class PlanetType : StaticBody3D {
#endif

    int planetID;
}