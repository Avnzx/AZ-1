using System;
using Godot;
using Newtonsoft.Json;

public static class ConfigManager {

    public static ConfigOptions GetConfig() {
        ConfigOptions cfgopts;

		if (FileAccess.FileExists(userConfigFile)) {
			using var cfgfile = FileAccess.Open(userConfigFile, FileAccess.ModeFlags.Read);

			cfgopts = JsonConvert.DeserializeObject<ConfigOptions>(cfgfile.GetAsText());
		} else {
            var optsstruct = new ConfigOptions();

            optsstruct.playerID = System.Guid.NewGuid();

            string serialised = JsonConvert.SerializeObject(optsstruct);
            using var cfgfile = FileAccess.Open(userConfigFile, FileAccess.ModeFlags.Write);
            cfgfile.StoreString(serialised);
            cfgopts = optsstruct;
		}
        return cfgopts;
    }

    public struct ConfigOptions {
        public System.Guid playerID;
    }


    public const string userConfigFile = "user://cfg.json";


}