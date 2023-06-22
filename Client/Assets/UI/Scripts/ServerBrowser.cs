using Godot;
using System;

public partial class ServerBrowser : MarginContainer {

	public void _on_hostname_text_changed(string newtext) {
		if (newtext.Length >= 3) {
			this.GetNode<Button>("MarginContainer/VBoxContainer/DirectConnect/connectbutton").Disabled = false;
		} else {
			this.GetNode<Button>("MarginContainer/VBoxContainer/DirectConnect/connectbutton").Disabled = true;
		}
	}

	public void _on_connectbutton_pressed() {
		string addr = GetNode<LineEdit>("MarginContainer/VBoxContainer/DirectConnect/hostname").Text;

		string portText = GetNode<LineEdit>("MarginContainer/VBoxContainer/DirectConnect/port").Text;

		int port = (portText != null && portText != "") ? int.Parse(portText) : 9898;

		var res = ResourceLoader.Load<PackedScene>("res://Assets/Scenes/main.tscn");
		var mainscene = res.Instantiate<MainGameScene>();
		mainscene.initialiseArgs = (addr, port);


		var cfgopts = ConfigManager.GetConfig();
		cfgopts.lastConnectedPort = port; 
		cfgopts.lastConnectedHost = addr;
		ConfigManager.SetConfig(cfgopts);

		GetNode<SceneManager>("/root/SceneManager").ReplaceNewestScene(mainscene);
		// add initialiser method
	}

	public void _on_main_menu_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").DeleteNewestScene();
	}


}
