using Godot;
using System;

public partial class test : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		ConfigManager.GetConfig();

		var enet = new ENetMultiplayerPeer();
		enet.CreateClient("localhost",9999);
		this.Multiplayer.MultiplayerPeer = enet;

		worldNode = GetNode<Node>("world");

		AddChild(new CommManager());
	}

    public override void _Process(double delta) {
        // if (Godot.MultiplayerPeer.ConnectionStatus.Connected == Multiplayer.MultiplayerPeer.GetConnectionStatus()){
		// 	worldNode!.RpcId(1,"UpdatePlayerInput");
		// }
    }

	Node? worldNode;

}
