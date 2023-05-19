using BombusApisBee.BeeDamageClass;
using BombusApisBee.Buffs;
using BombusApisBee.Items.Armor.BeeKeeperDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BombusApisBee.Projectiles
{
    class HoneyBeeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool inHitDistance;
    }

    public class HoneyBeeProj : BeeProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public bool Defense => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Defense;
        public bool Offense => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Offense;
        public bool Gathering => Owner.Hymenoptra().CurrentBeeState == (int)BeeDamagePlayer.BeeState.Gathering;

        public int AttackDelay;
        public int maxAttackDelay;

        public int blockDelay;
        public int shieldFade;
        public int shieldFlashTimer;

        public int honeyTimer;

        public NPC attackTarget;
        public Projectile projectileTarget;

        public Vector2 IdlePos;
        public Vector2 BottleVector => Projectile.Center + new Vector2(Projectile.spriteDirection == -1 ? -20 : 15, 15).RotatedBy(Projectile.rotation);

        public override string Texture => "BombusApisBee/Items/Accessories/BeeKeeperDamageClass/HoneyBee";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Bee");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SafeSetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            if (Owner.Hymenoptra().HoldingBeeWeaponTimer <= 0)
                Projectile.Kill();

            if (Owner.Bombus().HoneyBee && Owner.active)
                Projectile.timeLeft = 2;

            if (Projectile.Distance(IdlePos) > 25f)
                Projectile.spriteDirection = Projectile.direction;
            else
                Projectile.spriteDirection = Owner.direction;

            if (++Projectile.frameCounter % 8 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            IdlePos = Owner.Center + new Vector2(115 * Owner.direction, MathHelper.Lerp(-20f, -25f, Utils.Clamp((float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 2f), 0, 1)));

            if (shieldFlashTimer > 0)
                shieldFlashTimer--;

            if (Defense)
            {
                if (blockDelay > 0)
                    blockDelay--;

                if (shieldFade < 15)
                    shieldFade++;

                if (blockDelay <= 0 && projectileTarget == null)
                    projectileTarget = Main.projectile.Where(p => p.active && p.hostile && p.Distance(Owner.Center) < 150f && Owner.GetModPlayer<HolyCrusaderPlayer>().hitProjHitTimer[p.whoAmI] <= 0).OrderBy(p => p.Distance(Owner.Center)).FirstOrDefault();

                if (blockDelay <= 0)
                {
                    if (projectileTarget != null)
                    {
                        BeeUtils.CircleDust(Projectile.Center, 25, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3f, 0, null, 4f);

                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(projectileTarget.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), projectileTarget.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(), 0, new Color(255, 200, 20), 0.4f);
                            Dust.NewDustPerfect(projectileTarget.Center, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Main.rand.NextVector2Circular(2f, 2f), 0, default, 2f).noGravity = true;
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 pos = Vector2.Lerp(Projectile.Center, projectileTarget.Center, i * 0.05f);
                            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Vector2.Zero, 0, default, 2f).noGravity = true;
                        }

                        Projectile.velocity -= Projectile.DirectionTo(projectileTarget.Center) * 3f;


                        shieldFlashTimer = 15;

                        blockDelay = 60;

                        projectileTarget.Kill();
                        projectileTarget = null;
                    }
                    else
                    {
                        NPC target = Main.npc.Where(n => (n.CanBeChasedBy() || (NPCID.Sets.ProjectileNPC[n.type] && n.active)) && n.Distance(Owner.Center) < 150f).OrderBy(n => n.Distance(Owner.Center)).OrderBy(n => !NPCID.Sets.ProjectileNPC[n.type]).FirstOrDefault();

                        if (target != null)
                        {
                            BeeUtils.CircleDust(Projectile.Center, 25, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), 3f, 0, null, 4f);

                            for (int i = 0; i < 5; i++)
                            {
                                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), target.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(), 0, new Color(255, 200, 20), 0.4f);
                                Dust.NewDustPerfect(target.Center, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Main.rand.NextVector2Circular(2f, 2f), 0, default, 2f).noGravity = true;
                            }

                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, i * 0.05f);
                                Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyMetaballDustTransparent>(), Vector2.Zero, 0, default, 2f).noGravity = true;
                            }

                            Projectile.velocity -= Projectile.DirectionTo(target.Center) * 3f;


                            shieldFlashTimer = 15;

                            blockDelay = 60;

                            target.StrikeNPC(100, 10f, target.Center.X < Owner.Center.X ? -1 : 1, true);
                        }
                    }
                }

                IdleMovement();
                honeyTimer = 0;
            }
            else if (Offense)
            {
                if (shieldFade > 0)
                    shieldFade--;

                if (attackTarget != null)
                {

                }
                else
                    IdleMovement();

                honeyTimer = 0;
            }
            else if (Gathering)
            {
                if (shieldFade > 0)
                    shieldFade--;

                IdleMovement();

                if (honeyTimer > 100)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(BottleVector, DustID.Honey2, Projectile.DirectionTo(Owner.Center).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f), Main.rand.Next(50, 200), default, 2f).noGravity = true;
                    }

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), BottleVector,
                        Projectile.DirectionTo(Owner.Center) * 5f, ModContent.ProjectileType<BeeResourceIncreaseProjectile>(), 0, 0f, Owner.whoAmI, 0, 8);

                    honeyTimer = 0;
                }
                else
                    honeyTimer++;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        private void IdleMovement()
        {
            Vector2 toIdlePos = IdlePos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
            {
                toIdlePos = Vector2.Zero;
            }
            else
            {
                float speed = Vector2.Distance(IdlePos, Projectile.Center) * 0.2f;
                speed = Utils.Clamp(speed, 1f, 20f);
                if (Defense)
                    speed = Utils.Clamp(speed, 0.5f, 30f);

                toIdlePos.Normalize();
                toIdlePos *= speed;
            }

            Projectile.velocity = (Projectile.velocity * (25f - 1) + toIdlePos) / 25f;

            if (Vector2.Distance(Projectile.Center, IdlePos) > 2000f)
            {
                Projectile.Center = IdlePos;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;

            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            lightColor = lightColor * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

            float mult = (1f - blockDelay / 60f) * (shieldFade / 15f) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

            if (honeyTimer > 0)
            {
                Main.spriteBatch.Draw(glowTex, BottleVector - new Vector2(Projectile.spriteDirection == -1 ? -20 : 15, 5) - Main.screenPosition, null, (new Color(255, 150, 20, 0) * (honeyTimer / 100f)) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f), 0f, glowTex.Size() / 2f, 0.6f, 0f, 0f);
            }

            if (shieldFade > 0)
            {
                Rectangle glowFrame = texGlow.Frame(verticalFrames: 4, frameY: Projectile.frame);
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, new Color(250, 170, 20, 0) * mult, Projectile.rotation, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 20, 0) * mult, 0f, glowTex.Size() / 2f, 0.8f, 0f, 0f);
            }

            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            if (shieldFlashTimer > 0)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * MathHelper.Lerp(0f, 1f, shieldFlashTimer / 15f), Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(2f, 1f, shieldFlashTimer / 15f), Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            if (shieldFade > 0)
            {
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["HoneyShieldShader"].GetShader().Shader;
                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
                effect.Parameters["blowUpPower"].SetValue(3f);
                effect.Parameters["blowUpSize"].SetValue(1f);

                float noiseScale = MathHelper.Lerp(0.45f, 0.65f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.1f) + 1f);
                effect.Parameters["noiseScale"].SetValue(noiseScale);
                float opacity = 0.35f * mult;
                effect.Parameters["shieldOpacity"].SetValue(opacity);
                effect.Parameters["shieldEdgeColor"].SetValue((new Color(255, 200, 20) * mult).ToVector3());
                effect.Parameters["shieldEdgeBlendStrenght"].SetValue(5f);

                effect.Parameters["shieldColor"].SetValue((new Color(255, 100, 20) * mult).ToVector3());

                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["power"].SetValue(0.15f);
                effect.Parameters["offset"].SetValue(new Vector2(Main.screenPosition.X / Main.screenWidth * 0.5f, 0));
                effect.Parameters["speed"].SetValue(15f);

                Texture2D noise = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/SwirlyNoiseLooping").Value;
                Vector2 pos = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(noise, pos, null, Color.White, 0f, noise.Size() / 2f, 0.16f, 0, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

                Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(255, 200, 20, 0) * mult * 0.5f, 0f, glowTex.Size() / 2f, 1f, 0f, 0f);
            }

            return false;
        }
    }
}
