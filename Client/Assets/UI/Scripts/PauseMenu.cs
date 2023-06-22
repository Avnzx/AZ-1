using Godot;
using System;

public partial class PauseMenu : MarginContainer
{
	public void _on_main_menu_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").DeleteNewestScene();
	}

	public void GiveBackControl() {
		this.GetParent().GetParent().SetProcessUnhandledInput(true);
		this.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}


}
