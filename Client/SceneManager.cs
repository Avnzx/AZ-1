using Godot;
using System;
using System.Collections;

public partial class SceneManager : Node {
	Stack sceneQueue = new Stack();

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
		GD.Print($"adding scene {path}, count is {sceneQueue.Count}");
	}

	public void ReplaceNewestScene(string path) {
		if (sceneQueue.Count > 0) {
			Node scene = (sceneQueue.Pop() as Node)!;
			scene.QueueFree(); // delete old
		}
		AddNewSceneActual(path);
	}


	public void ReplaceNewestScene(Node node) {
		if (sceneQueue.Count > 0) {
			Node scene = (sceneQueue.Pop() as Node)!;
			scene.QueueFree(); // delete old
		}
		AddNewSceneActual(node);
	}

	public void AddNewSceneActual(Node node) {
		if (sceneQueue.Count > 0) {
			Node? oldscene = (sceneQueue.Peek() as Node);
			oldscene?.GetParentOrNull<Node>()?.RemoveChild(oldscene);
		}
		this.GetTree().Root.CallDeferred(Node.MethodName.AddChild, node);
		sceneQueue.Push(node);
	}

	// Removes the newest scene from the queue and makes the one before it active
	public bool DeleteNewestScene() {
		if (sceneQueue.Count < 1)
			return false;

		Node scene = (sceneQueue.Pop() as Node)!;
		scene.QueueFree(); // delete old
		GetTree().Root.AddChild((sceneQueue.Peek() as Node)!); // reactivate
		return true;
	}
}
