using Godot;
using System;
using FF.Management;

#if ISCLIENT
public partial class PlanetType : MeshInstance3D, ICanInitialize<(uint,Godot.Vector3I)> {
#else
public partial class PlanetType : StaticBody3D, ICanInitialize<(uint,Godot.Vector3I)> {
#endif

    public void DoInitialise((uint ,Godot.Vector3I) argv) {
        planetID = argv.Item1;
        this.Transform = this.Transform with { Origin = argv.Item2 };
        hasBeenInitialised = true;

        
        float mult = 100f / (float) BitConverter.GetBytes(planetID).AsMemory(2,1).ToArray()[0];
        mult = Mathf.Clamp(mult, 0.5f, 10f);
        #if ISCLIENT
        (this.Mesh as SphereMesh)!.Radius = mult;
        (this.Mesh as SphereMesh)!.Height = mult*2;
        #else
        var collisionshape = this.GetNode<CollisionShape3D>("PlanetCollider");
        collisionshape.Scale = collisionshape.Scale*1000*mult;
        #endif
    }

    #if ISCLIENT
    public override void _Process(double delta) {
        // FIXME: change the haze
        ShaderMaterial mat = (this.GetActiveMaterial(0) as ShaderMaterial)!;
        float nexttime = (((float) mat.GetShaderParameter("time")) + (float) delta*100f) % 1000000;
        mat.SetShaderParameter("time", nexttime);
    }
    #endif


    public bool hasBeenInitialised {get; private set;} = false;
    public uint planetID = 0xffffffff;
}