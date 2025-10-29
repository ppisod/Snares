
# MonoGameRenderer Spec & Checklist

Purpose
- [ ] Provide a concrete MonoGame implementation of an IRenderer abstraction for the scene-graph.
- [ ] Expose a minimal, testable, batch-friendly API that DrawNodes can consume.
- [ ] Encapsulate all GraphicsDevice/SpriteBatch/RenderTarget2D/Stencil/Scissor interactions so the rest of the engine does not need MonoGame specifics.
- [ ] Provide diagnostics, deterministic behavior for tests, safe main-thread GPU resource creation and clear error handling.

High-level goals
- [ ] Keep API small and focused: Draw textured quads, push/pop clip & transform, manage render targets.
- [ ] Batch when possible: avoid Begin/End per draw; group by texture / shader / blend state.
- [ ] Ensure all GPU object creation/disposal happens on main thread.
- [ ] Make renderer behavior deterministic and observable for unit tests.

Who uses this
- [ ] Scene: orchestrates per-frame BeginFrame/EndFrame and DrawNode lists.
- [ ] DrawNode factory: creates DrawNodes that contain world-space quads or per-draw matrices.
- [ ] BufferedContainer: creates and consumes RenderTarget handles via renderer.
- [ ] TextureStore: requests creation of Texture2D (renderer responsible for wrapping/creating RenderTargetHandles etc.).

Design constraints & decisions
- [ ] Renderer will use SpriteBatch for common textured quad draws, and GraphicsDevice calls for stencil operations and render target management.
- [ ] DrawNodes will supply either a screen-space Quad (4 points) OR a transform matrix + source rect. Prefer screen-space Quads for renderer simplicity.
- [ ] Renderer will expose a RenderTargetHandle (opaque struct) rather than raw RenderTarget2D to keep layer boundaries.
- [ ] All push/pop stacks (Transform, Scissor, Stencil) must be balanced; debug builds assert mismatches.
- [ ] Renderer keeps internal batching and flushes automatically on state changes.

Public API (interface sketch - expand as needed)
- [ ] Create interface/contract
  - [ ] BeginFrame()
  - [ ] EndFrame()
  - [ ] CreateRenderTarget(int width, int height, SurfaceFormat fmt = SurfaceFormat.Color) -> RenderTargetHandle
  - [ ] ReleaseRenderTarget(RenderTargetHandle)
  - [ ] SetRenderTarget(RenderTargetHandle? target) // null = backbuffer
  - [ ] PushTransform(in Matrix3x2) / PopTransform()
  - [ ] PushScissor(Rectangle scissor) / PopScissor()
  - [ ] PushStencilMask(Action drawMask) / PopStencilMask()
  - [ ] DrawTexturedQuad(Texture2D tex, in Quad quad, in RectangleF? source, Color colour, BlendState? blend = null, Effect? effect = null)
  - [ ] DrawRect(in RectangleF rect, Color colour)
  - [ ] Flush()
  - [ ] GetStats() -> RenderStats (drawcalls, batches, texture switches, triangles)
  - [ ] Dispose()
- [ ] Example signature block (put in README/tests as reference):
  ```csharp
  struct RenderTargetHandle { internal readonly int id; }
  struct RenderStats { public int DrawCalls; public int BatchCount; public int TextureBindings; }
  interface IMonoGameRenderer : IDisposable
  {
      void BeginFrame();
      void EndFrame();

      RenderTargetHandle CreateRenderTarget(int w, int h, SurfaceFormat fmt = SurfaceFormat.Color);
      void ReleaseRenderTarget(RenderTargetHandle rt);
      void SetRenderTarget(RenderTargetHandle? target);

      void PushTransform(in Matrix3x2 m);
      void PopTransform();

      void PushScissor(Rectangle scissor);
      void PopScissor();

      // Apply mask by drawing mask geometry with the provided action while the renderer sets stencil op.
      void PushStencilMask(Action drawMask);
      void PopStencilMask();

      void DrawTexturedQuad(Texture2D texture, in Quad screenQuad, in RectangleF? sourceRect, Color colour, BlendState? blend = null, Effect? effect = null);
      void DrawRect(in RectangleF rect, Color colour);

      void Flush();
      RenderStats GetStats();
  }
  ```

