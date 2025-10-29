
# Scene Graph & Rendering System Spec — Checklists & TODOs

This file is a comprehensive checklist / TODO list for implementing a MonoGame-based retained-mode scene graph and rendering system similar in capabilities to osu.Framework / ppy/osu. Use the checkboxes ("- [ ]") as implementation tasks and acceptance criteria. Items are grouped by subsystem and ordered roughly by implementation priority.

Top-level goals
- [ ] Provide a retained scene graph with Drawable nodes and Containers.
- [ ] Provide a robust transform/layout system (local/world matrices, anchoring, origin, relative sizing, autosize).
- [ ] Provide resource loading (TextureStore) with async-safe GPU creation and reference counting.
- [ ] Support RenderTarget-based BufferedContainers with redraw rules (RedrawOnScale, DrawOriginal).
- [ ] Provide DrawNode snapshotting for render-phase safety and future render-thread separation.
- [ ] Provide pooling of frequently created nodes (PoolableDrawable) and a simple particle system.
- [ ] Provide input & hit-testing integrated with transforms and clipping.
- [ ] Provide tests, performance checks and example scenes.

---

## Prioritization & Implementation Phases
Phase 1 — Minimal working system
- [ ] Scene root + per-frame Update/Draw loop integration with MonoGame.Game.
- [ ] Drawable base, Container, Sprite, TextureStore (sync first, async later).
- [ ] Local/world transform composition (cached) with anchors/origin basics.
- [ ] Simple hit test and input dispatch.

Phase 2 — Usability & features
- [ ] RelativeSizeAxes, AutoSizeAxes, Padding/Margins, Layout invalidation.
- [ ] Text rendering (SpriteText) abstraction.
- [ ] Resource async pipeline (decode off-thread, GPU creation on main thread).
- [ ] Pooling and particle example.

Phase 3 — Performance & robustness
- [ ] DrawNode snapshotting and pooling of DrawNodes.
- [ ] BufferedContainer (RenderTarget2D) with RedrawOnScale.
- [ ] Masking/clipping (scissor/stencil), pixel snapping.
- [ ] Extensive unit & integration tests and performance bench.

---

## Scene (SceneRoot / SceneManager)
Responsibilities:
- Manage root Drawables, life-cycle, input dispatch, update & draw order.
- Provide main-thread action queue for background tasks to marshal results.
- Manage adding/removing roots, scheduled removals (Expire), and loading hooks.

Checklist:
- [ ] Class: Scene
  - [ ] Property: GraphicsDevice, SpriteBatch, optional IRenderer adapter.
  - [ ] Methods: AddRoot(Drawable), RemoveRoot(Drawable), Update(GameTime), Draw(GameTime).
  - [ ] Main-thread queue: EnqueueMainThreadAction(Action).
  - [ ] Manage and finalize pending root additions/removals each Update.
  - [ ] Walk scene graph to Update and create DrawNodes (or snapshot).
  - [ ] On Draw: consume DrawNodes/snapshots and issue draw calls.
  - [ ] Provide event hooks: OnSceneStart/Stop/Resize.
  - [ ] Support Debug/Visualisation overlay (draw bounds/axes).
- [ ] Tests:
  - [ ] Adding a root triggers Load->LoadComplete flow and update/draw calls.
  - [ ] EnqueueMainThreadAction invoked from background finishes on next Update.

TODOs:
- [ ] Implement deferred-add/remove logic to avoid mutation during traversal.
- [ ] Provide debug flag to show render stats (draw calls, texture binds).

---

## Drawable (base node)
Responsibilities:
- Represent node with transform, layout, visuals and lifecycle.
- Manage children, invalidation flags, load/unload and pooling hooks.
- Provide API for transforms, anchor/origin, relative sizes.

Checklist:
- [ ] Class: Drawable
  - Core properties:
    - [ ] Position (Vector2)
    - [ ] Size (Vector2)
    - [ ] Scale (Vector2)
    - [ ] Rotation (float) — document units
    - [ ] Origin (Vector2) — normalized & absolute modes
    - [ ] Anchor (enum/struct)
    - [ ] RelativeSizeAxes (flags)
    - [ ] AutoSizeAxes (flags)
    - [ ] Padding, Margin (struct)
    - [ ] Alpha/Colour
    - [ ] Visible, AlwaysPresent, IsPresent behavior
      Good — let’s map each of those properties onto concrete behavior and show how they interact with the Local/World concept you already know. I’ll explain what each property is, what it affects, how it changes the local/world transforms, what invalidation it must trigger, and a few practical gotchas and recommended conventions.

