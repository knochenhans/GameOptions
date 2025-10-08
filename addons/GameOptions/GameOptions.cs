using Godot;
using Godot.Collections;
using static Logger;

public partial class GameOptions : Node
{
    private const string SavePath = "user://options.json";

    public static OptionsData Current { get; private set; } = new OptionsData();

    public static GameOptions Instance { get; private set; }

    public override void _EnterTree()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            LogError("Duplicate GameOptions instance detected, destroying the new one.", "GameOptions", LogTypeEnum.Framework);
            QueueFree();
            return;
        }

        Instance = this;

        if (AudioServer.GetBusIndex("Music") == -1)
            LogError("Audio bus 'Music' not found!", "GameOptions", LogTypeEnum.Audio);
        if (AudioServer.GetBusIndex("SFX") == -1)
            LogError("Audio bus 'SFX' not found!", "GameOptions", LogTypeEnum.Audio);

        Load();
        Current.OptionChanged += OnGameOptionChanged;
        ApplyGameOptions();
    }

    public void ApplyGameOptions()
    {
        OnGameOptionChanged("master_volume", Current.Get("master_volume", 100.0f));
        OnGameOptionChanged("music_volume", Current.Get("music_volume", 100.0f));
        OnGameOptionChanged("sfx_volume", Current.Get("sfx_volume", 100.0f));
        OnGameOptionChanged("music_enabled", Current.Get("music_enabled", true));
        OnGameOptionChanged("sfx_enabled", Current.Get("sfx_enabled", true));
        OnGameOptionChanged("resolution", Current.GetDropDown("resolution", new Vector2I(1920, 1080)));
        OnGameOptionChanged("fullscreen", Current.Get("fullscreen", false));
    }

    public void OnGameOptionChanged(string key, Variant value)
    {
        switch (key)
        {
            case "master_volume":
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Master"), value.As<float>());
                break;
            case "music_volume":
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Music"), value.As<float>());
                break;
            case "sfx_volume":
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("SFX"), value.As<float>());
                break;
            case "music_enabled":
                AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), !value.As<bool>());
                break;
            case "sfx_enabled":
                AudioServer.SetBusMute(AudioServer.GetBusIndex("SFX"), !value.As<bool>());
                break;
            case "resolution":
                DisplayServer.WindowSetSize(Current.GetDropDown("resolution", new Vector2I(1920, 1080)));
                break;
            case "fullscreen":
                DisplayServer.WindowSetMode(value.As<bool>() ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
                break;
        }
    }

    public static void Load()
    {
        Current = OptionsData.CreateDefault();

        if (FileAccess.FileExists(SavePath))
        {
            using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var parsed = Json.ParseString(jsonString);

            Current.Update((Dictionary<string, Variant>)parsed);
        }
        else
        {
            Log("No options file found, using defaults.", "GameOptions", LogTypeEnum.Framework);
        }

        Log("Game options loaded successfully.", "GameOptions", LogTypeEnum.Framework);
    }

    public static void Save()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(Current.Values, indent: "\t");
        file.StoreString(jsonString);

        Log("Game options saved successfully.", "GameOptions", LogTypeEnum.Framework);
    }

    public static void ResetToDefault()
    {
        Current = OptionsData.CreateDefault();
    }

    public override void _ExitTree()
    {
        Current.OptionChanged -= OnGameOptionChanged;
    }
}
