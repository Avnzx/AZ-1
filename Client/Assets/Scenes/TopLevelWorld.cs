using Godot;
using System;

public partial class TopLevelWorld : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}


	public void DoRotate(Quaternion rot) {
		var we = GetNode<WorldEnvironment>("WorldEnvironment");
		// For some reason skies have inverse rotation
		// Since the server is sending inverse we need to invert it again, WTF
		we.Environment.SkyRotation = rot.GetEuler();
	}
}
