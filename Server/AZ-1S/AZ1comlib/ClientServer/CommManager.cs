using Godot;
using System;

public partial class CommManager : Node {

    public override void _Ready() {
        this.Name = "CommManager";
    }
    
    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void GetPlayerID() {
        #if ISCLIENT
		GD.Print("tried to get pID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString() );
        #endif
	}
}