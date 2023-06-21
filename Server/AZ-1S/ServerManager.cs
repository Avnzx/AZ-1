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


		// TODO: Allow for using a STUN or TURN connection
		var commMan = new CommManager(worldNode,serverConfig!.Value);
		this.AddChild(commMan);
		commMan.StartServer();
		

		var rng = new RandomNumberGenerator();
		rng.Randomize();
		worldNode.DoInitialise(rng.Randi());
		worldNode.CreateChunk(null,Vector3I.Zero);

		var model = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/space_station.tscn");
		var station = model.Instantiate<ModelType>();
		station.modelPath = "res://Assets/Scenes/highres/space_station/space_station.tscn";
		station.Transform = station.Transform with { Origin = new Vector3(-30,10,-140) };
		worldNode.AddModelToChunk(Vector3I.Zero, station);		
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

	WorldManager? worldNode;
	FFServerConfig? serverConfig;

}
