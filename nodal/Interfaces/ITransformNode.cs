using System;
using ppilib.Types;
using ppilib.Types.Class;
using ppilib.Types.Struct;

namespace ppilib.Interfaces;

#nullable enable
public interface ITransformNode : INode
{
    LocalTransform Local { get; set; }
    Transform World { get; }
    
    event Action<ITransformNode, Transform>? WorldTransformChanged;

    void MarkDirty (DirtyFlags flags = DirtyFlags.All);
    void RecalculateWorld ();
    void MarkDescendantsDirty();
}