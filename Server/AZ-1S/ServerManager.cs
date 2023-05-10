using Godot;
using System;

public partial class ServerManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Print("Welcome to Forward Frontier Server Edition \n------------------------------------------");

		GD.Print(OS.GetCmdlineArgs());

		GD.Print(OS.ReadStringFromStdIn()); //WARNING: BLOCKING!

		// foreach (var arg in OS.GetCmdlineArgs()) {
		// 	if (arg.Find("=") > -1) {
		// 		string[] keyValue = arg.Split("=");
		// 		args[keyValue[0].LStrip("--")] = keyValue[1];
		// 	} else {
		// 		// Options without an argument will be present in the dictionary,
		// 		// with the value set to an empty string.
		// 		args[keyValue[0].LStrip("--")] = "";
		// 	}
		// }



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
	Godot.Collections.Dictionary args = new Godot.Collections.Dictionary();

}
