using Godot;
using System;

public partial class ServerManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Print("Welcome to Forward Frontier Server Edition \n------------------------------------------");


		var enet = new ENetMultiplayerPeer();
		enet.CreateServer(9999);
		this.Multiplayer.MultiplayerPeer = enet;


		this.Multiplayer.PeerConnected += (x) => {GD.Print(x.GetType());};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	{
	}
}
