using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Types;
using ppilib.Types.Struct;

namespace ppilib.Interfaces;

#nullable enable
public interface INode
{
    string Name { get; }
    NodeId Id { get; }
    
    INode? Parent { get; }
    IReadOnlyList<INode> Children { get; }
    List<INode> GetDescendants () ;
    
    bool UpdateActive { get; set; }
    bool DrawActive { get; set; }

    // Lifecycle
    bool IsDestroyed { get; }
    void Destroy ();

    void AddChild (INode child);
    void RemoveChild(INode child);
    void Reparent(INode newParent, ReparentMode mode = ReparentMode.PreserveLocal);
    
    void Update (GameTime gameTime);
    void Draw (SpriteBatch spriteBatch);
    INode GetChild (string name);
}