using Godot;
using System;
using System.Collections.Generic;

public partial class TopLevelWorld : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}


	public void DoRotate(Quaternion rot) {
		var we = GetNodeOrNull<Camera3D>("../Camera3D")?.Environment;

		if (we is not null)
			we.SkyRotation = rot.GetEuler();
		this.Quaternion = rot;
	}

	public void UpdatePlanetPos(Vector3I position, uint planetID) {
		PlanetType? planetref;

		if (!planetList.TryGetValue(planetID, out planetref)) {
			// by default the planets are at their max size
			var planet = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/BasePlanet.tscn");
			var plt = planet.Instantiate<PlanetType>();
			plt.DoInitialise((planetID, position));
			planetList.Add(planetID,plt);
			this.CallDeferred(Node.MethodName.AddChild, plt);
		} else {
			planetref!.Position = position;
		}
	}

	Dictionary<uint,PlanetType> planetList = new Dictionary<uint, PlanetType>();
	// List<PlayerType>
}
