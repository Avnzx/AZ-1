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

    WorldManager? worldNode;

    // Store the playernode and CHUNK in a vector3I, the local chunk offset is all g
    public Dictionary<PlayerNode,Vector3I> disconnectedPlayers = 
        new Dictionary<PlayerNode, Vector3I>();
    public Dictionary<long,PlayerNode> connectedPlayers = 
        new Dictionary<long,PlayerNode>();
	FFServerConfig? serverConfig;

    public void StartServer(int port = 9898) {
        var enet = new ENetMultiplayerPeer();
		enet.CreateServer(port);
		this.Multiplayer.MultiplayerPeer = enet;

        this.Multiplayer.PeerDisconnected += (long x) => { HandleDisconnectPeer(x); };
        this.Multiplayer.PeerConnected += (long x) => { HandleConnectPeer(x); };
    }


    public void HandleDisconnectPeer(long id) {
        GD.Print("Client: ", id, " disconnected :(");

        // peer was not fully authenticated anyway
        if (!connectedPlayers.ContainsKey(id))
            return;

        PlayerNode pNode = connectedPlayers[id];
        pNode.ProcessMode = Node.ProcessModeEnum.Disabled;

        // Store in disc players list and remove from play
        Chunk parentChunk = pNode.GetParent<Chunk>();
        parentChunk.playerList.Remove(pNode);
        disconnectedPlayers.Add(pNode, parentChunk.GetChunkPosition());

        pNode.GetParent().RemoveChild(pNode);
        connectedPlayers.Remove(id);
    }

    public void HandleConnectPeer(long id) {
        GD.Print("Client: ", id, " connected !");
        RpcId(id, nameof(this.CmdPlayerID));
	}


    public CommManager(WorldManager _worldNd, FFServerConfig _serverCfg) {
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
    public TopLevelWorld[] worldNodes = new TopLevelWorld[Enum.GetNames(typeof(FFRenderLayers.RenderLayersEnum)).Length];
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

    public void HandleConnectServer() {
        connectionDetails.isConnected = true;
        var offlinenode = this.GetNodeOrNull<GridContainer>("../UILayer/offlineindicator"); 
        if (offlinenode != null)
            offlinenode.Visible = false;

        GD.Print("Server connected!");
    }

    public void HandleDisconnectServer() {
        connectionDetails.isConnected = false;
        GD.Print("Server disconnected 💀");
        // FIXME: Reconnect logic
        
        var offlinenode = this.GetNodeOrNull<GridContainer>("../UILayer/offlineindicator"); 
        if (offlinenode != null)
            offlinenode.Visible = true;
        
        Timer timer = new Timer();
        timer.WaitTime = 2;
        timer.Autostart = true;
        this.AddChild(timer);
        timer.Connect("timeout", new Callable(this, nameof(HandleDisconnectRetryTimer)));
    }

    public void HandleDisconnectRetryTimer() {

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

        System.Guid retrievedGuid = new Guid(guidstr);

		GD.Print("recieved pID ", retrievedGuid, " from peer ", this.Multiplayer.GetRemoteSenderId());

        // Check to see if there is an already connected player with the same ID
        if (connectedPlayers.FirstOrDefault(x => x.Value.playerID == retrievedGuid).Value != null) {
            (this.Multiplayer.MultiplayerPeer as ENetMultiplayerPeer)?.
                DisconnectPeer(this.Multiplayer.GetRemoteSenderId());
        }

        PlayerNode? discCheck = 
            disconnectedPlayers.FirstOrDefault(x => x.Key.playerID == retrievedGuid).Key;

        if (discCheck != null) {
            // player is old
            GD.Print("preexisting player joined");
            Vector3I chunk = disconnectedPlayers[discCheck];
            disconnectedPlayers.Remove(discCheck);
            // we only want to reset inputs when they reconnect, so the server assumes
            // they keep going with their movement requests
            Array.Clear(discCheck.movReq); 
            connectedPlayers.Add(this.Multiplayer.GetRemoteSenderId(), discCheck);

            worldNode!.GetNode(Chunk.GetChunkNameFromPos(chunk)).
                CallDeferred(Node.MethodName.AddChild, discCheck);

            // Reenable the player
            discCheck.ProcessMode = Node.ProcessModeEnum.Inherit;

        } else {
            // If a player is new
            var pNode = new PlayerNode(new Guid(guidstr));
            connectedPlayers.Add(this.Multiplayer.GetRemoteSenderId(),pNode);

            (string, Godot.Vector3) defspawn = serverConfig!.Value.SpawnChunk;
            pNode.Position = defspawn.Item2;

            worldNode!.GetNode(defspawn.Item1).CallDeferred(Node.MethodName.AddChild, pNode);

            // Add to list of players in chunk
            (worldNode!.GetNode(defspawn.Item1) as Chunk)!.playerList.Add(pNode);
        }
        #endif
	} 

    [Rpc(Godot.MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    // movementtype must be PlayerMovementActions.MovementActionsEnum
    public void CmdPlayerInputs(int movementtp, float strength) {
        #if !ISCLIENT
        PlayerNode? tmpPnd;
        if (connectedPlayers.TryGetValue(this.Multiplayer.GetRemoteSenderId(), out tmpPnd))
            tmpPnd.movReq[movementtp] = strength;
        #endif
    }


    /*-------------------------------------------------------------------------
     SUBSINFO: This contains calls that are from the server to the client
    -------------------------------------------------------------------------*/

    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Reliable)]
	public void CmdPlayerID() {
        #if ISCLIENT
		GD.Print("tried to get playerID");
        GD.Print( ConfigManager.GetConfig().playerID.ToString());
        this.RpcId(1, nameof(this.RspPlayerID), ConfigManager.GetConfig().playerID.ToByteArray());
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

    
    [Rpc(Godot.MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = Godot.MultiplayerPeer.TransferModeEnum.Unreliable)]
    // Will not work if it is >1M units away, this is intended behaviour
    public void CmdUpdateArbitraryModelPos(Vector3 pos, Quaternion rot, long modelID, string path) {
        #if ISCLIENT
        worldNodes[(int) FFRenderLayers.RenderLayersEnum.CloseObjectLayer]
            .UpdateModelPos(pos, rot, (uint) modelID, path); 
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