using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class WorldManager : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void LoadWorld() {
		// random seed for the reproducible RNG
		var rand = new Random();

		var seed = new byte[8];
		rand.NextBytes(seed);

		var rng = new RandomNumberGenerator();
		rng.Seed = BitConverter.ToUInt64(seed);
	}

	public void CreateChunk(ulong seed, Vector3I pos) {
		var rng = new RandomNumberGenerator();
		rng.Seed = seed;

		// X,Y,Z for each planet :skull:
		var planetpos = new int[3*16];

		for (int i = 0; i < planetpos.Length; i++){
			rng.Randomize();
			// half needs to be taken away
			// FIXME: Should remove extra range to account for chunk border take 1.5 x maxplanetsz
			planetpos[i] = rng.RandiRange(-FrontierConstants.chunkSize/2,FrontierConstants.chunkSize/2);
		}


		static (int[], bool[]) CheckOrdinatesCollide(int[] ordinateArr) {
			var ordinateCheckFail = new bool[ordinateArr.Length];
			// TODO: Docs			
			Array.Sort(ordinateArr); //faster than fac(16) comparisons

			// see the differences, can only go to the n-1th element
			// ordinate check false when collision
			for (int i = 0; i < (ordinateArr.Length-1); i++) {
				ordinateCheckFail[i] = 
					Math.Abs(ordinateArr[i] - ordinateArr[i+1]) < FrontierConstants.maxPlanetSize 
						? true : false;
			}
			GD.Print(new string(ordinateCheckFail.Select(x => x ? '1' : '0').ToArray()));
			return (ordinateArr, ordinateCheckFail);
		}


		var posArray = new List<Vector3>();
		for (int i = 0; i < (planetpos.Length/3); i++) {
			int m = i*3;
			posArray.Add(new Vector3(
				planetpos[m],
				planetpos[m+1],
				planetpos[m+2]));
		}


		// TODO: Can check for Y,Z collisions to reduce false positives but very rare anyway.
		// They are only inside one another if all X,Y,Z are within a planets distance
		var pOrdinate = new int[planetpos.Length/3];

		// Check X coordinates
		for (int i = 0; i < pOrdinate.Length; i++) {
				pOrdinate[i] = planetpos[3*i];
		}
		var ordColld = CheckOrdinatesCollide(pOrdinate);
		// a collision occurred
		if (ordColld.Item2.Any(x => x)) {
			// Get all indices of colliding objects
			var arr = ordColld.Item2.Select((b,i) => b == true ? i : -1).Where(i => i != -1).ToArray();
			for (int i = 0; i < arr.Length; i++){
				int idx = arr[i]; // make the number the index
				idx = ordColld.Item1[idx]; // make the value the index
				idx = Array.FindIndex(pOrdinate, x => (x == idx)); // get the index in the ordinates
				posArray.RemoveAt(idx);
			}
		}
		

		Chunk nd = new Chunk(pos);

		// FIXME: The Ctor is broken
		// FIXME: The Ctor is broken
		// FIXME: The Ctor is broken

		var basePlnRes = ResourceLoader.Load<PackedScene>("res://Assets/Nodes/BasePlanet.tscn");
		foreach (var position in posArray) {
			var pd = basePlnRes.Instantiate<PlanetType>();
			pd.Transform = pd.Transform with { Origin = position };
			nd.planetList.Add(pd);
			nd.AddChild(pd);
		}

		


		this.AddChild(nd);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		var children = this.GetChildren();

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
						double[] ab = {0,0,0};
						ab[i] = -(chunkOffset * (float) FrontierConstants.chunkSize);
						

						GD.Print("Boundary crossed \n COORD: ", i, " RATIO: ",   temp / FrontierConstants.forgiveness, " OLD: ", chunk.Name ," NEW: ", string.Join('_',currChunk), " ORD: ", obj.Position[i], " NRD: ", ab[i]+obj.Position[i]);
						
						obj.Position += new Vector3(ab[0],ab[1],ab[2]);

						var possibleNode = this.GetNodeOrNull(string.Join('_',currChunk));

						// FIXME: Testing code
						if (possibleNode is null) {
							Chunk nd = new Chunk(currChunk);
							this.AddChild(nd);
						}

						this.GetNode(string.Join('_',currChunk)).AddChild(obj);
				}}
			}
		}

	}
}
