using System;

namespace ppilib2.Graphics._Attr;

/// <summary>
/// All properties which are marked with Dependent depend on the value of another property.
/// For example: the world position of a Drawable depends on its parent's world Position + their Origin.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Dependent : Attribute
{
    
}