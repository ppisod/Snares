# Drawable: Transform, Anchoring, Relative Size & ValueTransform Spec

This document enumerates concrete responsibilities, APIs and acceptance criteria for expanding a Drawable node to include full local/world transform composition, anchoring/origin, relative sizing, and a small ValueTransform type (scale/offset pairs). Each item is presented as a checklist (for implementation & test coverage). Use these items to drive implementation, review and unit tests. Items are grouped by feature area and include expected behaviors, edge-cases and performance constraints.

Notes:
- "Parent" always refers to the immediate drawable parent in the scene graph.
- "World" means the composed transform from the root of the scene graph down to the node.
- "Local" means the node's own transform, before composition with the parent.
- "AABB" refers to axis-aligned bounding box in world (screen) space.
- "Pixel snapping" is optional but included as a behavior to consider for crisp UI rendering.

---

## High-level acceptance
- [ ] Define a minimal immutable Matrix/Transform representation used for composition (or use existing engine Matrix types). Implementation must allow efficient composition and caching.
- [ ] All transform computations must be lazy and cached; recompute only when relevant properties change.
- [ ] Transform invalidation must propagate to children efficiently and only when necessary.
- [ ] All API must be safe to call from the single main thread (MonoGame), but design should allow later separation of update vs render threading (i.e., snapshottable state).

---

