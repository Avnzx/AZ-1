using Godot;
using System;

public partial class WorldManager : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		worldRoot = GetNodeOrNull<Node>("/root/main/world");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var children = worldRoot!.GetChildren();

		foreach (Node chunk in children) {
			foreach (Node3D obj in chunk.GetChildren()) {
				// obj.Transform.Origin.X
				// TODO: use bit shift (exponent) for speed

				// 2^-19 shift and determine if >1

				// FIXME: Should probably also change node OWNER for serialization
				for (int i = 0; i < 3; i++) {
					var temp = obj.Position[i] / (float) FrontierConstants.chunkSize;
					if (Math.Abs(temp) > FrontierConstants.forgiveness) {
						chunk.RemoveChild(obj);
						string[] currChunk = chunk.Name.ToString().Split('_');
						int chunkOffset = (int) (temp + 0.5d*Math.Sign(temp));
						currChunk[i] = (int.Parse(currChunk[i]) + chunkOffset).ToString();
						
						//  Annoying workaround for not being able to edit XYZ fields
						//FIXME: Broken godot version?
						float[] ab = {0,0,0};
						ab[i] = -(chunkOffset * (float) FrontierConstants.chunkSize);
						

						GD.Print("Boundary c/*rossed \n COORD: ", i, " RATIO: ",   temp / FrontierConstants.forgiveness, " OLD: ", chunk.Name ," NEW: ", string.Join('_',currChunk), " ORD: ", obj.Position[i], " NRD: ", ab[i]+obj.Position[i]);
						
						obj.Position += new Vector3(ab[0],ab[1],ab[2]);

						var possibleNode = worldRoot.GetNodeOrNull(string.Join('_',currChunk));

						// FIXME: Testing code
						if (possibleNode is null) {
							Node nd = new Node();
							nd.Name = string.Join('_',currChunk);
							worldRoot.AddChild(nd);
						}

						worldRoot.GetNode(string.Join('_',currChunk)).AddChild(obj);
				}}
			}
		}

	}

	Node? worldRoot;
}
