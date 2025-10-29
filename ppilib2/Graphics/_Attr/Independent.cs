using System;

namespace ppilib2.Graphics._Attr;

/// <summary>
/// All properties which are marked with Independent do not depend on the value of another property.
/// It is usually controlled by the dev/user.
/// For example: the local position of a Drawable is configurable and is Independent.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class Independent : Attribute
{
    
}