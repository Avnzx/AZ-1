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
			var planet = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/Planet/BasePlanet.tscn");
			var plt = planet.Instantiate<PlanetType>();
			plt.DoInitialise((planetID, position));
			planetList.Add(planetID,plt);
			this.CallDeferred(Node.MethodName.AddChild, plt);
		} else {
			planetref!.Position = position;
		}
	}



	public void UpdateModelPos(Vector3 position, Quaternion rot, uint modelID, string path) {
		Node3D? modelref;

		if (!modelList.TryGetValue(modelID, out modelref)) {
			// by default the planets are at their max size
			var model = ResourceLoader.Load<PackedScene>(path);
			Node3D mod = model.Instantiate<Node3D>();

			modelList.Add(modelID,mod);
			this.CallDeferred(Node.MethodName.AddChild, mod);
		} else {
			modelref!.Position = position;
			modelref!.Quaternion = rot;
		}
	}


	Dictionary<uint,PlanetType> planetList = new Dictionary<uint, PlanetType>();
	Dictionary<uint,Node3D> modelList = new Dictionary<uint, Node3D>();
	// List<PlayerType>
}
