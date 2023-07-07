using System.Reflection;
namespace BombusApisBee.Core
{
    public static class BombusApisBee_DoDetours
    {
        public static void Load()
        {
            On_Main.DrawInfernoRings += PlayerDraw;
            On_Player.beeType += EditStrongBeeChance;
        }
        // i would IL this but im too lazy
        private static int EditStrongBeeChance(On_Player.orig_beeType orig, Player self)
        {
            if (self.Hymenoptra().BeeStrengthenChance > 0f && Main.rand.NextFloat() < self.Hymenoptra().BeeStrengthenChance)
            {
                typeof(Player).GetField("makeStrongBee", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, true);       
                return ProjectileID.GiantBee;
            }

            typeof(Player).GetField("makeStrongBee", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(self, false);
            return ProjectileID.Bee;
        }

        public static void Unload()
        {
            On_Main.DrawInfernoRings -= PlayerDraw;
            On_Player.beeType -= EditStrongBeeChance;
        }

        private static void PlayerDraw(Terraria.On_Main.orig_DrawInfernoRings orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.outOfRange && !player.dead)
                {
                    if (player.Bombus().LihzardRelicTimer > 0)
                    {
                        var modPlayer = player.Bombus();
                        Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/BloomFlare").Value;
                        Texture2D texBloom = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
                        Vector2 pos = new Vector2(player.Center.X, player.Center.Y + player.gfxOffY) - Main.screenPosition;

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
