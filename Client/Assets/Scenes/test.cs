using Godot;
using System;

public partial class test : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		ConfigManager.GetConfig();

		var enet = new ENetMultiplayerPeer();
		enet.CreateClient("localhost",9898);
		this.Multiplayer.MultiplayerPeer = enet;

		worldNode = GetNode<Node>("world");
		commManager = new CommManager();
		AddChild(commManager);

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Process(double delta) {
    }

	public override void _UnhandledInput(InputEvent ev) {
		int testaccum = 0;

		if (ev.IsActionPressed(new StringName("player_move_forward"))) {
			commManager!.RpcId(1, "SendPlayerInputs", "player_move_forward");
			testaccum++;
		}



		if (ev.IsActionPressed(new StringName("player_reset_mouse_accumulator"))) {
			relativeMouseAccumulator = Vector2.Zero;
			commManager!.RpcId(1, "Cmd2AxisInput", "player_yaw_pitch", relativeMouseAccumulator);
			testaccum++;
		}

		if (ev is Godot.InputEventMouseMotion) {
			relativeMouseAccumulator += 
				(ev as Godot.InputEventMouseMotion)!.Relative / this.GetWindow().Size;
			relativeMouseAccumulator = relativeMouseAccumulator.LimitLength();
			GD.Print(relativeMouseAccumulator);
			testaccum++;
			commManager!.RpcId(1, "Cmd2AxisInput", "player_yaw_pitch", relativeMouseAccumulator);
		}

		GD.Print(testaccum);
	}

	Node? worldNode;
	CommManager? commManager;
	Vector2 relativeMouseAccumulator = new Vector2();

}
