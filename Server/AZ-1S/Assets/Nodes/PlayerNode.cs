using Godot;
using System;
using static PlayerMovementActions;

// Exists so I can assign a UUID to a player and decide on certain things
// when their UUID is retrieved after they connect
public partial class PlayerNode : RigidBody3D {
    public System.Guid playerID;

    // movement reqested
    public float[] movReq = new float[12];

    public PlayerNode(System.Guid _pID) {
        playerID = _pID;
        
        var collider = new Godot.CollisionShape3D();
        var shape = new Godot.BoxShape3D();

        this.GravityScale = 0;

        collider.Shape = shape;
        shape.Size = Vector3.One;

        this.AddChild(collider);

    }

    public PlayerMovementActions movementActions = new PlayerMovementActions();

    public override void _PhysicsProcess(double delta) {

        var txl = new Godot.Vector3(
            movReq[(int) MovementActionsEnum.PlayerMoveRight]-
                movReq[(int) MovementActionsEnum.PlayerMoveLeft],
            movReq[(int) MovementActionsEnum.PlayerMoveUp]-
                movReq[(int) MovementActionsEnum.PlayerMoveDown],
            movReq[(int) MovementActionsEnum.PlayerMoveBackward]-
                movReq[(int) MovementActionsEnum.PlayerMoveForward]
        );

        // rotation about X,Y,Z
        var rot = new Godot.Vector3(
            movReq[(int) MovementActionsEnum.PlayerRotatePitchUp]-
                movReq[(int) MovementActionsEnum.PlayerRotatePitchDown],
             movReq[(int) MovementActionsEnum.PlayerRotateYawLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateYawRight],
             movReq[(int) MovementActionsEnum.PlayerRotateRollLeft]-
                movReq[(int) MovementActionsEnum.PlayerRotateRollRight]
        );
        this.ApplyForce(txl);
        this.ApplyTorque(rot);
    }
}