Short mapping up-front
- Your old system: Local = (pos, scale, rot) per-node, World = composed parent * local.
- New (recommended) system: keep that same idea (LocalMatrix + WorldMatrix), but compute LocalMatrix from richer properties:
    - LocalMatrix ← function(Position, Size, Scale, Rotation, Origin, AnchorOffset, Margin).
    - WorldMatrix ← Parent.WorldMatrix * LocalMatrix.
      So Position/Scale/Rotation are still the basic local transform, but Origin, Anchor, Size, RelativeSize and Padding/Margin influence the LocalMatrix and layout phase (Size can be dynamic via RelativeSizeAxes/AutoSizeAxes).

Now the full walk-through.

1) Position (Vector2)
- Meaning: local translation of the node’s pivot/origin within parent coordinate space (i.e., where the node’s origin is placed relative to parent).
- Effect: directly used to compute LocalMatrix (translation step).
- Example: Position = (10, 20) moves node 10px right and 20px down from its anchor/origin placement.
- Invalidations: setting Position → mark Transform dirty (LocalMatrix, WorldMatrix of self and descendants).
- Notes:
    - Position is typically in parent local coordinates (not screen pixels).
    - If you support RelativePositionAxes, Position.X or Y could be interpreted as fraction of parent size (extension).

2) Size (Vector2)
- Meaning: the node’s logical content size (used for layout, origin pivot calculation and when drawing sprites to know intrinsic size).
- Effect: used for:
    - origin pivot calculations (Origin * Size gives pivot point in local coords),
    - when RelativeSizeAxes is enabled, Size becomes a fraction of parent content size,
    - drawing (texture fit) and autosize.
- Invalidations:
    - set Size → mark Layout dirty (if container), mark Transform dirty (because Origin * Size changed), cause children with RelativeSizeAxes to recompute.
- Notes:
    - If AutoSizeAxes includes X or Y, Size is computed from children/content instead of being set directly.
    - If RelativeSizeAxes includes an axis, Size for that axis is treated as a fraction (0..1) and is computed from parent content size each layout pass.

3) Scale (Vector2)
- Meaning: local scale factor applied to the node after the origin pivot is applied.
- Effect: influences LocalMatrix (Scale step).
- Example: Scale=(2,1) doubles width but keeps height same.
- Invalidations: setting Scale → Transform dirty (LocalMatrix and world descendants).
- Notes:
    - Use Vector2(1,1) default.
    - If you want uniform scale convenience, expose float ScaleUniform { set => Scale = new Vector2(v) }.

4) Rotation (float) — units & convention
- Meaning: local rotation applied about the pivot/origin.
- Recommended unit: radians internally (Matrix3x2.CreateRotation takes radians). Expose helpers to set/get in degrees if you prefer (DegreesToRadians conversion).
- Direction convention: document explicitly (e.g., positive = clockwise or counter-clockwise). MonoGame/Matrix uses radians with standard orientation (positive = clockwise on screen Y-down coordinate systems? be explicit for your engine).
- Effect: influences LocalMatrix (Rotation step).
- Invalidations: setting Rotation → Transform dirty.
- Notes:
    - Convert degrees<->radians at property boundaries if you want degrees API.

5) Origin (Vector2) — normalized & absolute modes
- Meaning: pivot point around which rotation and scale are applied.
- Two modes:
    - Normalized origin: values in [0..1], 0 = left/top, 0.5 = center, 1 = right/bottom. OriginNormalised=(0.5,0.5) is common.
    - Absolute origin: values in pixels relative to local coordinate (you might store it as OriginAbsolute = OriginNormalized * Size).
- Use both by storing Origin as normalized + bool OriginIsAbsolute flag OR provide API SetOriginNormalized(Vector2) and SetOriginAbsolute(Vector2).
- Effect on LocalMatrix: first translate by -Origin*Size so pivot moves to (0,0), then apply Scale/Rotate, then translate back into place by Position+AnchorOffset.
- Invalidations: changing Origin or switching mode → Transform dirty.
- Example:
    - Normalized origin (0.5,0.5) for a Size(100,50) → pivot at (50,25) px.

