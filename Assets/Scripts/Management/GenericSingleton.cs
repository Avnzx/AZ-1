using Godot;
using System;

public abstract partial class GenericSingleton<T> : Node where T: Node, new() {

	private static readonly Lazy<T> lazyInstance = new Lazy<T>(CreateObject);
	public static T instance => lazyInstance.Value;

	private static T CreateObject() {
		Node thisobj = instance;
		thisobj.Name = typeof(T).FullName;
		thisobj.GetTree().Root.AddChild(instance);
        return instance;
	}
}