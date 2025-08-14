using Godot;
using Godot.Collections;

public partial class OptionsData : Resource
{
    [Signal] public delegate void OptionChangedEventHandler(string key, Variant value);

    public Dictionary<string, Variant> Values { get; protected set; } = [];
    public Array<string> Keys => [.. Values.Keys];

    public virtual void InitializeDefaults()
    {
        Set("master_volume", 1.0f);
        Set("music_volume", 1.0f);
        Set("sfx_volume", 1.0f);
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
        set
        {
            Values[key] = value;
            EmitSignal(SignalName.OptionChanged, key, value);
        }
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
        EmitSignal(SignalName.OptionChanged, key, value);
    }

    public void Update(Dictionary<string, Variant> dict)
    {
        foreach (var key in dict.Keys)
        {
            Values[key.ToString()] = dict[key];
            EmitSignal(SignalName.OptionChanged, key.ToString(), dict[key]);
        }
    }

    public static OptionsData CreateDefault()
    {
        var data = new OptionsData();
        data.InitializeDefaults();
        return data;
    }
}