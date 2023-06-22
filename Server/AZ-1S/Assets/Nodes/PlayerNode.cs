using Godot;
using System;
using static PlayerMovementActions;

// Exists so I can assign a UUID to a player and decide on certain things
// when their UUID is retrieved after they connect
public partial class PlayerNode : RigidBody3D {
    public System.Guid playerID;

    // movement reqested
    public float[] movReq = new float[Enum.GetNames(typeof(PlayerMovementActions.MovementActionsEnum)).Length];

    private CommManager? commManager;
    public PlayerMovementActions movementActions = new PlayerMovementActions();

    private float shootCooldown = 0.5f;
    private float shootCooldownRemaining = 0;




    public PlayerNode(System.Guid _pID) {
        playerID = _pID;
        
        var collider = new Godot.CollisionShape3D();
        var shape = new Godot.BoxShape3D();

        this.GravityScale = 0;

        collider.Shape = shape;
        shape.Size = new Vector3(3,8,1.2);

        this.Mass = 100;

        this.AddChild(collider);

    }

    public void DoRespawn() {
        this.Quaternion = Quaternion.Identity;
        this.Transform = this.Transform with { Origin = Vector3.Zero };
    }

    public override void _EnterTree() {
        commManager = GetNode<CommManager>("/root/main/CommManager");
    }


    public override void _Process(double delta) {
        if (movReq[(int) MovementActionsEnum.PlayerUseWeapons] > 0.5f) {
            shootCooldownRemaining -= (float) delta;

            if (shootCooldownRemaining <= 0) {
                shootCooldownRemaining = shootCooldown;

                Chunk chunk = GetParent<Chunk>();

                RandomNumberGenerator rng = new RandomNumberGenerator();
                rng.Randomize();

                var bulletres = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/bullet.tscn");
                BaseBullet bullet = bulletres.Instantiate<BaseBullet>();
                Vector3 tfx = this.Transform.Origin + 5*this.Transform.Basis.Z;

                bullet.DoInitialise((rng.Randi(), tfx, this.Quaternion,
                    "res://Assets/Scenes/nores/bullet/bullet.tscn", 3f, this));

                chunk.bulletList.Add(bullet);
                chunk.AddChild(bullet);
            }


        }
        


        commManager!.RpcIdIfConnected(this, nameof(CommManager.CmdUpdatePlayerRot), this.Quaternion.Inverse());

        // send planets in the chunk
        foreach (var planet in GetParent<Chunk>().planetList) {
            int[] deltapos = new int[3];
            var deltavec = (((Vector3I) planet.Position) - ((Vector3I)this.Position));
            deltavec.Deconstruct(out deltapos[0], out deltapos[1], out deltapos[2]);

            if (Array.TrueForAll(deltapos, (x => Math.Abs(x/1000) < 1000000))) {
                commManager.RpcIdIfConnected(this, 
                    nameof(CommManager.CmdUpdatePlanetPos),
                    deltavec/1000, planet.planetID );
            }
        }

        // send !models! in the chunk
        foreach (var model in GetParent<Chunk>().modelList) {
            double[] deltapos = new double[3];
            var deltavec = (model.Position - this.Position);
            deltavec.Deconstruct(out deltapos[0], out deltapos[1], out deltapos[2]);

            if (Array.TrueForAll(deltapos, (x => Math.Abs(x) < 1000000))) {
                commManager.RpcIdIfConnected(this, 
                // Vector3 pos, Quaternion rot, long modelID
                    nameof(CommManager.CmdUpdateArbitraryModelPos),
                    deltavec, model.Quaternion, model.modelID, model.modelPath );
            }
        }


        // send !players! in the chunk
        foreach (var player in GetParent<Chunk>().playerList) {
            if (player == this)
                continue;

            double[] deltapos = new double[3];
            var deltavec = (player.Position - this.Position);
            deltavec.Deconstruct(out deltapos[0], out deltapos[1], out deltapos[2]);

            if (Array.TrueForAll(deltapos, (x => Math.Abs(x) < 1000000))) {
                long id = BitConverter.ToInt64(player.playerID.ToByteArray(), 0);

                commManager.RpcIdIfConnected(this, 
                // Vector3 pos, Quaternion rot, long modelID
                    nameof(CommManager.CmdUpdateArbitraryModelPos),
                    deltavec, player.Quaternion, id, "res://Assets/Scenes/nores/space_ship/space_ship.tscn" );
            }
        }

        // send !players! in the chunk
        foreach (var bullet in GetParent<Chunk>().bulletList) {

            double[] deltapos = new double[3];
            var deltavec = (bullet.Position - this.Position);
            deltavec.Deconstruct(out deltapos[0], out deltapos[1], out deltapos[2]);

            if (Array.TrueForAll(deltapos, (x => Math.Abs(x) < 1000000))) {
                long id = bullet.modelID;

                commManager.RpcIdIfConnected(this, 
                // Vector3 pos, Quaternion rot, long modelID
                    nameof(CommManager.CmdUpdateArbitraryModelPos),
                    deltavec, bullet.Quaternion, id, "res://Assets/Scenes/nores/bullet/bullet.tscn" );
            }
        }
    }

