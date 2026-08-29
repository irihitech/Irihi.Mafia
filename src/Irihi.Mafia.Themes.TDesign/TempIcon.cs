using Irihi.Iconica.TDesign.Icons;

namespace Irihi.Mafia.Themes.TDesign.Icons;

public interface ITempIcon
{
    public object ProvideValue() => this;
}

public class TempChevronRight : ChevronRight, ITempIcon;

public class TempApp : App, ITempIcon;

public class TempUser : User, ITempIcon;

public class TempClose : Close, ITempIcon;

public class TempCheck : Check, ITempIcon;

public class TempHome : Home, ITempIcon;

public class TempChat : Chat, ITempIcon;

public class TempChevronLeft : ChevronLeft, ITempIcon;

public class TempSearch : Search, ITempIcon;

public class TempSetting : Setting, ITempIcon;