6) Anchor (enum/struct)
- Meaning: where the drawable is anchored within its parent’s content rectangle (e.g., TopLeft, Centre, BottomRight, or fractional Anchor(0.25, 0.6)).
- Implementation:
    - Represent Anchor as either a discrete enum (TopLeft, Centre, etc.) or an Anchor struct with normalized Vector2 (0..1) — I recommend Anchor be a normalized Vector2 to generalize.
    - Compute AnchorOffset = Parent.ContentRect.TopLeft + Anchor.Normalized * (Parent.ContentRect.Size - Self.Size)
        - Equivalently: AnchorOffset places the Drawable’s origin point (after accounting for Origin*Size) inside the parent's content area.
- Effect: AnchorOffset is added into Position at LocalMatrix translation step (effectively sets the baseline location before local Position offset).
- Invalidations: changing Anchor or parent padding/size → Layout dirty (recompute AnchorOffset), ultimately Transform dirty for the descendant.
- Example:
    - Parent size = (200, 100). Child size = (50, 20). Anchor center (0.5,0.5) → anchor offset = (75, 40) placing child centered.

7) RelativeSizeAxes (flags)
- Meaning: indicates which axes interpret Size as a fraction of parent content size instead of absolute pixels.
- Example:
    - RelativeSizeAxes = X: Size.X = SpecifiedSizeX * Parent.ContentWidth.
    - If SpecifiedSizeX is 1.0 → child fills parent width.
- Behavior:
    - Must compute Size during layout pass after parent size is known.
    - If both RelativeSizeAxes and AutoSizeAxes conflict: define precedence (usually AutoSize wins; you’ll want a deterministic rule).
- Invalidations: parent size change → children with RelativeSizeAxes set must be marked Layout dirty and recomputed.

8) AutoSizeAxes (flags)
- Meaning: the node should compute its Size from content (children or intrinsic content such as texture bounds or text measurement) on axes marked AutoSize.
- Behavior:
    - Measure pass: query children or content to produce intrinsic size (e.g., sum widths/heights or max).
    - Arrange pass: set Size to measured size on those axes, then compute LocalMatrix.
- Interaction:
    - AutoSize can propagate up: if a child autosizes, parent autosize may need re-layout (careful to avoid infinite cycles).
- Invalidations:
    - child content change or text change → parent InvalidateLayout(propagateUp: true).
- Recommendation:
    - Separate Measure() and Arrange() phases so you can compute autosize without immediate side effects.

9) Padding / Margin (struct)
- Two distinct concepts:
    - Padding (internal to containers): shrinks the container’s content rect (children are laid out inside parent.ContentRect = Parent.Size minus Padding).
    - Margin (external on each child): shifts child away from anchor or adjacent siblings; affects Position/layout of the child.
- Data: Margin { Left, Top, Right, Bottom } or shorthand { Horizontal, Vertical }.
- Effects:
    - Padding changes Parent.ContentRect → affects children anchor offsets & RelativeSize computations.
    - Margin modifies the child’s final Position/AnchorOffset calculation (e.g., add margin.Left to X).
- Invalidations: changing padding/margin → Layout dirty.

10) Alpha / Colour
- Colour: per-drawable colour tint (e.g., Color4 or Color), usually multiplied with texture color.
- Alpha: transparency as a float 0..1 (often stored in the color’s alpha channel).
- Composition rule:
    - EffectiveAlpha = Parent.EffectiveAlpha * LocalAlpha
    - EffectiveColour = Parent.Colour * LocalColour (you may want to multiply RGB separately and clamp).
- Invalidations: changing Alpha/Colour → Visual dirty / DrawNode invalidation.
- Note:
    - Many systems expose both a Colour and an EffectColour (for glow/blend) — define semantics consistently.

11) Visible, AlwaysPresent, IsPresent behavior
- Visible: whether the node should be drawn (affects Draw call).
- AlwaysPresent: forces the node to remain present in the update loop (or layout) even if Visible=false or Alpha=0. Useful for overlays/input catchers that must still respond to input or maintain layout even if not drawn.
- IsPresent: computed property that decides if the node participates in drawing and/or layout. Example:
    - IsPresent = (Visible || AlwaysPresent) && IsLoaded && !(parent masked away) && within scissor.
    - Another common rule: IsPresent = Visible && (Alpha > epsilon) || AlwaysPresent.
