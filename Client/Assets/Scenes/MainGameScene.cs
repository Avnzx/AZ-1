using Godot;
using System;
using FF.Management;

public partial class MainGameScene : Node {
	private (string address, int port) _initialiseArgs;
	public (string address, int port) initialiseArgs {
		get { return _initialiseArgs;  }
		set { _initialiseArgs = value; hasBeenInitialised = true; }
	}

	CommManager? commManager;
	Vector2 relativeMouseAccumulator = new Vector2();
	Sprite2D? cursor;
	public bool hasBeenInitialised { get; private set; } = false;






	public override void _Ready(){

		var world = GetNode<TopLevelWorld>("CloseObjectLayer/ViewContainer/SubViewport/world");
		commManager = new CommManager( new System.Collections.Generic.List<TopLevelWorld>{
			GetNode<TopLevelWorld>("CloseObjectLayer/ViewContainer/SubViewport/world"),
			GetNode<TopLevelWorld>("FarawayLayer/ViewContainer/SubViewport/world")
		}.ToArray());
		AddChild(commManager);

		// this.commManager!.ConnectToServer();
		if (!hasBeenInitialised)
			GD.PushError("MainGameScene has not been initialised properly!");
		this.commManager!.ConnectToServer(initialiseArgs.address, initialiseArgs.port);


		Input.MouseMode = Input.MouseModeEnum.Captured;
		cursor = GetNode<Sprite2D>("UILayer/Cursor");
	}





	public void DoInitialise((string address, int port) varargs) {
		this.commManager!.ConnectToServer(initialiseArgs.address, initialiseArgs.port);
		hasBeenInitialised = true;
		GD.Print("await sucessful");
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
			commManager!.RpcIdIfConnected(nameof(CommManager.CmdPlayerInputs), (int) tmp, ev.GetActionStrength(action));
	}

	public void SendInput(PlayerMovementActions.MovementActionsEnum action, float strength) {
		commManager!.RpcIdIfConnected(nameof(CommManager.CmdPlayerInputs), (int) action, strength);
	}







	public override void _UnhandledInput(InputEvent ev) {


		if (ev.IsActionPressed(InputActionStr.PlayerResetMouseAccumulator)) {
			relativeMouseAccumulator = Vector2.Zero;     
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollLeft, 0f);
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollRight, 0f);
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchUp, 0f);
			SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchDown, 0f);
			cursor!.Position = (this.GetWindow().Size/2);
			(cursor!.Material as ShaderMaterial)!.SetShaderParameter("transparency", 0);
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

		CollectAndSendInput(ev, InputActionStr.PlayerUseWeapons);

		if (ev.IsActionPressed("PauseMenu")) {
			Input.MouseMode = Input.MouseModeEnum.Visible;
			this.SetProcessUnhandledInput(false);
			this.GetNode<MarginContainer>("UILayer/PauseMenu").Visible = true;
		}

		if (ev is Godot.InputEventMouseMotion) {
			// Uses screen coordinates (-Y is up)

			var deltaMouse = (ev as Godot.InputEventMouseMotion)!.Relative / this.GetWindow().Size;

			// counter mouse drift
			relativeMouseAccumulator += 
				deltaMouse.LengthSquared() > (0.005*0.005) ? deltaMouse : Vector2.Zero;
			relativeMouseAccumulator = relativeMouseAccumulator.LimitLength();

			// there is a simpler solution for this
			cursor!.Position = (this.GetWindow().Size/2) + relativeMouseAccumulator*200;
			cursor!.Rotation = (float) Math.PI + Mathf.Atan2( 
				((float) -relativeMouseAccumulator.X), 
				((float) relativeMouseAccumulator.Y));
			(cursor!.Material as ShaderMaterial)!
				.SetShaderParameter("transparency", relativeMouseAccumulator.LengthSquared());

				float temp = -relativeMouseAccumulator.Y;
				if (Mathf.Sign(temp) == 1) { 
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchUp, temp);
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchDown, 0f);
				} else { 
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchDown, -temp);
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotatePitchUp, 0f); } 

				temp = relativeMouseAccumulator.X;
				if (Mathf.Sign(temp) == 1) { 
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollRight, temp);
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollLeft, 0f);
				} else { 
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollLeft, -temp);
					SendInput(PlayerMovementActions.MovementActionsEnum.PlayerRotateRollRight, 0f); } 
		}
	}
}
