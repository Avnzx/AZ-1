using Godot;
using System;

public partial class ServerManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ProcessCmdline(OS.GetCmdlineArgs());
		// try to load default config file or allow custom path to a serverconfig
		GD.Print("Welcome to Forward Frontier Server Edition \n------------------------------------------");

		serverConfig = new ServerConfig();

		var enet = new ENetMultiplayerPeer();
		enet.CreateServer(9898);
		this.Multiplayer.MultiplayerPeer = enet;

		this.AddChild(new CommManager());

		this.Multiplayer.PeerConnected += (long x) => {OnPlayerConnect(x);};
		// this.Multiplayer.PeerConnected += (long x) => {OnPlayerConnect(x);};

		
	}


	private void ProcessCmdline(string[] args) {
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

		//  PROCESSING THE OPTIONS
		
		foreach (var arg in argdict) {
			
		}

	}


	private void OnPlayerConnect(long id) {
		GD.Print("Client: ", id, " connected!");
		(string, Godot.Vector3) test = serverConfig!.SpawnChunk;
		// worldNode.CallDeferred(Node2D.MethodName.Fi)
		// worldNode.GetNode()
		GetNode<CommManager>("CommManager").RpcId(id,"GetPlayerID");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	{
	}


	Node? worldNode;
	ServerConfig? serverConfig;

}