- Expiration: if a node is expired/removed, it's not present even if Visible true.
- Invalidations: toggling Visible or AlwaysPresent should trigger re-evaluation of presence for parent/children.

How LocalMatrix is computed (concrete formula)
Pick a canonical operation order; documenting this prevents future confusion. I recommend:

1. pivot = (OriginIsNormalized ? OriginNormalized * Size : OriginAbsolute)
2. Translate(-pivot)                        // move pivot to origin (0,0)
3. Scale(Scale)                             // scale around pivot
4. Rotate(RotationRadians)                  // rotate around pivot
5. Translate(Position + AnchorOffset + MarginOffset)

So LocalMatrix = T(Position+AnchorOffset+Margin) * R(Rotation) * S(Scale) * T(-pivot)

- Implementation detail:
    - Use Matrix3x2.MakeTranslation / CreateRotation / CreateScale and multiply in the correct order.
    - WorldMatrix = Parent != null ? Parent.WorldMatrix * LocalMatrix : LocalMatrix
        - This means Transforming a local point p to world: worldP = Vector2.Transform(p, WorldMatrix)

Why that order:
- You usually want scale and rotation about the pivot in local space, then place the whole transformed object at Position+AnchorOffset in parent coordinates.
- If you swapped order you’d rotate around parent origin rather than local pivot.

Relating to your old Local/World system
- Old Local = (pos, scale, rot): that’s effectively your LocalMatrix. New system decomposes LocalMatrix into more inputs (Origin, Size, AnchorOffset, Margin).
- Old World = Parent.World * Local: same as new WorldMatrix; we’re just computing Local more precisely to support layout and anchors.
- So equivalence:
    - old.Local.Pos ↔ Position + AnchorOffset (or just Position if your old system ignored anchor)
    - old.Local.Scale ↔ Scale
    - old.Local.Rot ↔ Rotation
    - Computation of world positions is identical once LocalMatrix is assembled.

Invalidation rules — what to mark dirty when a property changes
- TransformDirty (Local/World):
    - Position, Scale, Rotation, Origin, Size (because pivot depends on Size), AnchorOffset change → mark TransformDirty (and cascade to children world dirty).
- LayoutDirty:
    - Size (if autosize or relative), RelativeSizeAxes toggles, Padding, Margin, Parent size changes → mark LayoutDirty and schedule Measure/Arrange pass.
- DrawNodeDirty (visual snapshot needs refresh):
    - Colour, Alpha, Texture, BlendState, ShaderParams → mark DrawNodeDirty
- PresenceDirty:
    - Visible, AlwaysPresent changes → recompute IsPresent
- Implementation tip: set flags on setters and do lazy recompute when LocalMatrix/WorldMatrix/Bounds are requested.

Other practical points and gotchas
- Rounding & pixel-snapping:
    - If you need crisp UI, provide an option SnapToPixelGrid which rounds final world-positioned quads to integer pixel positions during rendering.
- Epsilon equality:
    - Avoid marking dirty when value change is within a small epsilon (for floats). Use Vector2.Equals with tolerance.
- Layout cycles:
    - Guard against autosize cycles (child autosize influences parent which influences child). Use a layout pass ordering and a maximum recursion depth or detect cycles and break them deterministically.
- Threading:
    - Compute transforms and layout on your main thread; draw-snapshot (DrawNode) copies immutable state for renderer usage.
- Caching:
    - Cache LocalMatrix, WorldMatrix, InverseWorldMatrix, and computed Screen-space AABB/Quad. Recompute lazily when flagged dirty.
- Performance:
    - Avoid recomputing world matrices of entire subtree on small local changes by only invalidating descendants when necessary and recomputing on-demand.
- Hit-test:
    - To hit-test a screen point p: transform p into local coords via local = Vector2.Transform(p, InverseWorldMatrix) and check containment against local rectangle [0..Size] (account for Origin if you use absolute pivot).

