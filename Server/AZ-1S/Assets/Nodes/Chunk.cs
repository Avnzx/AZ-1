using Godot;
using System.Collections.Generic;

public partial class Chunk : Node3D {
    public List<PlanetType> planetList = new List<PlanetType>();
    public List<ModelType> modelList = new List<ModelType>();
    public List<PlayerNode> playerList = new List<PlayerNode>();
    public List<BaseBullet> bulletList = new List<BaseBullet>();

    // Not global coordinates, in chunks
    public Chunk(Vector3I position) {
        this.Name = GetChunkNameFromPos(position);
        this.Transform = 
            this.Transform with {
                Origin = position*FrontierConstants.chunkSize
            };

    }

    public Chunk(string[] pos) {
        this.Name = $"{pos[0]}_{pos[1]}_{pos[2]}";
        this.Transform = 
            this.Transform with {
                Origin = GetChunkPosFromName(this.Name)*FrontierConstants.chunkSize
            };

    }

    public static string GetChunkNameFromPos(Vector3I position) {
        return $"{position.X}_{position.Y}_{position.Z}";
    }

    public Vector3I GetChunkPosition() {
        return GetChunkPosFromName(this.Name);
    }

    public static Vector3I GetChunkPosFromName(string name) {
        string[] strarr = name.ToString().Split('_');
        return new Vector3I(
            int.Parse(strarr[0]),
            int.Parse(strarr[1]),
            int.Parse(strarr[2])
        );
    }

}