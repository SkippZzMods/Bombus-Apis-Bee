using BombusApisBee.Core.ScreenTargetSystem;
using Terraria;

namespace BombusApisBee.Buffs
{
    public class Frostbroken : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostbroken");
            Description.SetDefault("Brr!.. Part two!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FrostbrokenGlobalNPC>().inflicted = true;
        }
    }

    class FrostbrokenGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
                modifiers.Defense.Base *= 0.65f;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
                modifiers.Defense.Base *= 0.65f;
        }

        public override void AI(NPC npc)
        {
            if (!inflicted)
                return;

            int whoAmI = npc.whoAmI;
            if (npc.realLife >= 0)
                whoAmI = npc.realLife;

            NPC n = Main.npc[whoAmI];

            float mult = 0.3f;
            if (n.boss)
                mult = 0.15f;

            if (n.life <= n.lifeMax * mult)
            {
                Explode(n);

                n.life = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    n.StrikeNPC(new NPC.HitInfo() { InstantKill = true, Knockback = 0f, HitDirection = 0 }, false, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, whoAmI, 9999f);
                }
            }

            if (Main.rand.NextBool(20))
            {
                float lerper = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f);
                Color color = Color.Lerp(new Color(75, 150, 200), new Color(100, 170, 200), lerper);
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(.5f, .5f), 0, color, 0.75f);
            }

            float scale;
            if (npc.width > npc.height)
                scale = npc.width / 15f;
            else if (npc.height > npc.width)
                scale = npc.height / 15f;
            else
                scale = npc.width / 15f;

            if (scale < 3f)
                scale = 3f;

            Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), ModContent.DustType<Dusts.Gas>(), Main.rand.NextVector2Circular(0.25f, 0.25f), newColor: new Color(255, 255, 255)).scale = Main.rand.NextFloat(scale, scale * 1.5f);

            Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), ModContent.DustType<Dusts.Gas>(), Main.rand.NextVector2Circular(0.5f, 0.5f), newColor: new Color(100, 150, 255)).scale = Main.rand.NextFloat(scale, scale * 1.5f);
        }

        private void Explode(NPC npc)
        {
            Vector2 center = npc.Center;

            if (Main.LocalPlayer.Distance(center) < 2000)
            {
                Main.LocalPlayer.Bombus().AddShake(10);
            }

            float scale;
            if (npc.width > npc.height)
                scale = npc.width / 15f;
            else if (npc.height > npc.width)
                scale = npc.height / 15f;
            else
                scale = npc.width / 15f;

            if (scale < 3f)
                scale = 3f;

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), ModContent.DustType<Dusts.Gas>(), Main.rand.NextVector2Circular(5.25f, 5.25f), newColor: new Color(255, 255, 255)).scale = Main.rand.NextFloat(scale, scale * 1.5f);

                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), ModContent.DustType<Dusts.Gas>(), Main.rand.NextVector2Circular(5.5f, 5.5f), newColor: new Color(100, 150, 255)).scale = Main.rand.NextFloat(scale, scale * 1.5f);
            }

            SoundID.DD2_WitherBeastDeath.PlayWith(npc.Center, pitch: 0.35f);

            SoundEngine.PlaySound(SoundID.Item27, npc.position);

            foreach (NPC n in Main.npc)
            {
                int whoAmI = n.whoAmI;
                if (npc.realLife >= 0)
                    whoAmI = n.realLife;

                NPC realNPC = Main.npc[whoAmI];

                if (realNPC.Distance(center) < 250f && realNPC.CanBeChasedBy())
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        realNPC.SimpleStrikeNPC(75, realNPC.Center.X > center.X ? 1 : -1, Main.rand.NextBool(10), 2f, null, true);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, whoAmI, 50f);
                    }

                    realNPC.AddBuff(BuffID.Frostburn2, 600);

                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(realNPC.Center, DustID.Frost, (center.DirectionTo(realNPC.Center).RotatedByRandom(0.3f) * 15f) * Main.rand.NextFloat(0.1f, 1f), Scale: Main.rand.NextFloat(0.5f, 1.75f)).noGravity = true;

                        float lerper = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f);
                        Color color = Color.Lerp(new Color(75, 150, 200), new Color(100, 170, 200), lerper);
                        Dust.NewDustPerfect(realNPC.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), (center.DirectionTo(realNPC.Center).RotatedByRandom(0.3f) * 15f) * Main.rand.NextFloat(0.1f, 1f), 0, color, Main.rand.NextFloat(0.25f, .6f)).noGravity = true;
                    }
                }
            }
        }

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (inflicted)
            {
                float bright = Lighting.Brightness((int)npc.Center.X / 16, (int)npc.Center.Y / 16);

                Main.instance.DrawHealthBar((int)position.X, (int)position.Y, npc.life, npc.lifeMax, bright, scale);

                float life = npc.lifeMax * 0.3f;
                if (npc.boss)
                    life = npc.lifeMax * 0.15f;

                Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/Items/Accessories/BeeKeeperDamageClass/FrozenStinger_Bar").Value;

                float factor = Math.Min(life / (float)npc.lifeMax, 1);

                var source = new Rectangle(0, 0, (int)(factor * tex.Width), tex.Height);
                var target = new Rectangle((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y), (int)(factor * tex.Width * scale), (int)(tex.Height * scale));

                Main.spriteBatch.Draw(tex, target, source, Color.White * bright * 0.75f, 0, new Vector2(tex.Width / 2, 0), 0, 0);

                return false;
            }

            return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (inflicted)
            {
                float scale;
                if (npc.width > npc.height)
                    scale = npc.width / 40f;
                else if (npc.height > npc.width)
                    scale = npc.height / 40f;
                else
                    scale = npc.width / 40f;

                if (scale < 0.5f)
                    scale = 0.5f;


                Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
                spriteBatch.Draw(bloomTex, npc.Center - screenPos, null, new Color(100, 200, 255, 0), npc.rotation, bloomTex.Size() / 2f, scale, 0f, 0f);
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }

    class FrostbrokenNPCDrawer : ILoadable
    {
        public static ScreenTarget target = new(DrawNPCTarget, () => Main.npc.Any(n => n.active && n.HasBuff<Frostbroken>()), 1);
        public void Load(Mod mod)
        {
            Terraria.On_Main.DrawNPCs += DrawTarget;
        }

        public void Unload()
        {
            Terraria.On_Main.DrawNPCs -= DrawTarget;
        }

        private static void DrawNPCTarget(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC NPC = Main.npc[i];

                if (NPC.active && NPC.HasBuff<Frostbroken>())
                {
                    if (NPC.ModNPC != null)
                    {
                        ModNPC ModNPC = NPC.ModNPC;

                        if (ModNPC.PreDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White)))
                            Main.instance.DrawNPC(i, false);

                        ModNPC.PostDraw(spriteBatch, Main.screenPosition, NPC.GetAlpha(Color.White));
                    }
                    else
                    {
                        Main.instance.DrawNPC(i, false);
                    }
                }
            }
        }

        private static void DrawTarget(Terraria.On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            if (!behindTiles && Main.npc.Any(n => n.active && n.HasBuff<Frostbroken>()))
            {
                GraphicsDevice gD = Main.graphics.GraphicsDevice;
                SpriteBatch spriteBatch = Main.spriteBatch;

                if (Main.dedServ || spriteBatch == null || target == null || gD == null)
                    return;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

                Effect effect = Filters.Scene["FrostbrokenShader"].GetShader().Shader;
                effect.Parameters["uImageSize0"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["alpha"].SetValue(1f);

                float lerper = Math.Clamp((float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + 1f), 0, 1f);

                Color color = Color.Lerp(new Color(75, 150, 200, 50), new Color(100, 170, 200, 100), lerper);

                effect.Parameters["colorOne"].SetValue(new Color(100, 180, 255, 100).ToVector4());
                effect.Parameters["colorTwo"].SetValue(new Color(180, 255, 255, 150).ToVector4());
                effect.Parameters["colorThree"].SetValue(new Color(0, 200, 225).ToVector4());

                effect.Parameters["noiseColor"].SetValue(color.ToVector4());

                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/ShaderNoiseLooping").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/Cracks").Value);

                effect.Parameters["noiseScale"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 1000);
                effect.Parameters["noiseScale2"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 500);

                Vector2 offset = 4f * Main.screenPosition / new Vector2(Main.screenWidth, Main.screenHeight);

                effect.Parameters["offset"].SetValue(offset + new Vector2((float)(Main.timeForVisualEffects * 0.0005f), 0f));

                effect.Parameters["offset2"].SetValue(offset + new Vector2((float)(Main.timeForVisualEffects * -0.00075f), 0f));

                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(target.RenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