Recommended data types & API ergonomics
- Use Matrix3x2 for 2D transforms (fast & built-in in System.Numerics or MonoGame’s Matrix types).
- Internally store rotation in radians; expose Degrees <-> Radians helpers if API consumers prefer degrees.
- Provide:
    - SetOriginNormalized(Vector2), SetOriginAbsolute(Vector2)
    - SetAnchorNormalized(Vector2)
    - SetRelativeSizeAxes(Axes flags), SetAutoSizeAxes(Axes flags)
    - InvalidateTransform(), InvalidateLayout(), ForceRecomputeTransform()
    - Vector2 LocalToWorld(Vector2), Vector2 WorldToLocal(Vector2)
  - Lifecycle:
    - [ ] OnAddedToScene(Scene)
    - [ ] Load / LoadAsync / LoadComplete handlers
    - [ ] OnRemovedFromScene()
    - [ ] Expire() and IsExpired
    - [ ] Dispose()/Unload (release references, deref textures)
  - Transform/Layout:
    - [ ] LocalMatrix getter (cached)
    - [ ] WorldMatrix getter (cached, composes with parent)
    - [ ] InvalidateTransform(bool cascade = true)
    - [ ] InvalidateLayout(bool propagateUp = false)
    - [ ] ComputeAnchorOffset(RectangleF parentRect)
    - [ ] ComputeScreenSpaceQuad() and AABB
    - [ ] LocalToWorld/WorldToLocal conversion helpers
  - Draw snapshot:
    - [ ] CreateDrawNode() / ApplyToDrawNode(DrawNode)
    - [ ] Invalidation flags: Transform, Layout, DrawNode
  - Children management:
    - [ ] AddChild/RemoveChild/ClearChildren
    - [ ] InternalChildren vs external API
  - Pooling:
    - [ ] Optional PoolableDrawable hooks: OnAcquireFromPool, OnReturnToPool
- [ ] Tests:
  - [ ] LocalMatrix correctness with origin/anchor/size/rotation/scale combos.
  - [ ] WorldMatrix recomputation and invalidation propagation.
  - [ ] Correct AABB / quad for rotated & scaled nodes.
  - [ ] Expire and removal results in Dispose and resource deref.
- [ ] TODOs:
  - [ ] Provide fluent setters (MoveTo/ScaleTo) that schedule transforms but don't force recompute.
  - [ ] Optimize setters to avoid dirty flag when value unchanged within epsilon.

---

## Container (composite node)
Responsibilities:
- Contain children, forward update/draw, optionally mask/clipping children.
- Provide layout area (content rect) used by anchors/relative sizes.

Checklist:
- [ ] Class: Container : Drawable
  - [ ] Property: ContentRect (parent content rectangle after padding)
  - [ ] Methods: AddInternal(Drawable) vs Add(Drawable) for public/pool use
  - [ ] Override Update to update children in draw order
  - [ ] Override Draw to apply scissor/stencil when Masking enabled
  - [ ] Masking: Masking (bool), CornerRadius for rounded masks
  - [ ] Layout helpers: FillFlowContainer / GridContainer patterns (optional)
- [ ] Tests:
  - [ ] Children layout with relative sizes is recomputed when container size changes.
  - [ ] Masking/clipping excludes children outside content rect.
- [ ] TODOs:
  - [ ] Implement scissor-based clipping for performance; fall back to stencil for rounded corners.
  - [ ] Implement z-order / draw-order controls.

---

## Sprite
Responsibilities:
- Draw a Texture2D with transforms, colour and sprite effects.
- Request texture from TextureStore and manage reference lifecycle.

Checklist:
- [ ] Class: Sprite : Drawable
  - [ ] Properties: TextureKey (string), Texture (Texture2D), FillMode (Fit/Stretch/Fill), SourceRect
  - [ ] Draw logic: use SpriteBatch.Draw with WorldMatrix/transform or compute screen position manually
  - [ ] Load: request texture from TextureStore (async or sync)
  - [ ] Unload: notify TextureStore on disposal
  - [ ] Support common fields: Blending parameters, DrawDepth (z-order)
- [ ] Tests:
  - [ ] Sprite draws when texture loaded, does not when null.
  - [ ] TextureStore refcount increments/decrements on load/unload.
- [ ] TODOs:
  - [ ] Add support for nine-patch / 9-slice rendering.
  - [ ] Add optional pixel-snapping behavior.

---

## SpriteText / Font rendering
Responsibilities:
- Provide text rendering built on SpriteFont or alternative text system.
- Support layout sizing, autosize and alignment.

Checklist:
- [ ] Class: SpriteText : Drawable
  - [ ] Properties: FontKey / SpriteFont, Text, Alignment, Wrap, Trimming
  - [ ] Measure text to compute preferred size for AutoSizeAxes
  - [ ] Draw: render glyphs with SpriteBatch.DrawString or custom glyph atlas
