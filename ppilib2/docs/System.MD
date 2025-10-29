
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
