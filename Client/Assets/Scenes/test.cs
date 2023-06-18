using Godot;
using System;

public partial class test : Node{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		ConfigManager.GetConfig();

		var enet = new ENetMultiplayerPeer();
		enet.CreateClient("localhost",9898);
		this.Multiplayer.MultiplayerPeer = enet;

		worldNode = GetNode<Node3D>("world");
		commManager = new CommManager(worldNode);
		AddChild(commManager);

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			GetTree().AutoAcceptQuit = false;
			this.Multiplayer.MultiplayerPeer.Close();
			GetTree().Quit();
		}
	}




	public void CollectAndSendInput(InputEvent ev, StringName action) {
		PlayerMovementActions.MovementActionsEnum tmp;
		Enum.TryParse<PlayerMovementActions.MovementActionsEnum>(action.ToString(),false,out tmp);

		if (ev.IsAction(action)) // send on both press and release
			commManager!.RpcId(1, nameof(CommManager.CmdPlayerInputs),  (int) tmp, ev.GetActionStrength(action));
	}

	public void SendInput(PlayerMovementActions.MovementActionsEnum action, float strength) {		
		commManager!.RpcId(1, nameof(CommManager.CmdPlayerInputs), (int) action, strength);
	}







	public override void _UnhandledInput(InputEvent ev) {

		if (ev.IsActionPressed(InputActionStr.PlayerResetMouseAccumulator)) {
			relativeMouseAccumulator = Vector2.Zero;     
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateYawLeft, 0f);
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchUp, 0f);
		}

		CollectAndSendInput(ev, InputActionStr.PlayerDisableFlightAssist);
		CollectAndSendInput(ev, InputActionStr.PlayerResetThrottle);

		CollectAndSendInput(ev, InputActionStr.PlayerMoveBackward);
		CollectAndSendInput(ev, InputActionStr.PlayerMoveForward);
		CollectAndSendInput(ev, InputActionStr.PlayerMoveDown);
		CollectAndSendInput(ev, InputActionStr.PlayerMoveUp);
		CollectAndSendInput(ev, InputActionStr.PlayerMoveLeft);
		CollectAndSendInput(ev, InputActionStr.PlayerMoveRight);

		CollectAndSendInput(ev, InputActionStr.PlayerRotatePitchDown);
		CollectAndSendInput(ev, InputActionStr.PlayerRotatePitchUp);
		CollectAndSendInput(ev, InputActionStr.PlayerRotateRollLeft);
		CollectAndSendInput(ev, InputActionStr.PlayerRotateRollRight);
		CollectAndSendInput(ev, InputActionStr.PlayerRotateYawLeft);
		CollectAndSendInput(ev, InputActionStr.PlayerRotateYawRight);

		if (ev is Godot.InputEventMouseMotion) {
			// Uses screen coordinates (-Y is up)
			relativeMouseAccumulator += 
				(ev as Godot.InputEventMouseMotion)!.Relative / this.GetWindow().Size;
			relativeMouseAccumulator = relativeMouseAccumulator.LimitLength();
			GD.Print(relativeMouseAccumulator);


			float temp = -relativeMouseAccumulator.Y;
			if (Mathf.Sign(temp) == 1) 
			{ SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchUp, temp);
			} else { SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchDown, -temp); } 

			temp = relativeMouseAccumulator.X;
			if (Mathf.Sign(temp) == 1) 
			{ SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollRight, temp);
			} else { SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollLeft, -temp); } 
		}
	}

	Node3D? worldNode;
	CommManager? commManager;
	Vector2 relativeMouseAccumulator = new Vector2();

}