Checklist — Implementation steps (concrete)
- Initialization & lifecycle
    - [ ] Implement MonoGameRenderer constructor accepting GraphicsDevice and optional SpriteBatch or create an internal SpriteBatch.
    - [ ] Implement BeginFrame(): reset per-frame counters; set up SpriteBatch if needed.
    - [ ] Implement EndFrame(): flush any remaining batches; restore device state if changed.
    - [ ] Implement Dispose(): dispose internal SpriteBatch and release known RenderTargets.

- RenderTarget management
    - [ ] Implement CreateRenderTarget: create RenderTarget2D with given parameters; track handle -> RenderTarget2D mapping.
    - [ ] Assign monotonically increasing id for handles; store width/height/format.
    - [ ] Implement SetRenderTarget(handle): bind/unbind target with GraphicsDevice.SetRenderTarget; handle switching with proper SpriteBatch.End/Begin to avoid state conflicts.
    - [ ] Implement ReleaseRenderTarget: dispose RenderTarget2D and remove mapping; guard against double-release.

- Transform stack
    - [ ] Maintain a stack<Matrix3x2>. PushTransform multiplies with top-of-stack and sets current transform; PopTransform restores prior transform.
    - [ ] Provide method to get current world matrix (used for SpriteBatch.Begin if you choose).
    - [ ] Ensure transform pushes/pop flush batching if using SpriteBatch Begin with different transform requirement.

- Scissor rect stack
    - [ ] Implement PushScissor(Rectangle) / PopScissor().
    - [ ] Use GraphicsDevice.RasterizerState with ScissorTestEnable = true.
    - [ ] When scissor changes, flush SpriteBatch if current Begin mode does not support runtime scissor changes.
    - [ ] Maintain a stack for nested scissor rectangles: push computes intersection with current and sets GraphicsDevice.ScissorRectangle accordingly.
    - [ ] Restore previous scissor on PopScissor.

- Stencil masking
    - [ ] Implement PushStencilMask(Action drawMask):
        - [ ] Flush batches.
        - [ ] Configure GraphicsDevice to write to stencil buffer by rendering mask shape (drawMask() will call renderer.DrawRect / DrawTexturedQuad with a special Effect or immediate vertex draw to set stencil).
        - [ ] Switch render states to use stencil test for subsequent child draws.
        - [ ] Track nesting; PopStencilMask restores previous stencil state and clears stencil writes if needed.
    - [ ] Tests/Docs: document stencil limitations on platforms (some platforms may not support stencil in certain target formats).

- Drawing primitives & batching
    - [ ] Core draw: DrawTexturedQuad should:
        - [ ] Accept a Texture2D reference, Quad (4 vertices with positions + UVs), source rect override, color, blend state, effect.
        - [ ] Append to an internal batch if compatible with previous draws (same texture, effect, blend state).
        - [ ] If incompatible, flush current batch and start new one.
    - [ ] Implement batching scheme:
        - [ ] Batches are groups of quads with same texture + effect + blend state.
        - [ ] Buffer vertex data into a dynamic vertex buffer (or use SpriteBatch) and issue a single DrawUserPrimitives or SpriteBatch call.
    - [ ] Implement DrawRect as a small helper (fills a rectangle using white 1x1 texture or dynamic vertex quad).
    - [ ] Ensure DrawTexturedQuad handles Quad vertices transformed (no additional transform multiplication inside renderer if DrawNode provides quad already).

- SpriteBatch integration options (choose one approach)
    - Option A: Use SpriteBatch for all draws where possible
        - [ ] Map DrawTexturedQuad to SpriteBatch.Draw calls (computing position, origin, scale and rotation from quad or by pre-transforming positions into screen-space and using SpriteBatch overloads that draw quads if available).
        - [ ] Manage Begin/End carefully with RasterizerState for scissor and BlendState for different blends.
    - Option B: Use custom vertex buffer + effect for maximum control & batching
        - [ ] Implement VertexPositionColorTexture vertex format and dynamic buffer.
        - [ ] Render batches via GraphicsDevice.DrawUserPrimitives or DrawIndexedPrimitives.
    - [ ] Implement at least one option and provide a configuration flag.

