using Godot;
using Godot.Collections;

public static class GameOptions
{
    private const string SavePath = "user://options.json";

    public static OptionsData Current { get; private set; } = new OptionsData();
    public readonly static Dictionary<string, Array<string>> DropDownOptions = new()
    {
        { "resolution", new Array<string> { "1280x720", "1920x1080", "2560x1440", "3840x2160" } }
    };

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

    public class OptionsData
    {
        public Dictionary<string, Variant> Values { get; private set; }
        public Array<string> Keys => [.. Values.Keys];

        public OptionsData()
        {
            Values = [];
        }

        public OptionsData(Dictionary dict)
        {
            Values = new Dictionary<string, Variant>(dict);
        }

        public Variant this[string key]
        {
            get => Values.TryGetValue(key, out var value) ? value : Variant.CreateFrom<string>(null);
            set => Values[key] = value;
        }

        public T Get<[MustBeVariant] T>(string key, T defaultValue = default)
        {
            return Values.TryGetValue(key, out var value) && value.VariantType != Variant.Type.Nil
                ? value.As<T>()
                : defaultValue;
        }

        public void Set(string key, Variant value)
        {
            Values[key] = value;
        }

        public static OptionsData CreateDefault()
        {
            var data = new OptionsData();
            data.Set("master_volume", 1.0f);
            data.Set("music_volume", 0.8f);
            data.Set("sfx_volume", 0.8f);
            data.Set("fullscreen", false);
            data.Set("resolution", 0);
            return data;
        }
    }
}
