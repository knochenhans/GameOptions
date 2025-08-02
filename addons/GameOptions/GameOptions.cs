using Godot;
using Godot.Collections;

public static class GameOptions
{
    private const string SavePath = "user://options.json";

    public static OptionsData Current { get; private set; } = new OptionsData();

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
            Logger.Log("No options file found, using defaults.", "GameOptions", Logger.LogTypeEnum.Framework);
            Current = OptionsData.CreateDefault();
        }

        Logger.Log("Game options loaded successfully.", "GameOptions", Logger.LogTypeEnum.Framework);
    }

    public static void Save()
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(Current.Values, indent: "\t");
        file.StoreString(jsonString);

        Logger.Log("Game options saved successfully.", "GameOptions", Logger.LogTypeEnum.Framework);
    }

    public static void ResetToDefault()
    {
        Current = OptionsData.CreateDefault();
    }
}
