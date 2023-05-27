using Godot;
using System;
using System.Collections.Generic;

public partial class CommManager : Node {

    public override void _Ready() {
        this.Name = "CommManager";
    }

    #if !CLIENT
    public Dictionary<long,Node> connectedPlayers = new Dictionary<long, Node>();
    #endif


    /*-------------------------------------------------------------------------
     INFO: This contains calls that are initially from the client to the
     server
    -------------------------------------------------------------------------*/

    // TODO: Can this be implemented with custom attributes and function factories? 
    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void SendPlayerInfo(string aa) {
		GD.Print("recieved pID ", aa);
	}



    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void SendPlayerInputs(string actionName) {
        GD.Print(actionName, " from ", this.Multiplayer.GetRemoteSenderId());
    }    

    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void SendPlayerInputs(string actionName, float strength) {
        GD.Print(actionName, " from ", this.Multiplayer.GetRemoteSenderId());
    }

    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void Send2AxisInput(string actionName, Vector2 input) {
        GD.Print(actionName, " ", input , " from ", this.Multiplayer.GetRemoteSenderId());
    } 


    /*-------------------------------------------------------------------------
     INFO: This contains calls that are initially from the server to the
     client
    -------------------------------------------------------------------------*/

    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void GetPlayerID() {
        #if ISCLIENT
		GD.Print("tried to get pID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString());
        this.RpcId(1, nameof(SendPlayerInfo), ConfigManager.GetConfig().playerID.ToString());
        #endif
	}

}