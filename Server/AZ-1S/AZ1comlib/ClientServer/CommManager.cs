using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Careful! Every inbuilt engine type might be using floats or doubles!!!

public partial class CommManager : Node {

    /*-------------------------------------------------------------------------
                            SERVER LOCAL FUNCTIONS
    -------------------------------------------------------------------------*/
    #if !ISCLIENT // SERVER ONLY THINGS GO HERE

    Node3D? worldNode;
    // public Dictionary<System.Guid, PlayerNode()
    public Dictionary<long,PlayerNode> connectedPlayers = new Dictionary<long,PlayerNode>();
	FFServerConfig? serverConfig;



    public void HandleDisconnectPeer(long id) {
        GD.Print("Client: ", id, " disconnected :(");
        connectedPlayers[id].QueueFree();
        connectedPlayers.Remove(id);
    }

    public void HandleConnectPeer(long id) {
        GD.Print("Client: ", id, " connected !");
        RpcId(id, nameof(this.CmdPlayerID));
	}


    public CommManager(Node3D _worldNd, FFServerConfig _serverCfg) {
        worldNode = _worldNd;
        serverConfig = _serverCfg;
    }
    
    public long? GetRemoteIDFromPlayer(PlayerNode nd) {
        return connectedPlayers.FirstOrDefault(x => x.Value == nd).Key;
    }

    /// <summary> Only sends an RPC if the peer is connected </summary>
    /// <returns> True if RPC was sent to a connected peer </returns>
    public bool RpcIdIfConnected(long peerId, StringName method, params Variant[] args) {
        if (connectedPlayers.ContainsKey(peerId)) {
            this.RpcId(peerId, method, args);
            return true;
        } else {
            return false;
        }      
    }
    public bool RpcIdIfConnected(PlayerNode playerNd, StringName method, params Variant[] args) {
        long? peerID = GetRemoteIDFromPlayer(playerNd);
        if (peerID.HasValue)
            return this.RpcIdIfConnected(peerID.Value, method, args);
        return false;
    }




    /*-------------------------------------------------------------------------
                            CLIENT LOCAL FUNCTIONS
    -------------------------------------------------------------------------*/
    #else // CLIENT ONLY THINGS GO HERE
    TopLevelWorld[] worldNodes = new TopLevelWorld[Enum.GetNames(typeof(FFRenderLayers.RenderLayersEnum)).Length];
    (string addr, int port, bool isConnected) connectionDetails;


    public CommManager(TopLevelWorld[] _worldNodes) {
        worldNodes = _worldNodes;
    }

    public void ConnectToServer(string addr = "localhost", int port = 9898) {
        connectionDetails = (addr, port, false);

        var enet = new ENetMultiplayerPeer();
		enet.CreateClient(addr,port);
		this.Multiplayer.MultiplayerPeer = enet;

        this.Multiplayer.ServerDisconnected += () => { HandleDisconnectServer(); };
        this.Multiplayer.ConnectedToServer += () => { HandleConnectServer(); };
    }

    public void HandleDisconnectServer() {
        connectionDetails.isConnected = false;
        GD.Print("Server disconnected 💀");
        // FIXME: Reconnect logic
    }

    public void HandleConnectServer() {
        connectionDetails.isConnected = true;
        GD.Print("Server connected :happy:");
    }

    public bool RpcIdIfConnected(StringName method, params Variant[] args) {
        if (connectionDetails.isConnected) {
            this.RpcId(1, method, args);
            return true;
        } else { return false; }
    }

    #endif

    public override void _Ready() {
        this.Name = "CommManager";
    }
    /*-------------------------------------------------------------------------
                            END OF LOCAL FUNCTIONS
    -------------------------------------------------------------------------*/







    /*-------------------------------------------------------------------------
    ---------------------------------------------------------------------------
     SECTIONINFO: This contains calls that use typed interface! USE DIRECTLY!!
    ---------------------------------------------------------------------------
    -------------------------------------------------------------------------*/

    /*-------------------------------------------------------------------------
     SUBSINFO: This contains calls that are from the client to the server
    -------------------------------------------------------------------------*/

    // TODO: Can this be implemented with custom attributes and function factories? 
    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Reliable)]
	public void RspPlayerID(byte[] guidstr) {
        #if !ISCLIENT
		GD.Print("recieved pID ", new Guid(guidstr), " from peer ", this.Multiplayer.GetRemoteSenderId());

        // FIXME: CHeck for duplicates 'n shit
        // TODO: Save players and see the chunk of old pID's
        var pNode = new PlayerNode(new Guid(guidstr));
        connectedPlayers[this.Multiplayer.GetRemoteSenderId()] = pNode;

        (string, Godot.Vector3) defspawn = serverConfig!.Value.SpawnChunk;
        pNode.Position = defspawn.Item2;

        worldNode!.GetNode(defspawn.Item1).CallDeferred(Node.MethodName.AddChild, pNode);
        #endif
	} 

    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    // movementtype must be PlayerMovementActions.MovementActionsEnum
    public void CmdPlayerInputs(int movementtp, float strength) {
        #if !ISCLIENT
        // GD.Print(movementtp, " from ", this.Multiplayer.GetRemoteSenderId(), " strength ", strength);
        connectedPlayers[this.Multiplayer.GetRemoteSenderId()].movReq[movementtp] = strength;
        #endif
    }


    /*-------------------------------------------------------------------------
     SUBSINFO: This contains calls that are from the server to the client
    -------------------------------------------------------------------------*/

    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Reliable)]
	public void CmdPlayerID() {
        #if ISCLIENT
		GD.Print("tried to get pID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString());
        this.RpcId(1, nameof(RspPlayerID), ConfigManager.GetConfig().playerID.ToByteArray());
        #endif
	}

    // This in addition to another RPC that allows for things around the player to be updated are also needed, this allows for only things that have moved to be sent
    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void CmdUpdatePlayerPose(Vector3G pos, Quaternion rot) {
    } 



    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    // position is in Kilometres Vector3I.one is 1000m. some imprecision is OK
    public void CmdUpdatePlanetPos(Vector3I pos, long planetID) {
        #if ISCLIENT
        worldNodes[(int) FFRenderLayers.RenderLayersEnum.FarawayLayer]
            .UpdatePlanetPos(pos, (uint) planetID); 
        #endif
    } 

    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void CmdUpdatePlayerRot(Quaternion rot) {
        #if ISCLIENT
        foreach (var world in worldNodes){ world.DoRotate(rot); }
        #endif
    } 

    












    /*-------------------------------------------------------------------------
    ---------------------------------------------------------------------------
     SECTIONINFO: This contains calls that use RAW BYTES! DO NOT USE DIRECTLY!!
    ---------------------------------------------------------------------------
    -------------------------------------------------------------------------*/

    /*-------------------------------------------------------------------------
     SUBSINFO: This contains calls that are from the client to the server
    -------------------------------------------------------------------------*/



    /*-------------------------------------------------------------------------
     SUBSINFO: This contains calls that are from the server to the client
    -------------------------------------------------------------------------*/




}