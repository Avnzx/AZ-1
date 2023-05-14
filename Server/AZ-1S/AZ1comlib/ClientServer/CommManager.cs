using Godot;
using System;

public partial class CommManager : Node {

    public override void _Ready() {
        this.Name = "CommManager";
    }


    /*-------------------------------------------------------------------------
     INFO: This contains calls that are initially from the server to the
     client,the server reqests the data with GetMethod, and the client sends
     a response back with SendMethod
    -------------------------------------------------------------------------*/

    // TODO: Can this be implemented with custom attributes and function factories? 
    
    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void GetPlayerID() {
        #if ISCLIENT
		GD.Print("tried to get pID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString() );
        SendPlayerInfo(ConfigManager.GetConfig().playerID.ToString());
        this.RpcId(1, nameof())
        #endif
	}

    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void SendPlayerInfo(string aa) {
        #if ISCLIENT
		GD.Print("tried to get pID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString() );
        #endif
	}


    // [SeClRpc(Godot.MultiplayerApi.RpcMode.AnyPeer)]
    // public async playerinfotype GetPlayerInfo() {
    //     codetoexeconclient;
    //     return Value;
    // }

    // SeClRpcId()

    /*-------------------------------------------------------------------------
     INFO: This contains calls that are initially from the server to the
     client,the server reqests the data with GetMethod, and the client sends
     a response back with SendMethod
    -------------------------------------------------------------------------*/


}