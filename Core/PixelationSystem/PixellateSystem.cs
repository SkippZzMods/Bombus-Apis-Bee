using BombusApisBee.Core.PixellateSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.PixellationSystem
{
    public class PixellateSystem : ModSystem
    {
        private readonly Dictionary<string, RenderingStepData> renderData;

        public PixellateSystem()
        {
            renderData = new Dictionary<string, RenderingStepData>();
        }

        public override void Load()
        {
            On_Main.DoDraw += PreparePrimitives;
            On_Main.DrawProjectiles += DrawProjectileTargets;
            On_Main.DrawDust += DrawDustTargets;

            Main.OnResolutionChanged += resolution =>
            {
                TargetsNeedResizing();
            };
        }

        public override void Unload()
        {
            Main.OnResolutionChanged -= resolution =>
            {
                TargetsNeedResizing();
            };

            foreach (RenderingStepData data in renderData.Values)
            {
                Main.QueueMainThreadAction(() =>
                {
                    data.RenderTarget.Dispose();
                });
            }
        }

        private void PreparePrimitives(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            if (Main.gameMenu || Main.dedServ)
            {
                orig(self, gameTime);
                return;
            }

            foreach (string id in renderData.Keys)
            {
                GraphicsDevice device = Main.graphics.GraphicsDevice;

                RenderTargetBinding[] bindings = device.GetRenderTargets();

                device.SetRenderTarget(renderData[id].RenderTarget);
                device.Clear(Color.Transparent);

                for (int i = 0; i < renderData[id].RenderEntries.Count; i++)
                {
                    renderData[id].RenderEntries[i].Invoke();
                }

                device.SetRenderTargets(bindings);

                Finish(id);
            }

            orig(self, gameTime);
        }

        private void DrawProjectileTargets(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            foreach (string id in renderData.Keys)
            {
                if (renderData[id].renderType == RenderType.Projectile)
                {
                    PixelPalette palette = renderData[id].Palette;

                    bool doNotApplyCorrection = palette.NoCorrection || Main.graphics.GraphicsProfile == GraphicsProfile.Reach;

                    Effect paletteCorrection = doNotApplyCorrection ? null : Filters.Scene["PaletteCorrection"].GetShader().Shader;

                    if (paletteCorrection != null)
                    {
                        paletteCorrection.Parameters["palette"].SetValue(palette.Colors);
                        paletteCorrection.Parameters["colorCount"].SetValue(palette.ColorCount);
                    }

                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                        DepthStencilState.None, RasterizerState.CullNone, paletteCorrection, Main.GameViewMatrix.TransformationMatrix);

                    Main.spriteBatch.Draw(renderData[id].RenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

                    Main.spriteBatch.End();
                }
            }
        }

        private void DrawDustTargets(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            foreach (string id in renderData.Keys)
            {
                if (renderData[id].renderType == RenderType.Dust)
                {
                    PixelPalette palette = renderData[id].Palette;

                    bool doNotApplyCorrection = palette.NoCorrection || Main.graphics.GraphicsProfile == GraphicsProfile.Reach;

                    Effect paletteCorrection = doNotApplyCorrection ? null : Filters.Scene["PaletteCorrection"].GetShader().Shader;

                    if (paletteCorrection != null)
                    {
                        paletteCorrection.Parameters["palette"].SetValue(palette.Colors);
                        paletteCorrection.Parameters["colorCount"].SetValue(palette.ColorCount);
                    }

                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                        DepthStencilState.None, RasterizerState.CullNone, paletteCorrection, Main.GameViewMatrix.TransformationMatrix);

                    Main.spriteBatch.Draw(renderData[id].RenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

                    Main.spriteBatch.End();
                }
            }
        }

        public void TargetsNeedResizing()
        {
            Unload();

            foreach (string id in renderData.Keys)
            {
                PixelPalette palette = renderData[id].Palette;

                RenderType renderType = renderData[id].renderType;

                renderData[id] = new(palette, renderType);
            }
        }

        /// <summary>
        /// Registers a rendertarget for use with a drawing action or list of drawing actions.
        /// </summary>
        /// <param name="id">ID of the rendertarget and its layer.</param>
        public void RegisterRenderTarget(string id, RenderType renderType = RenderType.Projectile)
        {
            Main.QueueMainThreadAction(() =>
            {
                PixelPalette palette = new();

                renderData[id] = new RenderingStepData(palette, renderType);
            });
        }

        /// <summary>
        /// Registers a rendertarget for use with a drawing action or list of drawing actions. This is used so that all draw calls of a needed palette can be done with a single RT.
        /// </summary>
        /// <param name="id">ID of the rendertarget and its layer.</param>
        /// <param name="palettePath">The given palette's texture path.</param>
        public void RegisterRenderTargetWithPalette(string id, string palettePath, RenderType renderType = RenderType.Projectile)
        {
            Main.QueueMainThreadAction(() =>
            {
                PixelPalette palette = PixelPalette.From(palettePath);

                renderData[id] = new RenderingStepData(palette, renderType);
            });
        }

        public void QueueRenderAction(string id, Action renderAction)
        {
            renderData[id].RenderEntries.Add(renderAction);
        }

        private void Finish(string id)
        {
            renderData[id].RenderEntries.Clear();
        }

        private struct RenderingStepData
        {
            public List<Action> RenderEntries;

            public RenderTarget2D RenderTarget;

            public PixelPalette Palette;

            public RenderType renderType;

            public RenderingStepData(PixelPalette palette, RenderType renderType)
            {
                RenderEntries = new List<Action>();

                RenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, false,
                    SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                Palette = palette;

                this.renderType = renderType;
            }
        }
    }

    public enum RenderingStep
    {
        PreDraw,
    }

    public enum RenderType : int
    {
        Projectile = 0,
        Dust = 1,
        NPC = 2,
    }
}
