using Godot;
using System;

public partial class ServerManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		{ // Initial setup NS
			var cmdline = new System.Collections.Generic.List<string>();
			cmdline.AddRange(OS.GetCmdlineArgs());
			cmdline.AddRange(OS.GetCmdlineUserArgs());
			ProcessCmdline(cmdline);
		}


		// try to load default config file or allow custom path to a serverconfig
		GD.Print("Welcome to Forward Frontier Server Edition \n------------------------------------------");

		worldNode = GetNode<WorldManager>("world");

		var enet = new ENetMultiplayerPeer();
		// Allow for using a STUN or TURN connection
		enet.CreateServer(9898);
		this.Multiplayer.MultiplayerPeer = enet;

		this.AddChild(new CommManager(worldNode,serverConfig!.Value));

		this.Multiplayer.PeerConnected += (long id) => {
			GetNode<CommManager>("CommManager").HandleConnectPeer(id);
		};
		
		this.Multiplayer.PeerDisconnected += (long id) => {
			GetNode<CommManager>("CommManager").HandleDisconnectPeer(id);
		};

		var rng = new RandomNumberGenerator();
		rng.Randomize();
		worldNode.CreateChunk(rng.Randi(),Vector3I.Zero);
		
	}


	private void ProcessCmdline(System.Collections.Generic.List<string> args) {
		var argdict = new System.Collections.Generic.Dictionary<string,string>();
		serverConfig = new FFServerConfig();

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
			switch (arg.Key) {
				case "ss": case "d":
					break;

				default:
					GD.Print("Unrecognized argument ", arg.Key);
					break;
				
			}
		}

	}




	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)	{
	}


	WorldManager? worldNode;
	FFServerConfig? serverConfig;

}
