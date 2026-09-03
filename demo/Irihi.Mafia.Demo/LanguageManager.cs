using Avalonia;
using Irihi.Lingua;

namespace Irihi.Mafia.Demo;

[LinguaManager("./Resources/Strings.resx")]
public partial class LanguageManager;

public sealed class Translate
{
    public LinguaKey? Key { get; set; }

    public Translate()
    {
    }

    public Translate(LinguaKey key) => Key = key;

    public object ProvideValue()
    {
        var observable = Key?.Manager.GetObservable(Key.Key);
        if (observable is null) return AvaloniaProperty.UnsetValue;
        return observable.ToBinding();
    }
}