using System;
using Godot;

public struct PlayerInputTp {
    // Can divide this by the unit quaternions and then multiply it by a specific ship's Roll, Yaw, and Pitch speeds 
    public Godot.Quaternion commandRotation;
    // 
    public Godot.Vector3 commandTranslation;
    public bool flightAssist;

}