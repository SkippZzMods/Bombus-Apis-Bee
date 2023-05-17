using BombusApisBee.BeeDamageClass;
using BombusApisBee.Buffs;
using BombusApisBee.Items.Armor.BeeKeeperDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BombusApisBee.Projectiles
{
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

        public NPC attackTarget;
        public Projectile projectileTarget;

        public Vector2 IdlePos;

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
            if (Owner.Bombus().HoneyBee && Owner.active)
                Projectile.timeLeft = 2;

            if (Projectile.Distance(IdlePos) > 15f)
                Projectile.spriteDirection = Projectile.direction;
            else
                Projectile.direction = Owner.direction;

            if (++Projectile.frameCounter % 8 == 0)
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

            IdlePos = Owner.Center + new Vector2(95 * Owner.direction, MathHelper.Lerp(-20f, -25f, Utils.Clamp((float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 2f), 0, 1)));

            if (Defense)
            {
                if (blockDelay > 0)
                    blockDelay--;

                if (shieldFade < 15)
                    shieldFade++;

                if (blockDelay <= 0 && projectileTarget == null)
                    projectileTarget = Main.projectile.Where(p => p.active && p.hostile && p.Distance(Owner.Center) < 150f && Owner.GetModPlayer<HolyCrusaderPlayer>().hitProjHitTimer[p.whoAmI] <= 0).OrderBy(p => p.Distance(Owner.Center)).FirstOrDefault();

                if (projectileTarget != null && blockDelay <= 0)
                {
                    if (Owner.Distance(projectileTarget.Center) > 150f || !projectileTarget.active || Owner.GetModPlayer<HolyCrusaderPlayer>().hitProjHitTimer[projectileTarget.whoAmI] > 0)
                    {
                        projectileTarget = null;
                        Projectile.velocity *= 0.25f;
                        return;
                    }

                    if (Projectile.Distance(projectileTarget.Center) < 50f && blockDelay <= 0)
                    {
                        projectileTarget.Kill();
                        Projectile.velocity *= 0.55f;
                        blockDelay = 60;
                        return;
                    }

                    Vector2 between = projectileTarget.Center - Projectile.Center;

                    float dist = Vector2.Distance(Projectile.Center, projectileTarget.Center);

                    float speed = 30f;
                    float adjust = 1f;

                    dist = speed / dist;
                    between.X *= dist;
                    between.Y *= dist;

                    if (Projectile.velocity.X < between.X)
                    {
                        Projectile.velocity.X += adjust;
                        if (Projectile.velocity.X < 0f && between.X > 0f)
                        {
                            Projectile.velocity.X += adjust * 2f;
                        }
                    }
                    else if (Projectile.velocity.X > between.X)
                    {
                        Projectile.velocity.X -= adjust;
                        if (Projectile.velocity.X > 0f && between.X < 0f)
                        {
                            Projectile.velocity.X -= adjust * 2f;
                        }
                    }
                    if (Projectile.velocity.Y < between.Y)
                    {
                        Projectile.velocity.Y += adjust;
                        if (Projectile.velocity.Y < 0f && between.Y > 0f)
                        {
                            Projectile.velocity.Y += adjust * 2f;
                        }
                    }
                    else if (Projectile.velocity.Y > between.Y)
                    {
                        Projectile.velocity.Y -= adjust;
                        if (Projectile.velocity.Y > 0f && between.Y < 0f)
                        {
                            Projectile.velocity.Y -= adjust * 2f;
                        }
                    }
                }
                else
                {
                    IdleMovement();
                }
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
            }
            else if (Gathering)
            {
                if (shieldFade > 0)
                    shieldFade--;

                IdleMovement();
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

            if (shieldFade > 0)
            {
                float mult = (1f - blockDelay / 60f) * (shieldFade / 15f) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

                Rectangle glowFrame = texGlow.Frame(verticalFrames: 4, frameY: Projectile.frame);
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, new Color(250, 170, 20, 0) * mult, Projectile.rotation, glowFrame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            if (shieldFade > 0)
            {
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["HoneyShieldShader"].GetShader().Shader;
                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
                effect.Parameters["blowUpPower"].SetValue(3f);
                effect.Parameters["blowUpSize"].SetValue(1f);

                float mult = (1f - blockDelay / 60f) * (shieldFade / 15f) * (Owner.Hymenoptra().HoldingBeeWeaponTimer / 15f);

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
            }

            return false;
        }
    }
}
