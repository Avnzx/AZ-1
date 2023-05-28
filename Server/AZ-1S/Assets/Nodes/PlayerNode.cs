using Godot;
using System;

// Exists so I can assign a UUID to a player and decide on certain things
// when their UUID is retrieved after they connect
public partial class PlayerNode : Node3D {
    public System.Guid playerID;

    public PlayerNode(System.Guid _pID) {
        playerID = _pID;
    }
}