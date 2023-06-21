using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using FF.Management;

public partial class WorldManager : Node3D, ICanInitialize<ulong> {

	private ulong? worldSeed;
	public bool hasBeenInitialised { get; private set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void DoInitialise(ulong seed) {
		this.worldSeed = seed;
		hasBeenInitialised = true;
	}



	// Expects the transform to already be set
	public void AddModelToChunk(Vector3I chunk, ModelType model) {
		Chunk thechunk = GetOrCreateChunk(chunk, null);
		thechunk.AddChild(model);
		thechunk.modelList.Add(model);		
	}



	public Chunk GetOrCreateChunk(Vector3I position, ulong? seed) {
		var attempt = GetChunkIfExists(position);
		if (attempt != null)
			return attempt;
		
		if (seed.HasValue)
			return CreateChunk(seed.Value, position);

		return CreateChunk(this.worldSeed!.Value, position);
	}

	public Chunk? GetChunkIfExists(Vector3I pos) {
		return GetNodeOrNull<Chunk>(Chunk.GetChunkNameFromPos(pos));
	}

	public Chunk CreateChunk(ulong? seed, Vector3I pos) {
		var rng = new RandomNumberGenerator();
		if (seed.HasValue) { rng.Seed = seed.Value;} 
			else { rng.Seed = this.worldSeed!.Value; }
		

		// X,Y,Z for each planet :skull:
		// TODO: Maybe make the number a param
		var planetpos = new int[3*50];

		for (int i = 0; i < planetpos.Length; i++){
			// half needs to be taken away
			// FIXME: Should remove extra range to account for chunk border take 1.5 x maxplanetsz
			planetpos[i] = rng.RandiRange(-FrontierConstants.chunkSize/2,FrontierConstants.chunkSize/2);
		}

		static (int[], BitArray) CheckOrdinatesCollide(int[] ordinateArr) {
			var ordinateCheckFail = new BitArray(ordinateArr.Length);		
			Array.Sort(ordinateArr); //faster than fac(16) comparisons

			// see the differences, can only go to the n-1th element
			// ordinate check false when collision
			for (int i = 0; i < (ordinateArr.Length-1); i++) {
				ordinateCheckFail[i] = 
					(Math.Abs(ordinateArr[i] - ordinateArr[i+1]) < FrontierConstants.maxPlanetSize)
					  || (Math.Abs(ordinateArr[i]) < FrontierConstants.maxPlanetSize*0.6);
			}
			// GD.Print(new string(ordinateCheckFail.Select(x => x ? '1' : '0').ToArray()));
			return (ordinateArr, ordinateCheckFail);
		}


		var posArray = new List<Vector3I>();
		for (int i = 0; i < (planetpos.Length/3); i++) {
			int m = i*3;
			posArray.Add(new Vector3I(
				planetpos[m],
				planetpos[m+1],
				planetpos[m+2]));
		}

		// TODO: Can check for Y,Z collisions to reduce false positives but very rare anyway.
		// They are only inside one another if all X,Y,Z are within a planets distance
		var xOrdinate = new int[planetpos.Length/3];
		// Check X coordinates
		for (int i = 0; i < xOrdinate.Length; i++) {
				xOrdinate[i] = planetpos[3*i];
		}
		var xordColld = CheckOrdinatesCollide(xOrdinate);
		// an X collision occurred
		if (!xordColld.Item2.Cast<bool>().Contains(true))
			goto SkipOtherCollisionChecks;
		// xordColld.Item2


		var yOrdinate = new int[planetpos.Length/3];
		for (int i = 0; i < yOrdinate.Length; i++) {
			yOrdinate[i] = planetpos[1+(3*i)];
		}
		var yordColld = CheckOrdinatesCollide(yOrdinate);
		xordColld.Item2.And(yordColld.Item2); // modifies xordColld bitarray
		if (!xordColld.Item2.Cast<bool>().Contains(true))
			goto SkipOtherCollisionChecks;



		var zOrdinate = new int[planetpos.Length/3];
		for (int i = 0; i < zOrdinate.Length; i++) {
			yOrdinate[i] = planetpos[2+(3*i)];
		}
		var zordColld = CheckOrdinatesCollide(yOrdinate);
		xordColld.Item2.And(zordColld.Item2); // modifies xordColld bitarray
		if (!xordColld.Item2.Cast<bool>().Contains(true))
			goto SkipOtherCollisionChecks;




		// Get all indices of colliding objects
		// var arr = xordColld.Item2.Cast<bool>().Select((b,i) => b == true ? i : -1).Where(i => i != -1).ToArray();
		// for (int i = 0; i < arr.Length; i++){
		// 	int idx = arr[i]; // make the number the index
		// 	idx = xordColld.Item1[idx]; // make the value the index
		// 	idx = Array.FindIndex(xOrdinate, x => (x == idx)); // get the index in the ordinates
		// 	// FIXME: some weird behaviour throws out of range except
		// 	posArray.RemoveAt(idx);
		// }


		SkipOtherCollisionChecks:
		// GD.Print( Array.ConvertAll(xordColld.Item2.Cast<bool>().ToArray(), item => item ? "1" : "0") );


		Chunk nd = new Chunk(pos);

		HashSet<UInt32> planetIDlist = new HashSet<uint>();
		while (planetIDlist.Count < posArray.Count) {
			planetIDlist.Add(rng.Randi());
		}

		var basePlnRes = ResourceLoader.Load<PackedScene>("res://Assets/Nodes/BasePlanet.tscn");
		for (int i = 0; i < posArray.Count; i++) {
			var pd = basePlnRes.Instantiate<PlanetType>();
			// pd.Transform = pd.Transform with { Origin = position };
			pd.DoInitialise((planetIDlist.ElementAt(i), posArray[i]));
			// GD.Print($"{pd.Position} ID: {pd.planetID}");
			nd.planetList.Add(pd);
			nd.AddChild(pd);
		}

		this.AddChild(nd);
		return nd;
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
						
						// This will be kept in the release as it is relatively
						// low frequency
						GD.Print("Boundary crossed \n COORD: ", i, " RATIO: ",   temp / FrontierConstants.forgiveness, " OLD: ", chunk.Name ," NEW: ", string.Join('_',currChunk), " ORD: ", obj.Position[i], " NRD: ", ab[i]+obj.Position[i]);
						
						obj.Position += new Vector3(ab[0],ab[1],ab[2]);

						var possibleNode = this.GetNodeOrNull(string.Join('_',currChunk));

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
