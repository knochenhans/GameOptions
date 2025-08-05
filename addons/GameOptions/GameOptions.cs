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
    }

    public static void Load()
    {
        if (FileAccess.FileExists(SavePath))
        {
            using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
            var jsonString = file.GetAsText();
            var parsed = Json.ParseString(jsonString);

            Current = new OptionsData((Dictionary)parsed);
        }
        else
        {
            Log("No options file found, using defaults.", "GameOptions", LogTypeEnum.Framework);
            Current = OptionsData.CreateDefault();
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
}