- [ ] Tests:
  - [ ] AutoSize height/width computed correctly for a given font & text.
  - [ ] Anchor/origin & rotation apply identically to sprites.
- [ ] TODOs:
  - [ ] Consider integration with MSDF font rendering for high-quality scaling.

---

## TextureStore / ResourceStore / TextureCache
Responsibilities:
- Central cache and loader for Texture2D.
- Support async decoding off-thread + GPU creation on main thread.
- Reference counting to safely unload unused textures.

Checklist:
- [ ] Class: TextureStore
  - [ ] Methods: Request(string key, Action<Texture2D> callback), RequestSync(string), Unload(string), TryGet(string), Clear()
  - [ ] Internals: cache dictionary key -> (Texture2D, refcount)
  - [ ] Async pipeline:
    - [ ] Step 1: background decode (byte[]/image) — Task.Run
    - [ ] Step 2: enqueue creation on main thread via Scene.EnqueueMainThreadAction to create Texture2D from stream/byte[]
  - [ ] Fallback texture when load fails
  - [ ] Integration with ContentManager, file-system, zip/skin stores and ruleset resource stores
- [ ] Tests:
  - [ ] Async load produces texture accessible next main-thread tick.
  - [ ] Refcounts increment on Request and decrement on Unload/Release and textures disposed only when ref==0.
- [ ] TODOs:
  - [ ] Add caching on-disk decoded bitmaps for long-run speed up.
  - [ ] Add pooling of temporary buffers and create Texture2D via GraphicsDevice.Textures rather than ContentManager where necessary.

---

## DrawNode (Render snapshot)
Responsibilities:
- Immutable snapshot of drawable state consumed by render stage.
- Contains world transform, colour, texture refs, screen-space quad, clip rect.

Checklist:
- [ ] Class/struct: DrawNode (lightweight)
  - [ ] Fields: Matrix3x2 WorldMatrix, float Alpha, Color Colour, Texture2D Texture, Quad ScreenQuad, Rectangle ScissorRect, ShaderParams
  - [ ] Provide pooling to avoid per-frame allocations
  - [ ] Provide method ApplyFrom(Drawable) or ApplyFrom(Drawable, additional params)
- [ ] Integration:
  - [ ] Update phase: Drawable.CreateDrawNode()/ApplyToDrawNode() to fill node from current cached state
  - [ ] Draw phase: Scene iterates DrawNodes and draws them using SpriteBatch/IRenderer
- [ ] Tests:
  - [ ] Snapshot immutability — mutate Drawable after snapshot, draw uses snapshot data unchanged.
- [ ] TODOs:
  - [ ] Provide multi-draw batching by grouping DrawNodes with same texture/shader to reduce drawcalls.

---

## BufferedContainer (Render target caching)
Responsibilities:
- Render children into a RenderTarget2D (cache) and draw the cached texture until invalidated.
- Support flags: DrawOriginal, RedrawOnScale, CachedFrameBuffer.

Checklist:
- [ ] Class: BufferedContainer : Container
  - [ ] Properties: cachedFrameBuffer (bool), RedrawOnScale (bool), DrawOriginal (bool)
  - [ ] Methods: InvalidateBuffer(), RecreateRenderTarget(), RenderChildrenToBuffer()
  - [ ] Manage RenderTarget2D lifetime (Dispose on unload/size change)
  - [ ] Compute buffer size based on DrawSize * pixelScale and clamp max
  - [ ] Respect inflation for effects (glow blur padding)
- [ ] Tests:
  - [ ] Buffer invalidates on child invalidation when expected.
  - [ ] Changing scale with RedrawOnScale triggers buffer recreation.
- [ ] TODOs:
  - [ ] Implement partial invalidation (dirty rectangles) later if needed.

---

## Masking & Clipping
Responsibilities:
- Provide scissor-based clipping and stencil-based masking for rounded corners and complex shapes.

Checklist:
- [ ] Implement scissor rect push/pop using GraphicsDevice.RasterizerState and SpriteBatch.Begin overloads when possible
- [ ] Implement stencil masking for rounded corners:
  - [ ] Draw mask shape to stencil buffer
  - [ ] Draw children with stencil test enabled
- [ ] Tests:
  - [ ] Rounded corner masking clipping is visually correct in sample scenes.
