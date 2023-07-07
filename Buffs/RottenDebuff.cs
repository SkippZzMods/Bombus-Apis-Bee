using BombusApisBee.Core.ScreenTargetSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Buffs
{
    public class RottenDebuff : ModBuff
    {
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten");
            Description.SetDefault("u smell gross");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<RottenGlobalNPC>().inflicted = true;
            npc.GetGlobalNPC<RottenGlobalNPC>().timer = npc.buffTime[buffIndex];
        }
    }

    class RottenGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;
        public int timer;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
            {
                modifiers.FlatBonusDamage += 4;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
            {
                modifiers.FlatBonusDamage += 4;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (inflicted)
            {
                modifiers.SourceDamage *= 0.9f;
            }
        }

        public override void AI(NPC npc)
        {
            if (inflicted && Main.rand.NextBool(20))
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height), DustID.Poisoned, -Vector2.UnitY * 2f, 200, default, 2f).noGravity = true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

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

                float fadeOut = 1f;
                if (timer < 30f)
                    fadeOut = timer / 30f;

                Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
                effect.Parameters["screenPos"].SetValue(Main.screenPosition * new Vector2(0.5f, 0.1f) / new Vector2(Main.screenWidth, Main.screenHeight));

                effect.Parameters["offset"].SetValue(new Vector2(0.002f));
                effect.Parameters["repeats"].SetValue(1);
                effect.Parameters["uImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value);
                effect.Parameters["uImage2"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/MiscNoise1").Value);
                Color color = new Color(70, 150, 60, 0) * fadeOut;

                effect.Parameters["uColor"].SetValue(color.ToVector4());
                effect.Parameters["noiseImage1"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/PerlinNoise").Value);

                effect.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(bloomTex, npc.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale, 0f, 0f);

                spriteBatch.Draw(bloomTex, npc.Center - Main.screenPosition, null, Color.White, 0f, bloomTex.Size() / 2f, scale * 1.1f, 0f, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
