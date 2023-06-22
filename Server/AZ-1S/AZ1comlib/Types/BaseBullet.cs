using Godot;
using System.Linq;
using FF.Management;

#if !ISCLIENT
public partial class BaseBullet : Area3D, ICanInitialize<(uint,Vector3,Quaternion,string,float)> {
#else
public partial class BaseBullet : Node3D, ICanInitialize<(uint,Vector3,Quaternion,string,float)> {
#endif

    public void DoInitialise((uint,Vector3,Quaternion,string,float) argv) {
        modelID = argv.Item1;
        this.Transform = this.Transform with { Origin = argv.Item2 };
        this.Quaternion = argv.Item3;
        modelPath = argv.Item4;
        bulletSpeed = argv.Item5;
        hasBeenInitialised = true;
    }

    // FIXME: If on the server check if it collides with something and kill em
    public override void _Process(double delta) {
        #if !ISCLIENT
        Vector3 origin = this.Transform.Origin;
        origin -= this.Transform.Basis.Z*bulletSpeed;
        this.Transform = this.Transform with { Origin = origin };
        
        if (aliveTimer > 1) {
            this.GetParent<Chunk>().bulletList.Remove(this);
            this.QueueFree();
        }
        #else 
        if (aliveTimer > 1) {
            var bulletList = this.GetParent<TopLevelWorld>().bulletList;
            bulletList.Remove(bulletList.FirstOrDefault(x => x.Value == this).Key);
            this.QueueFree();
        }        

        #endif

        aliveTimer += (float) delta;
    }
    

    public bool hasBeenInitialised {get; private set;} = false;
    public uint modelID = 0xffffffff;
    public float bulletSpeed = 0f;

    public float aliveTimer = 0f;

    [Export]
    public string modelPath = "";



}