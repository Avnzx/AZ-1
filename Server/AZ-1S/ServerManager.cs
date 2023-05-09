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

		this.AddChild(new CommManager());

		this.Multiplayer.PeerConnected += (long x) => {OnPlayerConnect(x);};
	}

	private void OnPlayerConnect(long id) {
		GD.Print("Client: ", id, " connected!");
		GetNode<CommManager>("CommManager").RpcId(id,"GetPlayerID");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	{
	}


	Node? worldNode;

}
