using Godot;
using System;

public partial class NewGameMenu : MarginContainer {
	public void OnGenerateNewWorld() {
		
	}
	public void _on_main_menu_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").DeleteNewestScene();
	}
}
