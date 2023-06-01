using System;
using Godot;

public struct FFServerConfig {
        public FFServerConfig() {}

        public Vector3G SpawnChunk { get; set; } = new Vector3G(0,0,0,0f,0f,0f);
        public DataDirEnum DataDirectory = DataDirEnum.kExecutableDir;
        // public static 








        public enum DataDirEnum { kExecutableDir, kGDUserDir, kCustomDir};
} 