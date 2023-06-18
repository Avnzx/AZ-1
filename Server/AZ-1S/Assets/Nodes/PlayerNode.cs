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

    public PlayerNode(System.Guid _pID) {
        playerID = _pID;
        
        var collider = new Godot.CollisionShape3D();
        var shape = new Godot.BoxShape3D();

        this.GravityScale = 0;

        collider.Shape = shape;
        shape.Size = Vector3.One;

        this.AddChild(collider);

    }

    public override void _EnterTree() {
        commManager = GetNode<CommManager>("/root/main/CommManager");
    }


    public override void _Process(double delta) {
        commManager!.RpcId(commManager!.GetRemoteIDFromPlayer(this), nameof(CommManager.CmdUpdatePlayerRot), this.Quaternion.Inverse());
    }

    public override void _PhysicsProcess(double delta) {
        var txlX = this.Basis.X * (movReq[(int) MovementActionsEnum.PlayerMoveRight]-
            movReq[(int) MovementActionsEnum.PlayerMoveLeft]);
        var txlY = this.Basis.Y * (movReq[(int) MovementActionsEnum.PlayerMoveUp]-
            movReq[(int) MovementActionsEnum.PlayerMoveDown]);
        var txlZ = this.Basis.Z * (movReq[(int) MovementActionsEnum.PlayerMoveBackward]-
            movReq[(int) MovementActionsEnum.PlayerMoveForward]);

        var rotX = this.Basis.X * (movReq[(int) MovementActionsEnum.PlayerRotatePitchUp]-
            movReq[(int) MovementActionsEnum.PlayerRotatePitchDown]);
        var rotY = this.Basis.Y * (movReq[(int) MovementActionsEnum.PlayerRotateYawLeft]-
            movReq[(int) MovementActionsEnum.PlayerRotateYawRight]);
        var rotZ = this.Basis.Z * (movReq[(int) MovementActionsEnum.PlayerRotateRollLeft]-
            movReq[(int) MovementActionsEnum.PlayerRotateRollRight]);

        this.ApplyTorque(rotX + rotY + rotZ);
        this.ApplyForce(txlX + txlY + txlZ);
    }
}