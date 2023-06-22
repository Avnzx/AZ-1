using Godot;
using System;

public partial class NewGameMenu : MarginContainer {
	public void OnGenerateNewWorld() {
		GetNode<SceneManager>("/root/SceneManager").localServerPID = OS.CreateProcess(
			#if GODOT_LINUXBSD
			(OS.GetExecutablePath().GetBaseDir() + "/AZ-1S.sh"),
			#else
			(OS.GetExecutablePath().GetBaseDir() + "/AZ-1S.exe"),
			#endif
			new string[] {}
		);
		
	}
	public void _on_main_menu_button_pressed() {
		GetNode<SceneManager>("/root/SceneManager").DeleteNewestScene();
	}
}
