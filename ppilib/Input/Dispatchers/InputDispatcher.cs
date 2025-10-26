#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using ppilib.Input.Definitions;
using ppilib.Input.Interfaces;
using ppilib.Interfaces;
using Point = Microsoft.Xna.Framework.Point;

namespace ppilib.Input.Dispatchers;

public sealed class InputDispatcher
{
    private readonly MouseController _mouse;
    private readonly INode _root;
    
    public InputDispatcher(MouseController mouse, INode root)
    {
        _mouse = mouse;
        _root = root;
        _mouse.LeftMouseDown    += OnLeftDown;
        _mouse.LeftMouseUp      += OnLeftUp;
    }

    private void OnLeftDown (MouseState state)
    {
        var target = HitTestTopMost(state.Position);
        if (target == null) return;

        var args = new MouseEventArgs(state);
        target.OnLeftDown(args);
        if (!args.Handled)
        {
            // If the target wants capture, set it inside OnLeftDown and mark Handled
        }

    }

    private void OnLeftUp(MouseState state)
    {
        var target = HitTestTopMost(state.Position);
        if (target == null) return;
        var args = new MouseEventArgs(state);
        target.OnLeftUp(args);
    }
    
    private IInputRegion? HitTestTopMost(Point point)
    {
        // Traverse the node tree in reverse draw order
        // Collect all IInputRegion nodes whose HitTest(point) is true, return the topmost
        IInputRegion? best = null;
        foreach (var node in EnumerateInReverseDrawOrder(_root))
        {
            if (node is not IInputRegion region || !region.HitTest(point)) continue;
            best = region; // first match in reverse draw order is topmost
            break;
        }
        return best;
    }
    private static IEnumerable<INode> EnumerateInReverseDrawOrder(INode root)
    {
        // Depth-first post-order, children reversed, so visually top-most comes first
        var stack = new Stack<INode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            var children = node.Children; 
            foreach (var t in children)
                stack.Push(t);

            yield return node;
        }
    }
}