## Core properties (state) to expose and persist
- [ ] Position: Vector2 (local offset from origin) — affects local matrix.
- [ ] Size: Vector2 (logical content size). When using RelativeSizeAxes, actual size may depend on parent's size.
- [ ] Scale: Vector2 (local scale, default Vector2(1,1)).
- [ ] Rotation: float (degrees or radians — choose convention and document).
- [ ] Origin: Vector2 (0..1 normalized pivot for the drawable's local coordinate system; (0.5,0.5) = center).
- [ ] Anchor: Enum (e.g., Anchor.TopLeft, Centre, BottomRight, or separate AnchorX/AnchorY). Anchor determines how the node is positioned relative to parent's layout area.
- [ ] RelativeSizeAxes: flags enum (None, X, Y, Both) — if set, Size.x/Size.y are treated as fractions of parent's content area.
- [ ] AutoSizeAxes: flags enum — if set, node computes Size from children/content.
- [ ] Padding / Margin: Margin struct to shift internal content / influence layout.
- [ ] Alpha/Opacity and Colour: included as part of transform snapshot considerations (alpha composes downwards).
- [ ] IsPresent / AlwaysPresent: for draw/presence rules.

Acceptance criteria:
- [ ] Document how Anchor and Origin interact with Position to produce final placement.
- [ ] Document precedence and interaction of RelativeSizeAxes vs explicit Size values vs AutoSizeAxes.

---

## ValueTransform type (scale/offset pair) — a small utility for mapping scalar/vector values
- [ ] Define a ValueTransform<T> generic-like spec (for floats and vectors) as a pair of (scale, offset) that maps: output = input * scale + offset.
- [ ] Support composition (Compose(other) = equivalent single scale/offset that applies other then this).
- [ ] Provide inversion (TryInvert) — compute inverse transform where possible.
- [ ] Provide lerp (interpolation) between two ValueTransforms for animated transitions.
- [ ] Provide convenience constructors: FromScale(float/Vector2), FromOffset(...), Identity.
- [ ] Provide a simple API comment specifying numerical tolerances for floating operations.

Acceptance criteria:
- [ ] Unit tests cover composition associativity and inversion for non-zero scales.
- [ ] Performance: operations should be allocation-free and simple math only.

---

## Matrix composition API (local/world transforms)
- [ ] Property / method: Matrix LocalMatrix { get; } (computed from Position, Scale, Rotation, Origin, Size).
    - Requirements:
        - [ ] LocalMatrix computation must apply transforms in the documented order (e.g., Translate(-Origin*Size) → Scale → Rotate → Translate(Position + AnchorOffset + Margin)).
        - [ ] Use consistent coordinate conventions and document whether rotation is clockwise/ccw and degrees/radians.
- [ ] Property / method: Matrix WorldMatrix { get; } (LocalMatrix composed with Parent.WorldMatrix if Parent != null).
    - Requirements:
        - [ ] WorldMatrix must be cached and only recomputed when LocalMatrix changes or Parent.WorldMatrix changes.
        - [ ] Changing any transform property on any ancestor must invalidate descendant WorldMatrix caches.
- [ ] Method: void InvalidateTransform(bool cascadeToChildren = true)
    - Requirements:
        - [ ] Sets dirty flags for LocalMatrix/WorldMatrix as appropriate.
        - [ ] Default behavior cascades invalidation to all descendants.
        - [ ] When only local properties change, invalidate LocalMatrix and WorldMatrix of self and recursively WorldMatrix of children (but avoid recomputing until needed).
- [ ] Method: void ForceRecomputeTransform()
    - Requirements:
        - [ ] Immediately recompute LocalMatrix and WorldMatrix (useful for deterministic tests).
- [ ] Event/Callback: OnWorldMatrixChanged (invoked when WorldMatrix value actually changes after recompute).
    - Requirements:
        - [ ] Provide old/new matrices in event args.
        - [ ] Event invoked only when matrix value changes (consider floating-point equality tolerance).
- [ ] Helpers:
    - [ ] Matrix GetLocalToParentMatrix() — returns LocalMatrix (alias).
    - [ ] Matrix GetLocalToWorldMatrix() — returns WorldMatrix.
    - [ ] Matrix GetParentToLocalMatrix(), GetWorldToLocalMatrix() — compute inverse matrices (use cached inverses with lazy recompute).
- [ ] Provide method Vector2 TransformPositionLocalToWorld(Vector2 localPos) and inverse.
    - Requirements:
        - [ ] Avoid allocating matrices in hot paths (use cached matrices or out-params).
        - [ ] Correctly handle Origin pivot and relative sizing.

Acceptance criteria:
- [ ] Compose transforms with test cases (e.g., parent translation + child rotation yields expected world positions).
- [ ] Invalidations should not trigger full recompute unless ForceRecomputeTransform is called or matrix is requested.
- [ ] Check for numerical stability for chained compositions (provide acceptable epsilon for tests).

---

## Anchoring & Origin behavior (layout semantics)
- [ ] Anchor semantics:
    - [ ] Define Anchor area on the parent: for a Container it is typically the parent's content rectangle (considering padding).
    - [ ] Anchor places the drawable within that area; combined with Origin, compute Position offset.
- [ ] Origin semantics:
    - [ ] Origin is normalized (0..1) relative to Size by default.
    - [ ] Support both normalized and absolute origin (e.g., Origin.SetAbsolute(x,y) or boolean flag).
- [ ] Method: Vector2 ComputeAnchorOffset(RectangleF parentContentRect)
    - Requirements:
        - [ ] Return the position (in parent coords) where the drawable's origin should be placed for the given Anchor.
        - [ ] Handles all built-in anchor enums (top-left/centre/etc.) and fractional anchor values.
- [ ] Integration:
    - [ ] During local matrix computation, include the anchor offset in position calculation.
    - [ ] Anchor must respect parent padding/margin and clip region adjustments.

Acceptance criteria:
- [ ] Implement tests: anchor top-left, centre, bottom-right on various parent sizes and check world-space placements.
- [ ] Document how Anchor interacts with RelativeSizeAxes (e.g., if child's size depends on parent, anchor offset must consider new size).

---

## Relative sizing & autosizing
- [ ] Support RelativeSizeAxes flags (X/Y):
    - [ ] When a child has RelativeSizeAxes set, its Size component must be treated as fraction of the parent's available content size (parent's inner rect).
    - [ ] Provide method RecalculateSizeFromParent() invoked during layout invalidation.
- [ ] AutoSizeAxes:
    - [ ] When AutoSizeAxes is set, compute Size from children or intrinsic content (e.g., Sprite texture size).
    - [ ] Provide measurement pass: Size Measure(Axis) and Arrange(Position/Size).
- [ ] Layout invalidation:
    - [ ] Changing a parent's size must mark children with RelativeSizeAxes as needing recompute.
    - [ ] Provide an Invalidation flag enum (Layout, Transform, DrawNode) and appropriate invalidation propagation.
- [ ] Method signatures:
    - [ ] void InvalidateLayout(bool propagateUp = false) — marks layout dirty; if propagateUp true, notify parent (useful when child autosize influences parent).
    - [ ] void PerformLayoutIfNeeded() — internal call to recompute sizes & anchors.
- [ ] Edge-cases:
    - [ ] Parent not yet loaded: defer relative size calculation until parent is loaded or until first layout pass.
    - [ ] Zero-size parents: clamp child sizes or provide default safe behavior.

Acceptance criteria:
- [ ] Tests for:
    - [ ] Child with RelativeSizeAxes=X gets Size.X == parent.ContentWidth * specified fraction.
    - [ ] AutoSize computes correct size from children and handles nested autosize.
    - [ ] Re-layout when parent size changes triggers correct recompute order without infinite recursion.

---

## Transform order & pivoting (document precise math)
- [ ] Define and document the exact order of operations used to compute LocalMatrix. Example canonical order (choose and document):
    - [ ] 1) Translate(-Origin * Size) (move local origin to pivot)
    - [ ] 2) Scale(Scale)
    - [ ] 3) Rotate(Rotation around pivot)
    - [ ] 4) Translate(Position + AnchorOffset + Margin + Origin*SizeAdjustment)
