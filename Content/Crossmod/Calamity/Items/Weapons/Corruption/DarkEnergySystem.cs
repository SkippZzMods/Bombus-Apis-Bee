using BombusApisBee;
using BombusApisBee.Content.Dusts;
using BombusApisBee.Content.Dusts.Pixelized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Corruption
{
    [JITWhenModsEnabled("CalamityMod")]
    public class DarkEnergyGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public const int MAXENERGY = 20;

        public int darkEnergy;
        public int cooldown;

        public override bool InstancePerEntity => true;

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D barFrontTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarFront").Value;
            Texture2D barFrontTexGlow = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarFront_Glow").Value;

            Texture2D barBackTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarBack").Value;
            Texture2D barBackTexGlow = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/GenericBarBack_Glow").Value;

            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            if (darkEnergy > 0)
            {
                Vector2 position = npc.Top + new Vector2(0f, -15f);
                float factor = Math.Min(darkEnergy / (float)MAXENERGY, 1);

                Vector2 offset = Main.rand.NextVector2Circular(3f, 3f) * (darkEnergy / (float)MAXENERGY);

                var source = new Rectangle(0, 0, (int)(factor * barFrontTex.Width), barFrontTex.Height);
                var target = new Rectangle((int)(position.X - Main.screenPosition.X) + (int)offset.X,
                    (int)(position.Y - Main.screenPosition.Y) + (int)offset.Y, (int)(factor * barFrontTex.Width), (int)(barFrontTex.Height));

                spriteBatch.Draw(glowTex, position + new Vector2(0f, 6f) - Main.screenPosition, null, new Color(152, 137, 255, 0) * 0.25f, 0, glowTex.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barBackTexGlow, position + new Vector2(0f, 6f) - Main.screenPosition, null, new Color(152, 137, 255, 0), 0, barBackTexGlow.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barBackTex, position + new Vector2(0f, 6f) - Main.screenPosition, null, Color.White, 0, barBackTex.Size() / 2f, 1f, 0, 0);

                spriteBatch.Draw(barFrontTex, target, source, new Color(119, 89, 227), 0, new Vector2(barFrontTex.Width / 2, 0), 0, 0);

                spriteBatch.Draw(barFrontTex, target, source, new Color(152, 137, 255, 0), 0, new Vector2(barFrontTex.Width / 2, 0), 0, 0);
            }
        }

        public override void ResetEffects(NPC npc)
        {
            if (cooldown == 1 )
            {
                darkEnergy--;

                if (darkEnergy <= 0)
                    cooldown = 0;
                else
                    cooldown = 15;
            }
            else
                cooldown--;
        }

        public override void AI(NPC npc)
        {

        }

        public void Explode(NPC npc, Player player)
        {
            player.Bombus().AddShake(10);

            new SoundStyle("BombusApisBee/Sounds/Item/GoreLight").PlayWith(npc.Center, volume: 0.65f);
            new SoundStyle("BombusApisBee/Sounds/ShadowDeath").PlayWith(npc.Center);
            new SoundStyle("BombusApisBee/Sounds/Shadow2").PlayWith(npc.Center);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), ProjectileType<DarksentBee>(), 20, 1f, player.whoAmI);
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(15f);

                Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                Dust.NewDustPerfect(npc.Center, DustType<PixelatedGlowAlt>(),
                    velocity, 0, color, 1f);

                if (Main.rand.NextBool(2))
                {
                    color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                    Dust.NewDustPerfect(npc.Center, DustType<StarDustWhite>(),
                                       velocity, 0, color, 1.2f).customData = true;
                }

                for (int j = 0; j < 2; j++)
                {
                    velocity = -Vector2.UnitY.RotatedByRandom(1.6f) * Main.rand.NextFloat(6f);

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity, Main.rand.Next(90, 220), new Color(119, 89, 227), Main.rand.NextFloat(1f, 1.2f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity, Main.rand.Next(90, 220), new Color(91, 71, 127), Main.rand.NextFloat(1f, 1.4f)).noGravity = true;

                    Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity, Main.rand.Next(90, 220), new Color(52, 42, 81), Main.rand.NextFloat(1f, 1.7f)).noGravity = true;
                }              
            }

            darkEnergy = 0;
        }

        public void Heal(Player player)
        {
            player.AddBuff<ShadestingerScytheCooldown>(60 * 30);

            new SoundStyle("BombusApisBee/Sounds/ShadowDeath").PlayWith(player.Center);
            new SoundStyle("BombusApisBee/Sounds/Shadow2").PlayWith(player.Center, volume: 0.65f);
            SoundID.Item4.PlayWith(player.Center);

            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(5f);

                Color color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                Dust.NewDustPerfect(player.Center, DustType<PixelatedGlowAlt>(),
                    velocity, 0, color, 0.5f);

                if (Main.rand.NextBool(2))
                {
                    color = Main.rand.Next(new Color[] { new Color(152, 137, 255, 0), new Color(119, 89, 227, 0), new Color(210, 228, 255, 0) });

                    Dust.NewDustPerfect(player.Center, DustType<StarDustWhite>(),
                                       velocity, 0, color, 0.6f).customData = true;
                }

                velocity = -Vector2.UnitY * Main.rand.NextFloat(15f);

                Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(player.width, player.height), DustType<PixelImpactLineDustGlow>(),
                    velocity, 0, new Color(10, 255, 50, 0), 0.2f);
            }

            player.Heal(1 + Main.rand.Next(darkEnergy / 3));

            darkEnergy = 0;
        }

        public void AddEnergy(int energyToAdd)
        {
            cooldown = 240;

            if (darkEnergy < MAXENERGY)
                darkEnergy += energyToAdd;

            if (darkEnergy > MAXENERGY)
                darkEnergy = MAXENERGY;
        }
    }
}
