using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Core
{
    public partial class BombusApisBeePlayer
    {
        public override void Load()
        {
            On.Terraria.Main.DrawInfernoRings += DrawShield;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawInfernoRings -= DrawShield;
        }

        private void DrawShield(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].outOfRange && !Main.player[i].dead)
                {
                    if (Main.player[i].Bombus().LihzardRelicTimer > 0)
                    {
                        var modPlayer = Main.player[i].Bombus();
                        Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BloomFlare").Value;
                        Texture2D texBloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
                        Vector2 pos = new Vector2(Main.player[i].Center.X, Main.player[i].Center.Y + Main.player[i].gfxOffY) - Main.screenPosition;

                        Main.spriteBatch.Draw(tex, pos, null, new Color(223, 170, 40, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.025f, tex.Size() / 2f, MathHelper.Lerp(1.25f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(tex, pos, null, new Color(245, 245, 149, 0) * 0.5f, modPlayer.LihzardRelicTimer * -0.0265f, tex.Size() / 2f, MathHelper.Lerp(1.1f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(tex, pos, null, new Color(245, 245, 149, 0) * 0.55f, modPlayer.LihzardRelicTimer * 0.0165f, tex.Size() / 2f, MathHelper.Lerp(0.9f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(texBloom, pos, null, new Color(223, 170, 40, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.025f, tex.Size() / 2f, MathHelper.Lerp(1.25f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);

                        Main.spriteBatch.Draw(texBloom, pos, null, new Color(245, 245, 149, 0) * 0.5f, modPlayer.LihzardRelicTimer * 0.0265f, tex.Size() / 2f, MathHelper.Lerp(1.1f, 0.05f, 1f - modPlayer.LihzardRelicTimer / 480f), 0, 0f);
                    }
                }
            }
            orig.Invoke(self);
        }
    }
}
