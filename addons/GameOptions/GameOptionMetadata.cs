using Godot.Collections;

public static partial class GameOptionMetadata
{
    public static readonly Dictionary<string, Array<string>> DropDownOptions = new()
    {
        { "resolution", new Array<string> { "1280x720", "1920x1080", "2560x1440", "3840x2160" } }
    };

    public static readonly Dictionary<string, OptionDisplayType> DisplayTypes = new()
    {
        { "master_volume", OptionDisplayType.Slider },
        { "music_volume", OptionDisplayType.Slider },
        { "sfx_volume", OptionDisplayType.Slider },
        { "fullscreen", OptionDisplayType.CheckBox },
        { "resolution", OptionDisplayType.DropDown }
    };
}
