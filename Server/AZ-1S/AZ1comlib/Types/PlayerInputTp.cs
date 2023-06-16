using System;
using Godot;

public struct PlayerInputTp {
    // Can divide this by the unit quaternions and then multiply it by a specific ship's Roll, Yaw, and Pitch speeds 
    public Godot.Quaternion commandRotation;
    // 
    public Godot.Vector3 commandTranslation;
    public bool commandFlightAssist;

    public PlayerInputTp(Quaternion _cRot, Vector3 _cTra, bool _cFla) {
        commandRotation = _cRot;
        commandTranslation = _cTra;
        commandFlightAssist = _cFla;
    }

}