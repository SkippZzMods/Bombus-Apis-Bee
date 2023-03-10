using BombusApisBee.Core.ScreenTargetSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.Graphics.Effects;

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

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (inflicted)
                damage = (int)Main.CalculateDamageNPCsTake(damage, (int)(npc.defense * 0.65f));
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (inflicted)
                damage = (int)Main.CalculateDamageNPCsTake(damage, (int)(npc.defense * 0.65f));
        }

        public override void AI(NPC npc)
        {
            if (!inflicted)
                return;

            int whoAmI = npc.whoAmI;
            if (npc.realLife >= 0)
                whoAmI = npc.realLife;

            NPC n = Main.npc[whoAmI];

            float mult = 0.2f;
            if (n.boss)
                mult = 0.05f;

            if (n.life <= n.lifeMax * mult)
            {
                n.life = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.StrikeNPCNoInteraction(9999, 0, 0);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, whoAmI, 9999f);
                }
            }
        }
    }

    class FrostbrokenNPCDrawer : ILoadable
    {
        public static ScreenTarget target = new(DrawNPCTarget, () => Main.npc.Any(n => n.active && n.HasBuff<Frostbroken>()), 1);
        public void Load(Mod mod)
        {
            On.Terraria.Main.DrawNPCs += DrawTarget;
        }

        public void Unload()
        {
            On.Terraria.Main.DrawNPCs -= DrawTarget;
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

        private static void DrawTarget(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behindTiles)
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

                effect.Parameters["colorOne"].SetValue(new Color(0, 170, 225, 75).ToVector4());
                effect.Parameters["colorTwo"].SetValue(new Color(180, 255, 255, 100).ToVector4());
                effect.Parameters["colorThree"].SetValue(new Color(0, 200, 225).ToVector4());

                effect.Parameters["noiseColor"].SetValue(new Color(150, 255, 255, 100).ToVector4());

                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/ShaderNoiseLooping").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/Cracks").Value);

                effect.Parameters["noiseScale"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 2000);
                effect.Parameters["noiseScale2"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / 5000);

                effect.Parameters["uTime"].SetValue((float)(Main.timeForVisualEffects * 0.0001f));

                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(target.RenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
