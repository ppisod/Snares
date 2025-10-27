using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppilib.Interfaces;

public interface ITextNode
{
    SpriteFont Font { get; set; }
    string Text { get; set; }
    Color Color { get; set; }
}