    public override void _PhysicsProcess(double delta) {

        if ( !Convert.ToBoolean(movReq[(int) MovementActionsEnum.PlayerDisableFlightAssist])) {
            var txlX = this.Basis.X * accelConst.X *
                (movReq[(int) MovementActionsEnum.PlayerMoveRight]-
                movReq[(int) MovementActionsEnum.PlayerMoveLeft]);
            var txlY = this.Basis.Y * accelConst.Y *
                (movReq[(int) MovementActionsEnum.PlayerMoveUp]-
                movReq[(int) MovementActionsEnum.PlayerMoveDown]);
            var txlZ = this.Basis.Z * accelConst.Z *
                (movReq[(int) MovementActionsEnum.PlayerMoveBackward]-
                movReq[(int) MovementActionsEnum.PlayerMoveForward]);

            var rotX = this.Basis.X * angularAccelConst.X *
                (movReq[(int) MovementActionsEnum.PlayerRotatePitchUp]-
                movReq[(int) MovementActionsEnum.PlayerRotatePitchDown]);
            var rotY = this.Basis.Y * angularAccelConst.Y *
                (movReq[(int) MovementActionsEnum.PlayerRotateYawLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateYawRight]);
            var rotZ = this.Basis.Z * angularAccelConst.Z *
                (movReq[(int) MovementActionsEnum.PlayerRotateRollLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateRollRight]);

            this.ApplyTorque( delta * (rotX + rotY + rotZ) );
            this.ApplyForce( delta * (txlX + txlY + txlZ) );
        } else {


            var angvel = -this.AngularVelocity/(2*Math.PI);

            // GD.Print($"{angvel}");

            var rotX = this.Basis.X * angularAccelConst.X *
                (movReq[(int) MovementActionsEnum.PlayerRotatePitchUp]-
                movReq[(int) MovementActionsEnum.PlayerRotatePitchDown]);
            var rotY = this.Basis.Y * angularAccelConst.Y *
                (movReq[(int) MovementActionsEnum.PlayerRotateYawLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateYawRight]);
            var rotZ = this.Basis.Z * angularAccelConst.Z *
                (movReq[(int) MovementActionsEnum.PlayerRotateRollLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateRollRight]);


            // float cmdXtxl = (movReq[(int) MovementActionsEnum.PlayerMoveRight]-
            //     movReq[(int) MovementActionsEnum.PlayerMoveLeft]);
            
            // if(!Convert.ToBoolean(cmdXtxl)) // see if there is a command
                // this.Basis.X * -accelConst.X * this.LinearVelocity.X
                // project 



            this.ApplyTorque(angvel);
            // this.ApplyForce(txlX + txlY + txlZ);
        }


    }

    Vector3 angularAccelConst = new Vector3(4000,2500,8000); // pitch, yaw, roll
    Vector3 accelConst = new Vector3(5000,10000,40000); // sideways, vertical, fw / back
}