- [ ] Provide alternative documented ordering if desired (e.g., rotate then scale) — choose a convention and stick to it.
- [ ] Provide example computations in comments or spec tests to demonstrate how pivot works with Size changes.

Acceptance criteria:
- [ ] Unit tests verifying rotation about origin pivot at several positions/sizes.

---

## Dirty flags & caching strategy
- [ ] Define an Invalidation enum: None, Transform, Layout, DrawNode, Visuals, etc.
- [ ] Each property setter (Position, Scale, Rotation, Size, Anchor, Origin, RelativeSizeAxes, etc.) must set appropriate flags.
- [ ] On request for LocalMatrix/WorldMatrix/Bounds, check cached state and recompute only if dirty.
- [ ] Caches to maintain:
    - [ ] LocalMatrix
    - [ ] WorldMatrix
    - [ ] InverseWorldMatrix
    - [ ] Screen-space AABB / Quad
- [ ] Provide concurrency notes: assume single-threaded updates; if threading later is added, expose snapshotting interface (see DrawNode section).

Acceptance criteria:
- [ ] Tests for repeated property sets: setting the same value must not mark as dirty.
- [ ] Tests for chained invalidations: parent transform change invalidates child world matrix.

---

## Screen-space quad / AABB and clipping
- [ ] Method: Quad ComputeScreenSpaceQuad()
    - [ ] Uses WorldMatrix to transform local rectangle corners to screen coordinates.
    - [ ] Provide AABB getter (axis-aligned bounding box) derived from transformed quad.
    - [ ] If a BufferedContainer inflates draw quad (like GlowingDrawable), provide an Inflate(float x, float y) param or compute from blur kernel size.
- [ ] Pixel snapping:
    - [ ] Optional method SnapToPixelGrid(bool enabled) — when enabled, apply rounding to world positions before drawing for crispness.
- [ ] Clipping:
    - [ ] Provide ClipToParent option (bool) for scissoring and ensure ComputeScreenSpaceQuad is used for scissor rect computation.
- [ ] Edge-cases:
    - [ ] Large rotations producing degenerate AABBs; ensure numerical stability and finite values.

Acceptance criteria:
- [ ] Tests verifying quad corner positions for translations, scales and rotations.
- [ ] Tests verifying clip rect calculation and proper scissor application in Render pipeline.

---

## DrawNode / Snapshot creation (render safety & performance)
- [ ] API: DrawNode CreateDrawNode() and void ApplyToDrawNode(DrawNode node)
    - [ ] DrawNode should be a lightweight immutable struct/class containing:
        - [ ] Texture references (if any), colour info, computed world transform matrix, AABB/quad, alpha, clipping rect, and any shader parameters.
    - [ ] During Update phase, call Create/Apply to prepare DrawNode(s).
    - [ ] Draw phase consumes DrawNode(s) only — no state reads from the Drawable itself.
- [ ] Ensure DrawNode recreation happens when Invalidation.DrawNode is set.
- [ ] Provide pooling of DrawNode instances to avoid allocations in hot paths.
- [ ] Provide thread-safety notes: if you later render on a separate thread, DrawNode must be fully independent.

