using ppilib.Input;
using ppilib.Interfaces;

namespace Snares.Game.Screens;

public enum BeatmapSelectorContext
{
    
}

public class BeatmapSelectorScreen(Game1 game, INode parent, MouseController mouse, KeyboardController keyboard)
    : Screen<BeatmapSelectorContext>(game, parent, mouse, keyboard)
{
    
}