using Godot;
using System;

public partial class test : Node{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		ConfigManager.GetConfig();

		var world = GetNode<TopLevelWorld>("CloseObjectLayer/ViewContainer/SubViewport/world");
		commManager = new CommManager( new System.Collections.Generic.List<TopLevelWorld>{
			GetNode<TopLevelWorld>("CloseObjectLayer/ViewContainer/SubViewport/world"),
			GetNode<TopLevelWorld>("FarawayLayer/ViewContainer/SubViewport/world")
		}.ToArray());
		AddChild(commManager);

		commManager.ConnectToServer();

		Input.MouseMode = Input.MouseModeEnum.Captured;

		cursor = GetNode<Sprite2D>("UILayer/Cursor");
	}


	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			GetTree().AutoAcceptQuit = false;
			this.Multiplayer.MultiplayerPeer.Close();
			GetTree().Quit();
		}
	}

	public override void _Process(double delta) {
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

			var deltaMouse = (ev as Godot.InputEventMouseMotion)!.Relative / this.GetWindow().Size;

			relativeMouseAccumulator += 
				deltaMouse.LengthSquared() > (0.005*0.005) ? deltaMouse : Vector2.Zero;
			relativeMouseAccumulator = relativeMouseAccumulator.LimitLength();

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
	CommManager? commManager;
	Vector2 relativeMouseAccumulator = new Vector2();
	Sprite2D? cursor;

}
