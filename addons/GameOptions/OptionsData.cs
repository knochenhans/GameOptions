using Godot;
using Godot.Collections;

public class OptionsData
{
    public Dictionary<string, Variant> Values { get; protected set; } = [];
    public Array<string> Keys => [.. Values.Keys];

    public virtual void InitializeDefaults()
    {
        Set("master_volume", 1.0f);
        Set("music_volume", 0.8f);
        Set("sfx_volume", 0.8f);
        Set("fullscreen", false);
        Set("resolution", 0); // Index of dropdown
    }

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

    public void Set(string key, Variant value) => Values[key] = value;

    public static OptionsData CreateDefault()
    {
        var data = new OptionsData();
        data.InitializeDefaults();
        return data;
    }
}