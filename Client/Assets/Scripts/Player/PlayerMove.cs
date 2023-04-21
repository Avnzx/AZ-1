using Godot;
using System;

public partial class PlayerMove : CharacterBody3D {
	public override void _Ready() {
		AddChild(new Vector3g());
	}

	public override void _Process(double delta) {
	}

	public override void _UnhandledInput(InputEvent ev) {

	}


	private Node3D? m_universeroot;
	private bool m_grounded = true;
}