Acceptance criteria:
- [ ] Implement tests that mutate Drawable after creating draw snapshot and ensure DrawNode represents the state at snapshot time (not the mutated state if snapshot was taken earlier).

---

## BufferedContainer semantics (interaction with transform & scale)
- [ ] When a BufferedContainer caches its children into a RenderTarget:
    - [ ] The render target size must be computed from the Drawable's content size multiplied by a pixel scale factor (handle high-DPI).
    - [ ] RedrawOnScale flag: if true, changing scale invalidates buffer and forces redraw.
    - [ ] DrawOriginal flag: if true, draw children as usual in addition to cached render (useful for blending).
- [ ] Buffered render target must be recreated on size change or when invalidated.
- [ ] Memory management:
    - [ ] Dispose old RenderTarget2D promptly when replaced/unloaded.
- [ ] Interaction with ComputeScreenSpaceQuad:
    - [ ] When using buffering that inflates draw area (glow), ensure ComputeScreenSpaceQuad accounts for inflation to avoid cropping.

Acceptance criteria:
- [ ] Tests verifying buffer redraw when Scale changes and when children change.
- [ ] Tests verifying RenderTarget recreation and disposal.

---

## Event & lifecycle hooks (integration points)
- [ ] OnAddedToScene(Scene scene) — called when node is attached to a scene or root.
    - [ ] Must queue/load resources (but do not perform GPU creation on background thread).
- [ ] OnRemovedFromScene() — clean up references, optionally return to pool.
- [ ] Load / Unload callbacks:
    - [ ] LoadBegin / LoadComplete / Unload — provide background-loading pattern and main-thread completion hooks.
- [ ] Expire() / Kill() method for scheduled removal:
    - [ ] On Expire, mark IsExpired and ensure Scene will remove and dispose in next update.

Acceptance criteria:
- [ ] Tests for lifecycle ordering: Load -> LoadComplete -> OnAddedToScene -> Update -> Draw -> Expire -> OnRemovedFromScene -> Unload.

---

## Interaction with input & hit testing
- [ ] Method: bool ContainsPoint(Vector2 screenPoint) / HitTest(Vector2 point)
    - [ ] Converts screenPoint to local coordinates (use inverse world matrix) and then test against local bounds considering Origin & Size.
- [ ] HitTest rules:
    - [ ] Honor masking/clipping — if parent clip excludes point, descendant hit test must fail.
    - [ ] Transparent/alpha hit testing optional (e.g., pixel-perfect); document how and when to do it.
- [ ] Events:
    - [ ] OnHover/OnClick/OnPressed/OnReleased hooks; assume input system dispatches events top-down or bottom-up depending on desired behavior.

Acceptance criteria:
- [ ] Test hit test conversion with rotated & scaled parents and verify correct local-space detection.

---

## Pooling, Expiration & Unloading semantics
- [ ] When Expire() is called:
    - [ ] Mark IsExpired; Scene must remove from parent and call Dispose/ReturnToPool.
- [ ] Implement optional PoolableDrawable pattern:
    - [ ] OnRecycle() — reset transforms, alpha, visibility and internal state.
    - [ ] Ensure pooled drawables are not still referenced by the scene graph or renderer when returned.
- [ ] TextureStore/resource refcounting:
    - [ ] Drawable must notify TextureStore when it no longer needs a texture (on Unload or Dispose).
    - [ ] Provide reference counting or usage tracking to avoid premature disposal.

Acceptance criteria:
- [ ] Test that Expire followed by Remove results in resources released (or refcount decremented) and that pooling returns an object to pool cleanly.

---

## Tests & verification checklist
- [ ] Unit tests for:
    - [ ] LocalMatrix calculation for combinations of Position, Scale, Rotation, Origin.
    - [ ] WorldMatrix composition for multiple ancestor transforms.
    - [ ] Anchor & Origin interactions across varied parent sizes.
    - [ ] RelativeSizeAxes recalculation and re-layout propagation.
    - [ ] ValueTransform composition & inversion.
    - [ ] DrawNode snapshot immutability.
    - [ ] Pixel snapping behavior when enabled.
    - [ ] Hit testing under transforms.
    - [ ] BufferedContainer invalidation and redraw.
