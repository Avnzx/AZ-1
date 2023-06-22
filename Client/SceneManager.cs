using Godot;
using System;
using System.Collections;

public partial class SceneManager : Node {
	Stack sceneQueue = new Stack();
	public int localServerPID;

	public override void _Ready() {
		this.Name = "SceneManager";

		AddNewSceneActual("res://Assets/UI/MainMenu.tscn");
	}

	// Adds a new scene, it becomes the new actual
	public void AddNewSceneActual(string path) {
		PackedScene sceneres = ResourceLoader.Load<PackedScene>(path);
		Node? scene = sceneres.Instantiate();

		if (scene == null)
			return;

		if (sceneQueue.Count > 0) {
			GD.Print(sceneQueue.Count);
			Node? oldscene = (sceneQueue.Peek() as Node);
			oldscene?.GetParentOrNull<Node>()?.RemoveChild(oldscene);
		}
		this.GetTree().Root.CallDeferred(Node.MethodName.AddChild, scene);
		sceneQueue.Push(scene);
	}

	public void AddNewSceneActual(Node node) {
		if (sceneQueue.Count > 0) {
			Node? oldscene = (sceneQueue.Peek() as Node);
			oldscene?.GetParentOrNull<Node>()?.RemoveChild(oldscene);
		}
		this.GetTree().Root.CallDeferred(Node.MethodName.AddChild, node);
		sceneQueue.Push(node);
	}






	public void ReplaceNewestScene(string path) {
		DeleteNewestScene();
		AddNewSceneActual(path);
	}

	public void ReplaceNewestScene(Node node) {
		DeleteNewestScene();
		AddNewSceneActual(node);
	}





	// Removes the newest scene from the queue and MAY make the one before it active
	public bool DeleteNewestScene() {
		GD.Print(sceneQueue.Count);
		if (sceneQueue.Count < 1) {
			return false;
		}

		Node scene = (sceneQueue.Pop() as Node)!;
		scene.QueueFree(); // delete old

		if (sceneQueue.Count == 0)
			return true;

		GetTree().Root.AddChild((sceneQueue.Peek() as Node)!); // reactivate
		return true;
	}
}
