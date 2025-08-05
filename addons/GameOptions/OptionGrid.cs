using System.Linq;
using Godot;
using Godot.Collections;

public partial class OptionGrid : VBoxContainer
{
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

    UISoundPlayer UISoundPlayer;

    GridContainer GridContainer;

    public void Init(UISoundPlayer UISoundPlayer)
    {
        this.UISoundPlayer = UISoundPlayer;

        GridContainer = new GridContainer
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

        MenuBuilder.BuildMenu(GridContainer, GameOptions.Current.Values, OptionsMetadata, OptionOrder, GameOptionMetadata.DropDownOptions, SetOptionValue, OnMousePressedItem, OnMouseEnteredItem);

        AddChild(GridContainer);
    }

    public void Clear()
    {
        foreach (Node child in GetChildren())
        {
            if (child is GridContainer)
            {
                RemoveChild(child);
                child.QueueFree();
            }
        }
        GameOptions.Save();
    }

    private void SetOptionValue(string key, Variant value)
    {
        GameOptions.Current[key] = value;
        Logger.Log($"Option '{key}' set to {value}.", "OptionsGrid", Logger.LogTypeEnum.UI);
    }

    public override void _ExitTree() => Clear();

    public void OnMousePressedItem(string key) => UISoundPlayer.PlaySound("click1");
    public void OnMouseEnteredItem(string key) => UISoundPlayer.PlaySound("hover");

    public void DisableInput()
    {
        foreach (Node child in GridContainer.GetChildren())
            if (child is Control control)
                control.SetBlockSignals(true);
    }
}
