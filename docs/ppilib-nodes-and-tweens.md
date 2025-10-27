### ppilib: Transformable Nodes and Tweening

This document provides a quick overview of the transform-related classes recently improved for ergonomics and documentation.

- TransformNodeBase: Base node with Local and World transforms and lazy world recomputation with an event when world changes.
- LerpableNodeBase: Adds simple time-based tweens (fixed duration) for Local.Pos, Local.Scale, Local.Rotation and an Opacity field.
- ILerpableNode: Interface for lerp-enabled nodes.
- IContinuousNode + ContinuousNodeBase: Interface and base class for nodes that continuously approach target values for Local transform components using a rate and easing function.
- IContinuousTween<T> + ContinuousTween<T>: Interface and implementation for generic continuous approach helpers used by ContinuousNodeBase and others.
- Frame: Simple rectangular frame that can draw a debug texture scaled to world size.
- ContinuousTextFrame: Text node whose position/scale/rotation and opacity smoothly approach targets.

Usage tips and examples

1) TransformNodeBase basics

- The Local transform is relative to the parent; World is computed from ancestors + Local.
- World is lazily recomputed; it’s guaranteed up-to-date during OnUpdate/OnDraw.
- Subscribe to WorldTransformChanged to react when the node’s world changes.

2) LerpableNodeBase tweens

Default easing and mode
- Easing defaults to Linear and Mode to InOut so you can start tweening immediately.

Common helpers
- LerpLocalPos(Vector2 to, double seconds)
- LerpLocalScale(Vector2 to, double seconds)
- LerpLocalRotation(float to, double seconds)
- LerpOpacity(float to, double seconds)

Arbitrary attribute
- LerpAttribute(get, set, interpolate, to, seconds)
  Example:
  node.LerpAttribute(
    get: () => myValue,
    set: v => myValue = v,
    interpolate: (a, b, t) => a + (b - a) * t,
    to: 42f,
    time: 0.5
  );

3) ContinuousNode and ContinuousTween

- ContinuousTween<T> never "completes"; it approaches the target using a normalized progress with a rate.
- Changing Target re-bases from the current value for a smooth chase effect.
- ContinuousNode exposes Pos, Scale and Rot ContinuousTween controllers.

Example:
var node = new ContinuousNode("follower", parent, LocalTransform.Root, easeF: p => p);
// Approach a new position:
node.Pos.Target = new Vector2(200, 128);
// Scale towards 2x size:
node.Scale.Target = new Vector2(2, 2);
// Rotate to 90 degrees (radians):
node.Rot.Target = MathF.PI / 2f;

4) Frame debug drawing

- Set DrawDebugShape = true and provide a small Texture2D (e.g., a 1x1 white pixel) to visualize the frame’s world rectangle.
- The texture will be scaled to World.Scale.

5) ContinuousTextFrame

- World.Scale.Y defines the text height; width is derived from font metrics.
- Opacity is clamped to [0..1] when drawing, and you can animate it via OpacityTween.Target.

Example:
var text = new ContinuousTextFrame(
  name: "title",
  parent: root,
  wantedTransform: LocalTransform.Root with { Pos = new Vector2(64, 32), Scale = new Vector2(400, 64) },
  easeF: p => p,
  text: "Snares",
  color: Color.Crimson,
  font: myFont,
  opacity: 0f
);
text.Pos.Target = new Vector2(64, 64);
text.OpacityTween.Target = 1f;

Notes

- All tweens update during OnUpdate; if you manipulate values outside the game loop, call EnsureWorldUpToDate() before reading World.
- For custom easing types with fixed-duration tweens, set node.Easing and node.EasingMode on LerpableNodeBase-derived nodes before starting tweens.
