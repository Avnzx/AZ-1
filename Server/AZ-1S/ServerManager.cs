using Godot;
using System;

public partial class ServerManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ProcessCmdline(OS.GetCmdlineArgs());
		GD.Print("Welcome to Forward Frontier Server Edition \n------------------------------------------");


		var enet = new ENetMultiplayerPeer();
		enet.CreateServer(9999);
		this.Multiplayer.MultiplayerPeer = enet;

		this.AddChild(new CommManager());

		this.Multiplayer.PeerConnected += (long x) => {OnPlayerConnect(x);};
	}


	private System.Collections.Generic.Dictionary<string,string> ProcessCmdline(string[] args) {
		var argdict = new System.Collections.Generic.Dictionary<string,string>();

		foreach (var arg in args) {
			if (!arg.Contains("--")) {
				GD.Print("Unrecognized argument ", arg);
				continue;
			}

			if (arg.Contains("=")) {
				string[] keyValue = arg.Split("=");
				argdict[keyValue[0].TrimStart("--".ToCharArray())] = keyValue[1];
			} else {
				// Options without an argument will be present in the dictionary,
				// with the value set to an empty string.
				argdict[arg.TrimStart("--".ToCharArray())] = "";
			}
		}
		return argdict;
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