- [ ] Integration tests / visual verification scenes:
    - [ ] Grid scene showing anchor origins and pivot rotations.
    - [ ] Resizing parent actor and ensuring children with relative sizes update visually.
    - [ ] Particle example to verify pooling, transforms and draw ordering.
- [ ] Performance tests:
    - [ ] Ensure frequent recalculations of transforms (e.g., many moving nodes) do not allocate per-frame.
    - [ ] Measure invalidation propagation cost on large hierarchies.

---

## Implementation notes & performance hints
- [ ] Use struct-based matrices (Matrix or Matrix3x2 where available) and Vector2 structs to avoid allocations.
- [ ] Avoid building new Matrix objects per vertex; reuse cached matrices and apply them to points via fast transforms.
- [ ] Use epsilon comparisons for floating equality on matrices / vectors to avoid churn.
- [ ] Prefer marking "dirty" and lazy recompute over immediate recompute in setters to reduce redundant work.
- [ ] When supporting both normalized origin and absolute origin, store a flag and compute absolute pivot on layout pass.
- [ ] Consider separating "layout pass" and "update pass" to keep responsibilities clear:
    - [ ] Layout: resolve autosize & relative sizes; compute anchor offsets.
    - [ ] Update: apply animation transforms (which may modify Position/Scale/Rotation) and then InvalidateTransform.
    - [ ] Render snapshot: create DrawNode copying matrix, alpha, texture refs.

---

## Example checklist items for method implementations (signature & behavior guidance)
- [ ] Vector2 ComputeAnchorOffset(RectangleF parentRect)
    - [ ] Must compute pixel coordinates where the drawable's origin should be placed for the given Anchor.
- [ ] void RecalculateSizeFromParent()
    - [ ] If RelativeSizeAxes is set, compute Size.x/y appropriately; mark Layout invalidation.
- [ ] void UpdateLocalMatrixIfNeeded()
    - [ ] If local dirty flag set, compute LocalMatrix using Size, Origin, Scale, Rotation, Position and clear flag.
- [ ] void UpdateWorldMatrixIfNeeded()
    - [ ] If world dirty or parent world changed, set WorldMatrix = Parent.WorldMatrix * LocalMatrix (or Local * Parent depending on convention), clear flag and fire OnWorldMatrixChanged if value changed.
- [ ] Quad ComputeScreenSpaceQuad()
    - [ ] Transform the local rectangle corners via WorldMatrix and return Quad.
- [ ] Vector2 LocalToWorld(Vector2 local)
    - [ ] Multiply local point by WorldMatrix and return result.
- [ ] Vector2 WorldToLocal(Vector2 world)
    - [ ] Multiply by inverse world matrix; if inverse not available, compute lazily.
- [ ] void SetOriginNormalized(Vector2 normalizedOrigin)
    - [ ] Store as normalized origin, mark transform/layout invalid.
- [ ] void SetAnchor(Anchor anchor)
    - [ ] Set anchor and mark layout/transform invalid.
- [ ] void SetRelativeSizeAxes(Axes axes)
    - [ ] Set flag and mark layout invalid; if set, RecalculateSizeFromParent must be scheduled.
- [ ] ValueTransform<T> Compose(ValueTransform<T> other)
    - [ ] Return new transform representing application of other then this.

---

## UX & API ergonomics (developer-facing)
- [ ] Property setters should be chainable where it makes sense (fluent API).
- [ ] Provide helper extension methods for common transforms (MoveTo, ScaleTo, RotateTo) that schedule transforms but do not force immediate recompute.
- [ ] Clear documentation comments for each method showing coordinate space semantics (local vs parent vs world).
- [ ] Provide debug visualization helpers (draw local axes, bounds and anchor gizmos) to aid debugging.

---

## Deliverables checklist (for PR)
- [ ] Implementation of Drawable transform system following the above spec.
- [ ] Unit test suite covering all items in "Tests & verification checklist".
- [ ] A small example scene demonstrating anchors, relative sizing and transform composition.
- [ ] Performance benchmark results for large hierarchies and many transform changes per frame.
- [ ] Documentation page summarizing transform math and usage examples.