- State & flush strategy
    - [ ] Flush on:
        - [ ] Texture change
        - [ ] Blend state change
        - [ ] Effect/shader change
        - [ ] Scissor / stencil state change
        - [ ] RenderTarget change
        - [ ] Manual Flush() calls
    - [ ] When flushing, increment drawcall counters and reset current batch.

- Pixel-snapping / precision
    - [ ] Provide optional flag to snap quad vertex positions to integer pixel coordinates before issuing draws.
    - [ ] Implement SnapToPixelGrid option and ensure rounding occurs at the last step (renderer level).

- Error handling & validation
    - [ ] Debug-mode asserts for:
        - [ ] Unbalanced Push/Pop calls
        - [ ] Use of disposed RenderTargetHandle
        - [ ] Attempt to create RenderTarget with zero dimension
    - [ ] Throw descriptive exceptions for invalid usage in release builds only when necessary (prefer returning gracefully where possible).

- Diagnostics & stats
    - [ ] Track: draw calls, batches, texture binds, vertices drawn, active render targets.
    - [ ] Expose GetStats() returning a struct with these metrics.
    - [ ] Provide optional logging callback to record flush events and reasons during debugging.

- Deterministic behavior for tests
    - [ ] Allow injection of a "test mode" where actual GPU draws are skipped but batching logic still runs and stats increment for verification.
    - [ ] Provide deterministic handle IDs for RenderTargetHandle in test mode.
    - [ ] Allow a headless GraphicsDevice (or mock IMonoGameRenderer) for unit tests that only validate state changes & batching logic.

Testing checklist
- Unit tests (fast, run in CI)
    - [ ] Test BeginFrame/EndFrame resets counters and does not throw on repeated calls.
    - [ ] Test CreateRenderTarget -> SetRenderTarget -> ReleaseRenderTarget lifecycle: mapping, creation, and disposal behaviors (in test mode).
    - [ ] Test Push/Pop Transform stack: resulting current transform matches expected matrix composition, assert on mismatch stacks.
    - [ ] Test Push/Pop Scissor: intersection logic and restoration of previous scissor rect.
    - [ ] Test PushStencilMask / PopStencilMask: verify correct sequencing and that drawMask action is invoked when entering mask; in test-mode validate stencil state transitions recorded.
    - [ ] Test DrawTexturedQuad batching:
        - [ ] Batching groups draws with same texture and blend state.
        - [ ] Flush happens on state change (texture/effect/blend).
        - [ ] Stats reflect expected draw calls and batches.
    - [ ] Test pixel-snapping: positions are rounded when flag enabled.
    - [ ] Test Flush() behavior and that subsequent draws start a new batch.
    - [ ] Test error conditions (unbalanced Pop, release twice) produce expected asserts/exceptions in debug/test mode.

- Integration tests (may require actual GraphicsDevice)
    - [ ] Render textured quad to render target and read back pixels to verify expected color presence.
    - [ ] Render nested scissor rectangles and verify child pixels are correctly clipped.
    - [ ] Apply stencil mask: draw mask shape then child content and verify masked pixels.
    - [ ] Render to RenderTarget, then sample the RenderTarget to verify content (pixel-perfect where possible).

- Performance tests / Benchmarks
    - [ ] Measure batching efficiency: number of draw calls when drawing N quads grouped vs ungrouped.
    - [ ] Stress test: create many small quads and ensure per-frame allocations are minimal and renderer scales linearly.
    - [ ] Memory leak checks: create/release many RenderTargets and ensure resources are freed.

Edge cases & platform specifics
- [ ] Platforms with limited stencil support: detect and fallback to scissor-only masking or offscreen masked RenderTarget creation.
- [ ] GLES/OpenGL drivers might differ in scissor/stencil semantics — include compatibility notes and tests per-platform.
- [ ] VR/High-DPI: ensure RenderTarget size (pixel scale) is properly handled; allow specifying or querying device DPI scaling.

