using System;
using Godot;
using Godot.Collections;

public static class MenuBuilder
{
    public static void BuildMenu(GridContainer gridContainer, Dictionary<string, Variant> options, Dictionary<string, OptionMetadata> metadataDict, Array<string> optionOrder, Dictionary<string, Array<string>> dropDownOptions, Action<string, Variant> setValue)
    {
        foreach (var key in optionOrder)
        {
            var value = options[key];
            var metadata = metadataDict.TryGetValue(key, out OptionMetadata outMetadata)
                ? outMetadata
                : new OptionMetadata { DisplayName = key, DisplayType = OptionDisplayType.Slider };

            Control control = null;
            OptionDisplayType displayType = metadata.DisplayType;
            // Variant.Type dataType = metadata.DataType;

            switch (displayType)
            {
                case OptionDisplayType.Slider:
                    if (value.VariantType == Variant.Type.Float)
                    {
                        float f = (float)value;
                        var slider = new HSlider
                        {
                            MinValue = metadata.Min,
                            MaxValue = metadata.Max,
                            Step = (metadata.Max - metadata.Min) / 100.0f,
                            Value = f,
                            Name = $"{key}Slider",
                            CustomMinimumSize = new Vector2(100, 0),
                            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                        };
                        slider.ValueChanged += v => setValue(key, (float)v);
                        control = slider;
                    }
                    break;
                case OptionDisplayType.SpinBox:
                    int i = (int)value;
                    var spinBox = new SpinBox
                    {
                        MinValue = metadata.Min,
                        MaxValue = metadata.Max,
                        Value = i,
                        Name = $"{key}SpinBox",
                        CustomMinimumSize = new Vector2(100, 0),
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                        SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                    };
                    spinBox.ValueChanged += v => setValue(key, (int)v);
                    control = spinBox;
                    break;
                case OptionDisplayType.CheckBox:
                    bool b = (bool)value;
                    var checkBox = new CheckBox
                    {
                        ButtonPressed = b,
                        Name = $"{key}CheckBox"
                    };
                    checkBox.Toggled += pressed => setValue(key, pressed);
                    control = checkBox;
                    break;
                case OptionDisplayType.DropDown:
                    int selectedIndex = (int)value;
                    var dropDown = new OptionButton
                    {
                        Name = $"{key}DropDown",
                        CustomMinimumSize = new Vector2(100, 0),
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                        SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                    };

                    for (int o = 0; o < dropDownOptions[key].Count; o++)
                        dropDown.AddItem(dropDownOptions[key][o], o);

                    dropDown.Selected = selectedIndex;
                    dropDown.ItemSelected += index => setValue(key, index);
                    control = dropDown;
                    break;
                default:
                    Logger.LogWarning($"Unsupported display type '{displayType}' for key '{key}'.", "MenuBuilder", Logger.LogTypeEnum.UI);
                    break;
            }

            if (control == null)
            {
                Logger.LogWarning($"Unsupported data type '{value.VariantType}' for key '{key}'.", "MenuBuilder", Logger.LogTypeEnum.UI);
                continue;
            }

            gridContainer.AddChild(new Label
            {
                Text = metadata.DisplayName,
                Name = $"{key}Label"
            });

            gridContainer.AddChild(control);
        }
    }
}
