namespace Irihi.Mafia.Themes.TDesign.Icons;

public abstract class TDesignIconFilledBase : TDesignIconBase
{
    static TDesignIconFilledBase()
    {
        // Override the default mode for this base type (and all derived filled
        // icons), so the fill slot resolves to a color instead of being hidden
        // in the default Line mode.
        ModeProperty.OverrideDefaultValue<TDesignIconFilledBase>(IconMode.TwoTone);
    }
}