Integration with TextureStore & DrawNode
- [ ] DrawNodes should provide either:
    - [ ] screenQuad (preferred), sourceRect, texture pointer, and state flags; OR
    - [ ] texture + local transform + sourceRect (renderer then multiplies vertices).
- [ ] Renderer must not hold strong references to Texture2D beyond what MonoGame requires; resource lifetime tracked by TextureStore.
- [ ] Texture creation from decoded bytes must be performed on main thread: TextureStore will call renderer helper or GraphicsDevice directly to create GPU texture.

Testing utilities & mocks
- [ ] Implement RendererTestMode which:
    - [ ] Records batches and draws but does not call GPU draw functions.
    - [ ] Exposes recorded draw calls for assertions (texture id, vertex counts, reasons for flush).
    - [ ] Provides deterministic RenderTargetHandle ids.
- [ ] Provide MockGraphicsDevice or wrapper to let integration tests run on CI without full GPU (if your environment supports it). Otherwise, provide a headless mode and skip integration tests on CI unless GPU available.

Documentation & examples
- [ ] Provide README section:
    - [ ] Explain common usage patterns: drawing quads, push/pop stacks, render target usage, scissor vs stencil.
    - [ ] Show sample code: how Scene draws DrawNodes through renderer.
    - [ ] Explain thread-safety rules: GPU operations must run on main thread.
- [ ] Provide example scenes:
    - [ ] SimpleQuadDemo: draws many quads showing batching effect.
    - [ ] ScissorDemo: nested scissor areas.
    - [ ] StencilMaskDemo: rounded mask usage.
    - [ ] BufferedContainerDemo: render to texture, then draw texture with glow/inflation.

Deliverables checklist (PR readiness)
- [ ] Implementation of MonoGameRenderer class implementing interface above.
- [ ] Unit test suite covering core behaviors and edge cases.
- [ ] Test-mode renderer and/or mocks for headless unit tests.
- [ ] Integration test harnesses for render-target verification (optional on CI).
- [ ] Example scenes and README documentation.
- [ ] Benchmarks results and notes on performance tuning.
- [ ] API docs/comments for each public method.

Implementation notes & tips
- [ ] Minimize Begin/End lifecycle calls of SpriteBatch. If you call SpriteBatch.Begin/End per flush, ensure draw calls grouped to reduce counts.
- [ ] When scissor/stencil or render target state changes, SpriteBatch must be ended and begun again with appropriate states — encapsulate this logic inside renderer to avoid callers having to manage it.
- [ ] Use dynamic vertex buffers for custom batching if you need more control than SpriteBatch offers.
- [ ] Provide small white-pixel 1x1 texture for DrawRect helper to use via SpriteBatch.
- [ ] Keep per-frame allocations near zero: reuse arrays, vertex buffers, and DrawNode objects via pools.
- [ ] Keep debug assertions to help catch misuse early (e.g., Pop without Push).

Small implementation timeline (suggested)
- [ ] Week 1: implement skeleton MonoGameRenderer with BeginFrame/EndFrame, RenderTarget creation/release, Push/Pop transform & scissor stack (no batching yet).
- [ ] Week 2: implement DrawTexturedQuad mapped to SpriteBatch and naive flush-on-state-change; add unit tests for stacks & flush logic.
- [ ] Week 3: implement batching optimizations or custom vertex buffer path; implement PushStencilMask and RenderTarget-based masking.
- [ ] Week 4: add test-mode, diagnostics, integration tests with pixel-readback verification; polish docs and examples.

Appendix — Test cases (quick list)
- [ ] Test_BeginEndFrame_NoThrow
- [ ] Test_CreateReleaseRenderTarget_Disposes
- [ ] Test_SetRenderTarget_Switching_ProperFlush
- [ ] Test_TransformStack_Composition
- [ ] Test_ScissorStack_IntersectionAndRestore
- [ ] Test_StencilMask_SequenceInvocation
- [ ] Test_DrawTexturedQuad_Batching_OnSameTexture
- [ ] Test_DrawTexturedQuad_FlushOnTextureChange
- [ ] Test_PixelSnapping_RoundsVertices
- [ ] Integration_RenderToTarget_ReadbackMatches
- [ ] Performance_Batching_Speedup_When_Grouped