- [ ] TODOs:
  - [ ] Optimize mask batching where possible.

---

## Input Dispatcher & Hit testing
Responsibilities:
- Convert input screen coordinates to scene graph, perform hit-tests respecting clip & transforms, dispatch events.

Checklist:
- [ ] Class: InputManager/SceneInputHandler
  - [ ] Methods: HitTest(Vector2 screenPos) -> Drawable
  - [ ] Hit test uses WorldToLocal conversion and checks against Local bounds
  - [ ] Honor Masking/Clipping (if parent clipped, child cannot be hit)
  - [ ] Optional pixel-perfect hit testing using texture alpha (expensive)
  - [ ] Input event pipeline: top-down capture/bubble phases (configurable)
- [ ] Tests:
  - [ ] Hit testing returns correct node under nested transforms (rotation/scale).
  - [ ] Clipping prevents hits outside parent content rect.

---

## Pooling & Particle system
Responsibilities:
- Provide Pool<T> for poolable drawables and particles, and an example ParticleSpewer using pooled particles.

Checklist:
- [ ] Class: ObjectPool<T>
  - [ ] Methods: Get(), Return(T)
  - [ ] Optionally support max pool size and warm-up
- [ ] Interface: IPoolable or PoolableDrawable
  - [ ] Methods: ResetForReuse(), OnAcquire(), OnReturn()
- [ ] Particle system:
  - [ ] Particle struct (no allocation per particle)
  - [ ] ParticleSpewer as Drawable: spawn, update particle array, draw via DrawNode or direct rendering
- [ ] Tests:
  - [ ] Pool returns same instance after return, states reset correctly.
- [ ] TODOs:
  - [ ] Add automatic pool eviction policy and tracking of instance leaks.

---

## Layout system (AutoSize & Relative sizing)
Responsibilities:
- Measure/Arrange pass similar to UI frameworks; compute sizes for AutoSizeAxes; provide invalidation propagation.

Checklist:
- [ ] Layout pass ordering:
  - [ ] Pre-Update: Measure/Arrange for nodes with layout dirty flag
  - [ ] Update: Animated transforms and invalidation of layout/transform as a result
- [ ] Methods:
  - [ ] Measure() -> returns preferred size based on intrinsic content (sprites/text/children)
  - [ ] Arrange(Rectangle parentContent)
- [ ] Relative size computation:
  - [ ] Recalculate child size when parent's content rect changes
- [ ] Tests:
  - [ ] Nested autosize containers produce correct sizes in various nesting orders.
- [ ] TODOs:
  - [ ] Provide layout constraint system (min/max size) and flexible alignment.

---

## Lifecycle & Async loading
Responsibilities:
- Background decode of assets, main-thread creation, dependency injection pattern for services.

Checklist:
- [ ] Background loading pipeline:
  - [ ] Start decode on thread pool
  - [ ] On decode complete, enqueue GPU creation on main thread via Scene.EnqueueMainThreadAction
- [ ] Drawable load lifecycle:
  - [ ] LoadStart (enqueue tasks), LoadComplete (resources available), OnLoadComplete callback
  - [ ] Unload: release texture refs, stop tasks, free render targets
- [ ] Tests:
  - [ ] Simulate background tasks to ensure main-thread creation occurs reliably.
- [ ] TODOs:
  - [ ] Add cancellation tokens to background loads and ensure cancellation when Drawable expired.

---

## Renderer abstraction (IRenderer)
Responsibilities:
- Abstract the concrete draw commands from SpriteBatch/MonoGame API, enabling swapping backends or simplifying batching.

Checklist:
- [ ] Interface: IRenderer
  - [ ] Methods: DrawTexture(Texture2D, Quad/Rect, Colour, ShaderParams), SetScissor(Rect), PushTransform(Matrix), PopTransform(), CreateRenderTarget, PresentRenderTarget (for advanced usages)
- [ ] Implementation: MonoGameRenderer (wraps SpriteBatch and GraphicsDevice)
- [ ] Tests:
  - [ ] Basic draw calls produce expected visual output in a sample scene.
- [ ] TODOs:
  - [ ] Add methods for low-level GPU operations if needed (custom shaders, vertex batches).

---

## Tests & Benchmarks
Responsibilities:
- Unit tests for mathematical correctness, integration tests for visual layout, performance tests for scaling.

