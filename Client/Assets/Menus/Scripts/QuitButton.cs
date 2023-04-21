using Godot;
using System;

public partial class QuitButton : Button
{

	public override void _Pressed() {
		GetTree().Quit();
	}

	public override void _Notification(int what) {
    	if (what == NotificationWMCloseRequest)
        	GetTree().Quit(); // default behavior
	}

}
