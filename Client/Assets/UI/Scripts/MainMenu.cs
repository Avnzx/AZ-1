using Godot;
using System;

public partial class MainMenu : MarginContainer {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {	
		var config = ConfigManager.GetConfig();

		if (config.lastConnectedHost == default(string))
			GetNode<Button>("%ContinueButton").Disabled = true;
	}

	public void _on_continue_button_pressed() {
		
	}

	public void _on_new_game_button_pressed() {
		//  ("res://Assets/UI/NewGameMenu.tscn");
	}

	public void _on_join_game_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").AddNewSceneActual("res://Assets/UI/ServerBrowser.tscn");
	}

	public void _on_option_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").AddNewSceneActual("res://Assets/UI/OptionsMenu.tscn");
	}

	public void _on_credits_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").AddNewSceneActual("res://Assets/UI/CreditsMenu.tscn");
	}
}
