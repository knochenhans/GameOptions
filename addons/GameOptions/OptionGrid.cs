using System.Linq;
using Godot;
using Godot.Collections;

public partial class OptionGrid : VBoxContainer
{
    [Signal] public delegate void OptionChangedEventHandler(string key, Variant value);

    [Export]
    public Dictionary<string, OptionMetadata> OptionsMetadata = new()
    {
        { "master_volume", new OptionMetadata { DisplayName = "Master Volume", Min = 0.0f, Max = 1.0f, DisplayType = OptionDisplayType.Slider } },
        { "music_volume", new OptionMetadata { DisplayName = "Music Volume", Min = 0.0f, Max = 1.0f, DisplayType = OptionDisplayType.Slider } },
        { "sfx_volume", new OptionMetadata { DisplayName = "Sound Effects Volume", Min = 0.0f, Max = 1.0f, DisplayType = OptionDisplayType.Slider } },
        { "fullscreen", new OptionMetadata { DisplayName = "Fullscreen Mode", DisplayType = OptionDisplayType.CheckBox } },
        { "resolution", new OptionMetadata { DisplayName = "Resolution", DisplayType = OptionDisplayType.DropDown } }
    };

    [Export] public Array<string> OptionOrder = [];

    public override void _Ready()
    {
        var gridContainer = new GridContainer
        {
            Name = "GridContainer",
            Columns = 2,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };

        foreach (var key in OptionsMetadata.Keys)
        {
            // Check if all metadata keys are actually present in the GameOptions
            if (!GameOptions.Current.Values.ContainsKey(key))
            {
                Logger.LogError($"Option '{key}' is not defined in GameOptions.", "Options", Logger.LogTypeEnum.UI);
                return;
            }
        }

        if (OptionOrder.Count == 0)
        {
            OptionOrder = [.. OptionsMetadata.Keys.ToArray()];
            Logger.LogWarning("OptionOrder is empty, using default order based on metadata keys.", "Options", Logger.LogTypeEnum.UI);
        }

        if (OptionOrder.Count != OptionsMetadata.Count)
        {
            Logger.LogWarning("OptionOrder does not match OptionsMetadata count.", "Options", Logger.LogTypeEnum.UI);
            return;
        }

        MenuBuilder.BuildMenu(gridContainer, GameOptions.Current.Values, OptionsMetadata, OptionOrder, GameOptions.DropDownOptions, SetOptionValue);

        AddChild(gridContainer);
    }

    private void SetOptionValue(string key, Variant value)
    {
        GameOptions.Current[key] = value;
        EmitSignal(SignalName.OptionChanged, key, value);
        Logger.Log($"Option '{key}' set to {value}.", "OptionsGrid", Logger.LogTypeEnum.UI);
    }

    public override void _ExitTree()
    {
        GameOptions.Save();
    }
}
