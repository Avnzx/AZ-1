using Godot;
using System;

public partial class CreditsMenu : MarginContainer
{
	public void _on_main_menu_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").RemoveNewestScene();
	}
}
