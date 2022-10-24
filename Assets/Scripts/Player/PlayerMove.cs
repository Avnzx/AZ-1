using Godot;
using System;

public partial class PlayerMove : CharacterBody3D {

    public override void _Ready() {
        m_camera = GetNode<Camera3D>("Camera3D");
    }

    public override void _Process(double delta) {
        
    }


    private Camera3D? m_camera;
    private bool m_grounded = true;

}