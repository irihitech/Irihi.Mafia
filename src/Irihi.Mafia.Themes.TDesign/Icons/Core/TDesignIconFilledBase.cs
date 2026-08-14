namespace Irihi.Mafia.Themes.TDesign.Icons;

public abstract class TDesignIconFilledBase : TDesignIconBase
{
    static TDesignIconFilledBase()
    {
        ModeProperty.OverrideDefaultValue<TDesignIconFilledBase>(IconMode.FilledDouble);
    }
}