Checklist:
- [ ] Unit tests for:
  - [ ] Transform math: local + world matrix compositions
  - [ ] Anchor/origin behaviors
  - [ ] RelativeSize and AutoSize
  - [ ] ValueTransform composition & inverse
  - [ ] DrawNode snapshot immutability
  - [ ] TextureStore refcounting and async load flow
  - [ ] Pooling and Expire lifecycle
- [ ] Integration tests:
  - [ ] Visual fixtures (grid of nodes with anchors/pivots)
  - [ ] Particle stress scene with thousands of particles to measure update & draw costs
- [ ] Benchmarks:
  - [ ] Measure per-frame GC allocations (target near-zero per frame)
  - [ ] Measure draw calls and batched draws for a typical scene
- [ ] TODOs:
  - [ ] Create CI pipeline to run unit tests and headless benchmark harness.

---

## Debugging & Developer Tools
Responsibilities:
- Visual debug overlays and instrumentation.

Checklist:
- [ ] DebugDraw: draw bounding boxes, anchors, pivot points and local axes for nodes
- [ ] Stats overlay: FPS, draw calls, texture count, allocated memory
- [ ] Hot reload support: allow swapping textures at runtime to test skins
- [ ] TODOs:
  - [ ] Add logging for invalidation propagation to help track redundant recomputations.

---

## Integration examples & sample scenes
Responsibilities:
- Small example scenes for manual verification and demonstration.

Checklist (example scenes to implement):
- [ ] AnchorPivotDemo: grid showing each Anchor + Origin combination with rotation sliders.
- [ ] RelativeSizeDemo: parent resizing and children with RelativeSizeAxes change visually.
- [ ] BufferedContainerDemo: glow & blur example using RenderTarget2D with inflated quad.
- [ ] ParticleSpewerDemo: spawn rate controls, pooling/toggle.
- [ ] HitTestDemo: show hit test highlights under rotation/scale/clipping.
- [ ] TODOs:
  - [ ] Provide a single test harness that can load each demo and snapshot expected screenshots for visual tests.

---

## Cross-cutting concerns & best practices
- [ ] Use struct math (Matrix3x2/Vector2) for speed.
- [ ] Avoid allocations in Update/Draw hot paths (draw nodes should be pooled).
- [ ] Use dirty flags and lazy recompute.
- [ ] Keep GPU resource creation on main thread only.
- [ ] Provide clear documentation comments for APIs (coordinate spaces, transform order).
- [ ] Add configurable epsilon for float comparisons.

---

## Implementation Milestones / TODO list (actionable)
- [ ] Implement Scene root & integration with MonoGame Game (basic update/draw loop).
- [ ] Implement Drawable base with local/world transform caching and Invalidate system.
- [ ] Implement Container with add/remove children semantics.
- [ ] Implement TextureStore sync variant and basic Sprite using it.
- [ ] Add basic input hit testing using WorldToLocal conversions.
- [ ] Implement SpriteText wrapper (basic).
- [ ] Implement ObjectPool<T> and PoolableDrawable interface.
- [ ] Implement ParticleSpewer using pooled particles.
- [ ] Implement async resource pipeline (background decode + main-thread creation).
- [ ] Implement DrawNode snapshotting and change Scene draw to consume DrawNodes.
- [ ] Implement BufferedContainer (RenderTarget caching).
- [ ] Implement masking/clipping (scissor first, then stencil for rounded masks).
- [ ] Add layout system (Measure/Arrange) for AutoSize and RelativeSizeAxes.
- [ ] Add tests for each milestone.

---

## Deliverables (for PR)
- [ ] Core API implementation (Scene, Drawable, Container, Sprite, TextureStore).
- [ ] DrawNode snapshot system and renderer abstraction.
- [ ] BufferedContainer and ParticleSpewer example.
- [ ] Pooling utilities and tests.
- [ ] Unit & integration tests and sample scenes.
- [ ] Documentation page with transform math and usage examples.

---

## Notes & rationale narration
This checklist turns the earlier Drawable transform spec into a project-wide implementation plan and task list. It ensures each subsystem has explicit acceptance criteria and tests, and that implementation order flows from a minimal working core to more advanced features like buffering and draw snapshotting.

Use this file as a "definition of done" for each feature: check items as you implement and attach tests to validate behaviors. The lists include TODOs for optimization and future improvements that can be deferred until correctness and basic performance are achieved.
