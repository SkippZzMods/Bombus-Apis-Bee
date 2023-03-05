using BombusApisBee.Core.Metaballs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class BandOfTheHive_Metaballs : MetaballActor
    {
        public override bool Active => Main.projectile.Any(x => x.active && x.type == ModContent.ProjectileType<BandHoneyGlob>()) || Main.dust.Any(x => x.active && x.type == ModContent.DustType<BandHoneyDust>());

        public override Color outlineColor => new Color(255, 245, 40);

        public override bool overNPCS => true;

        public override void DrawShapes(SpriteBatch spriteBatch)
        {
            Effect borderNoise = Filters.Scene["BorderNoise"].GetShader().Shader;

            if (borderNoise is null)
                return;

            borderNoise.Parameters["offset"].SetValue((float)Main.time / 100f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            borderNoise.CurrentTechnique.Passes[0].Apply();

            var honey = Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<BandHoneyGlob>());
            foreach (Projectile proj in honey)
            {
                (proj.ModProjectile as BandHoneyGlob).DrawMetaball();
            }

            var honeyDust = Main.dust.Where(n => n.active && n.type == ModContent.DustType<BandHoneyDust>());
            foreach (Dust dust in honeyDust)
            {
                borderNoise.Parameters["offset"].SetValue((float)Main.time / 1000f + dust.rotation);
                spriteBatch.Draw(ModContent.Request<Texture2D>("BombusApisBee/Items/Accessories/BeeKeeperDamageClass/BandHoneyDust").Value, (dust.position - Main.screenPosition) / 2, null, Color.White, 0f, Vector2.One * 256f, dust.scale / 64f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override bool PostDraw(SpriteBatch spriteBatch, Texture2D target)
        {
            Effect effect = Filters.Scene["HoneyNoise"].GetShader().Shader;
            effect.Parameters["noiseScale"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 250);
            effect.Parameters["offset"].SetValue(2 * Main.screenPosition / new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["codedColor"].SetValue(Color.White.ToVector4());
            effect.Parameters["uColorOne"].SetValue(new Color(255, 200, 20).ToVector4());
            effect.Parameters["uColorTwo"].SetValue(new Color(255, 150, 10).ToVector4());
            effect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);

            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.0001f);

            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.003f);
            effect.Parameters["power"].SetValue(0.15f);
            effect.Parameters["uOffset"].SetValue(new Vector2(Main.screenPosition.X / Main.screenWidth * 0.5f, 0));
            effect.Parameters["speed"].SetValue(50f);

            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);

            return false;
        }
    }
}
