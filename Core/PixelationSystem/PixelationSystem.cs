using BombusApisBee.Core.PixellateSystem;
using BombusApisBee.Core.PixellationSystem;
using BombusApisBee.Core.ScreenTargetSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core.PixelationSystem
{
    public class PixelationSystem : ModSystem
    {
        public List<PixelationTarget> pixelationTargets = new List<PixelationTarget>();

        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawNPCs += DrawTargets;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            On_Main.DrawNPCs -= DrawTargets;
        }

        private void DrawTargets(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            SpriteBatch sb = Main.spriteBatch;

            for (int i = 0; i <  pixelationTargets.Count; i++)
            {
                PixelationTarget target = pixelationTargets[i];

                if (target.Active)
                {
                    PixelPalette palette = target.palette;

                    bool doNotApplyCorrection = palette.NoCorrection || Main.graphics.GraphicsProfile == GraphicsProfile.Reach;

                    Effect paletteCorrection = doNotApplyCorrection ? null : Filters.Scene["PaletteCorrection"].GetShader().Shader;

                    if (paletteCorrection != null)
                    {
                        paletteCorrection.Parameters["palette"].SetValue(palette.Colors);
                        paletteCorrection.Parameters["colorCount"].SetValue(palette.ColorCount);
                    }

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                        DepthStencilState.None, RasterizerState.CullNone, paletteCorrection, Main.GameViewMatrix.EffectMatrix);

                    sb.Draw(target.pixelationTarget.RenderTarget, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

                    sb.End();
                    sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
                }
            }
        }

        /// <summary>
        /// Registers a ScreenTarget for use with a drawing action or list of drawing actions.
        /// </summary>
        /// <param name="id">ID of the rendertarget and its layer.</param>
        public void RegisterScreenTarget(string id, RenderType renderType = RenderType.Projectile)
        {
            Main.QueueMainThreadAction(() =>
            {
                pixelationTargets.Add(new PixelationTarget(id, new PixelPalette(), renderType));
            });
        }

        /// <summary>
        /// Registers a ScreenTarget for use with a drawing action or list of drawing actions. This is used so that all draw calls of a needed palette can be done with a single ScreenTarget.
        /// </summary>
        /// <param name="id">ID of the rendertarget and its layer.</param>
        /// <param name="palettePath">The given palette's texture path.</param>
        public void RegisterScreenTarget(string id, string palettePath, RenderType renderType = RenderType.Projectile)
        {
            Main.QueueMainThreadAction(() =>
            {
                pixelationTargets.Add(new PixelationTarget(id, PixelPalette.From(palettePath), renderType));
            });
        }

        public void QueueRenderAction(string id, Action renderAction)
        {
            PixelationTarget target =  pixelationTargets.Find(t => t.id == id);

            target.pixelationDrawActions.Add(renderAction);
            target.renderTimer = 2;
        }
    }

    public class PixelationTarget
    {
        public int renderTimer;

        public string id;

        public List<Action> pixelationDrawActions;

        public ScreenTarget pixelationTarget;

        public PixelPalette palette;

        public RenderType renderType;

        public bool Active => renderTimer > 0;

        public PixelationTarget(string id, PixelPalette palette, RenderType renderType)
        {
            pixelationDrawActions = new List<Action>();

            pixelationTarget = new(DrawPixelTarget, () => Active, 1, Resize);

            this.palette = palette;

            this.renderType = renderType;

            this.id = id;
        }

        private Vector2? Resize(Vector2 obj)
        {
            return new Vector2(obj.X / 2, obj.Y / 2);
        }

        private void DrawPixelTarget(SpriteBatch sb)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            for (int i = 0; i < pixelationDrawActions.Count; i++)
            {
                pixelationDrawActions[i].Invoke();
            }

            pixelationDrawActions.Clear();
            renderTimer--;

            sb.End();
            sb.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}
