using BombusApisBee.Buffs;
using BombusApisBee.PrimitiveDrawing;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;

namespace BombusApisBee.Projectiles
{
    enum QueenAttackState : int
    {
        StingerRapid = 0,
        Bees = 1,
        Dash = 2,
        StingerSpread = 3,
        HoneyBeam = 4,
    }

    public class TheQueen : BeeProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        internal QueenAttackState attackState = QueenAttackState.StingerRapid;
        public Vector2 laserEnd => Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) + Vector2.One.RotatedBy(laserRot - MathHelper.PiOver4) * 750f;
        public float laserRot;
        public float oldLaserRot;
        public override string Texture => "Terraria/Images/NPC_" + NPCID.QueenBee;
        public int dashDir = 1;
        public ref float switchTimer => ref Projectile.ai[1];
        public ref float attackTimer => ref Projectile.ai[0];   
        public Player owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Queen");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 66;

            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.timeLeft = 7200;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.frame = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            if (!owner.HasBuff<TheQueensGuard>())
                Projectile.Kill();

            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(owner.Center) < 1500f).OrderBy(n => Vector2.Distance(n.Center, owner.Center)).FirstOrDefault();
            if (target != default)
            {
                switch (attackState)
                {
                    case QueenAttackState.StingerRapid:
                        StingerRapidAI(target);
                        break;

                    case QueenAttackState.Bees:
                        BeesAI(target);
                        break;

                    case QueenAttackState.Dash:
                        DashAI(target);
                        break;

                    case QueenAttackState.StingerSpread:
                        StingerSpreadAI(target);
                        break;

                    case QueenAttackState.HoneyBeam:
                        HoneyBeamAI(target);
                        break;
                }
            }
            else
            {
                Animate();

                attackTimer = 0;
                Projectile.rotation = 0f;

                Vector2 idlePos = owner.Center + new Vector2(160 * owner.direction, -100);
                Vector2 toIdlePos = idlePos - Projectile.Center;
                if (toIdlePos.Length() < 0.0001f)
                    toIdlePos = Vector2.Zero;

                float distance = toIdlePos.Length();
                float speed = Vector2.Distance(Projectile.Center, idlePos) * 0.035f;
                speed = Utils.Clamp(speed, 1f, 30f);
                float inertia = Vector2.Distance(Projectile.Center, idlePos) * 0.05f;
                inertia = Utils.Clamp(inertia, 5f, 65f);

                if (distance > 25f)
                {
                    toIdlePos.Normalize();
                    toIdlePos *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.25f;
                    Projectile.velocity.Y = -0.1f;
                }

                if (Main.myPlayer == owner.whoAmI && distance > 2500f)
                {
                    Projectile.position = idlePos;
                    Projectile.velocity *= 0.1f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Projectile.direction, -2f, 0, default, 1f);
            }
            Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y - 35f), Projectile.velocity, 303, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y - 45f), Projectile.velocity, 304, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.velocity, 305, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y + 20f), Projectile.velocity, 306, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y + 10f), Projectile.velocity, 307, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(),new Vector2(Projectile.position.X, Projectile.position.Y - 10f), Projectile.velocity, 308, Projectile.scale);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (attackState == QueenAttackState.HoneyBeam && attackTimer >= 122)
            {
                float useless = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), laserEnd, 15, ref useless);
            }

            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : 0;
            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRect.Size() / 2f + new Vector2(0, 35);

            if ((attackState == QueenAttackState.Dash || attackState == QueenAttackState.StingerSpread) && attackTimer > 0)
            {
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + (Projectile.Size / 2f) + new Vector2(0f, Projectile.gfxOffY);
                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * MathHelper.Lerp(1f, 0f, 1f - attackTimer / 40f);
                    Main.EntitySpriteDraw(tex, drawPos, sourceRect, color, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
                }
            }

            if (attackState == QueenAttackState.HoneyBeam)
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : 0;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, sourceRect, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);

            DrawTrail(Main.spriteBatch);
            if (attackState == QueenAttackState.HoneyBeam && attackTimer >= 122)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) - Main.screenPosition, null, new Color(141, 72, 0, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 1.15f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) - Main.screenPosition, null, new Color(233, 194, 28, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 1.05f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) - Main.screenPosition, null, new Color(254, 255, 204, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 0.95f, 0f, 0f);

                Vector2 pos = Projectile.Center - Projectile.velocity + new Vector2(15 * Projectile.spriteDirection, 15) + Vector2.One.RotatedBy(oldLaserRot - MathHelper.PiOver4) * 750f;
                Main.spriteBatch.Draw(bloomTex, pos - Main.screenPosition, null, new Color(141, 72, 0, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 1.15f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, pos - Main.screenPosition, null, new Color(233, 194, 28, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 1.05f, 0f, 0f);

                Main.spriteBatch.Draw(bloomTex, pos - Main.screenPosition, null, new Color(254, 255, 204, 0) * TrailFade(), 0f, bloomTex.Size() / 2f, 0.95f, 0f, 0f);
            }
            return false;
        }

        private void StingerRapidAI(NPC target)
        {
            Animate();

            Projectile.velocity = (Projectile.velocity * 20f + Utils.SafeNormalize(Projectile.Center.DirectionTo(target.Center + (++switchTimer / 16f).ToRotationVector2() * 200f), Vector2.UnitX) * 19f) / 21f;

            if (++attackTimer % 11 == 0)
                if (Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1))
                {
                    Vector2 pos = Projectile.Center + new Vector2(15 * Projectile.direction, 15) + Main.rand.NextVector2Circular(25f, 15f);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, pos.DirectionTo(target.Center) * 20f, ModContent.ProjectileType<StingerFriendly>(), Projectile.damage, 4.5f, Projectile.owner);
                    for (int i = 0; i < 45; ++i)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)45;
                        Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.StingerDust>(), Utils.ToRotationVector2(angle2) * 3.5f, Main.rand.Next(50, 150), default, 1.15f).noGravity = true;
                    }

                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.StingerDust>(), pos.DirectionTo(target.Center).RotatedByRandom(0.35f) * Main.rand.NextFloat(6.5f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;

                        Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.StingerDust>(), Main.rand.NextVector2Circular(8.5f, 8.5f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;
                    }
                }

            if (switchTimer >= 480)
            {
                attackState++;
                switchTimer = 0;
                attackTimer = 0;
            }
        }

        private void BeesAI(NPC target)
        {
            Animate();

            Vector2 targetPos = target.Center + Vector2.UnitY * -250f;
            Vector2 toIdlePos = targetPos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed = Vector2.Distance(Projectile.Center, targetPos) * 0.035f;
            speed = Utils.Clamp(speed, 1f, 35f);
            float inertia = Vector2.Distance(Projectile.Center, targetPos) * 0.05f;
            inertia = Utils.Clamp(inertia, 5f, 65f);

            if (distance > 25f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.25f;
                Projectile.velocity.Y = -0.1f;
            }

            if (++attackTimer % 8 == 0)
            {
                Vector2 pos = Projectile.Center + new Vector2(15 * Projectile.direction, 15) + Main.rand.NextVector2Circular(25f, 15f);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, pos.DirectionTo(target.Center).RotatedByRandom(0.45f) * 6f, Main.rand.Next(new int[] { ProjectileID.Bee, ProjectileID.GiantBee, ProjectileID.Wasp }), (int)(Projectile.damage * 0.65f), 0.5f, Projectile.owner);
                BombusApisBee.HoneycombWeapon.PlayWith(pos);

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDust>(), Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(150, 200), default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(pos, DustID.Honey2, Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDust>(), pos.DirectionTo(target.Center).RotatedByRandom(0.45f) * Main.rand.NextFloat(3f), Main.rand.Next(150, 200), default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(pos, DustID.Honey2, pos.DirectionTo(target.Center).RotatedByRandom(0.45f) * Main.rand.NextFloat(2f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;
                }
            }
                    

            if (++switchTimer >= 480)
            {
                attackState++;
                switchTimer = 0;
                attackTimer = 0;
            }
        }

        private void DashAI(NPC target)
        {
            if (attackTimer > 0)
                attackTimer--;

            if (attackTimer > 0)
            {
                Animate(true);
                Projectile.velocity *= 0.99f;
                return;
            }

            Animate();

            Vector2 targetPos = target.Center + Vector2.UnitX * 200f * dashDir;
            Vector2 toIdlePos = targetPos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed = Vector2.Distance(Projectile.Center, targetPos) * 0.055f;
            speed = Utils.Clamp(speed, 20f, 35f);
            float inertia = Vector2.Distance(Projectile.Center, targetPos) * 0.05f;
            inertia = Utils.Clamp(inertia, 25f, 65f);

            if (distance > 55f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }
            else
            {
                attackTimer = 40;
                owner.Bombus().shakeTimer += 8;
                SoundID.Roar.PlayWith(Projectile.Center);
                Projectile.velocity = Projectile.DirectionTo(target.Center) * 25f;
                if (Main.myPlayer == Projectile.owner)
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Circular(15f, 15f), ModContent.ProjectileType<HoneyHoming>(), Projectile.damage, 5f, Projectile.owner);
                    }

                for (int i = 0; i < 65; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)65;
                    Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Utils.ToRotationVector2(angle2) * 9.5f, 0, default, 1.85f).noGravity = true;
                }

                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDust>(), Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(100, 200), default, 1.45f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Honey2, Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(50, 150), default, 1.45f).noGravity = true;
                }

                dashDir *= -1;
                switchTimer++;
            }

            if (switchTimer >= 6 && attackTimer <= 0)
            {
                attackState++;
                switchTimer = 0;
                attackTimer = 0;
            }
        }

        private void StingerSpreadAI(NPC target)
        {
            if (attackTimer > 0)
                attackTimer--;

            if (attackTimer > 0)
            {
                Animate(true);
                if (attackTimer < 25)
                    Projectile.velocity *= 0.985f;

                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == -1 ? MathHelper.Pi : 0);
                return;
            }

            Animate();
            Vector2 targetPos = target.Center + (dashDir == 1 ? new Vector2(200, -200) : new Vector2(-200, 200));
            Vector2 toIdlePos = targetPos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed = Vector2.Distance(Projectile.Center, targetPos) * 0.055f;
            speed = Utils.Clamp(speed, 20f, 35f);
            float inertia = Vector2.Distance(Projectile.Center, targetPos) * 0.05f;
            inertia = Utils.Clamp(inertia, 25f, 65f);

            if (distance > 55f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }
            else
            {
                attackTimer = 40;
                owner.Bombus().shakeTimer += 8;
                SoundID.Roar.PlayWith(Projectile.Center);
                Projectile.velocity = Projectile.DirectionTo(target.Center) * 20f;
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        float angle2 = 6.2831855f * (float)i / (float)8;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Utils.ToRotationVector2(angle2) * 10f, ModContent.ProjectileType<HomingStinger>(), Projectile.damage * 2 / 3, 3f, owner.whoAmI);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        float radians = MathHelper.Lerp(-0.3f, 0.3f, i / (float)6);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center).RotatedBy(radians) * 15f, ModContent.ProjectileType<StingerFriendly>(), Projectile.damage * 2 / 3, 4f, owner.whoAmI);
                    }
                }

                for (int i = 0; i < 45; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.StingerDust>(), Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(100, 200), default, 1.45f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned, Main.rand.NextVector2Circular(7.5f, 7.5f), Main.rand.Next(50, 150), default, 1.45f).noGravity = true;
                }

                dashDir *= -1;
                switchTimer++;
            }

            Projectile.rotation = 0f;

            if (switchTimer >= 6 && attackTimer <= 0)
            {
                attackState++;
                switchTimer = 0;
                attackTimer = 0;
                Projectile.rotation = 0f;
            }
        }

        private void HoneyBeamAI(NPC target)
        {
            Animate();
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.spriteDirection = Projectile.Center.X < target.Center.X ? 1 : -1;
            Vector2 targetPos = target.Center + Vector2.UnitY * -150f + Vector2.UnitX * -100 * target.direction;
            Vector2 toIdlePos = targetPos - Projectile.Center;
            if (toIdlePos.Length() < 0.0001f)
                toIdlePos = Vector2.Zero;

            float distance = toIdlePos.Length();
            float speed = Vector2.Distance(Projectile.Center, targetPos) * 0.045f;
            speed = Utils.Clamp(speed, 1f, 35f);
            float inertia = Vector2.Distance(Projectile.Center, targetPos) * 0.05f;
            inertia = Utils.Clamp(inertia, 5f, 65f);

            if (distance > 25f)
            {
                toIdlePos.Normalize();
                toIdlePos *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + toIdlePos) / inertia;
            }

            if (++attackTimer < 120)
            {
                for (int i = 0; i < 3; i++)
                {
                    float lerper = MathHelper.Lerp(150f, 10f, attackTimer / 120f);
                    Vector2 pos = Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) + Main.rand.NextVector2CircularEdge(lerper, lerper);
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.Glow>(), pos.DirectionTo(Projectile.Center), 0, new Color(214, 178, 36), 0.45f);
                }

                if (attackTimer % 10 == 0)
                    owner.Bombus().shakeTimer += (int)MathHelper.Lerp(1, 9, attackTimer / 120f);
            }
            else
            {
                if (switchTimer == 0)
                {
                    SoundID.Item74.PlayWith(Projectile.Center, -0.15f, 0.25f, 1.35f);
                    owner.Bombus().shakeTimer += 20;
                    laserRot = Projectile.DirectionTo(target.Center).ToRotation();
                }

                oldLaserRot = laserRot;
                laserRot = MathHelper.Lerp(oldLaserRot, Math.Abs(Projectile.DirectionTo(target.Center).ToRotation()), 0.15f);

                Projectile.velocity -= Vector2.One.RotatedBy(laserRot - MathHelper.PiOver4) * MathHelper.Lerp(0.65f, 0.1f, switchTimer / 240f);

                Vector2 lerpPos = Vector2.Lerp(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), laserEnd, Main.rand.NextFloat());
                Dust.NewDustPerfect(lerpPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(214, 178, 36) * TrailFade(), 0.45f);
                lerpPos = Vector2.Lerp(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), laserEnd, Main.rand.NextFloat());
                Dust.NewDustPerfect(lerpPos, ModContent.DustType<Dusts.Glow>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(254, 255, 204) * TrailFade(), 0.4f);

                Dust.NewDustPerfect(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(254, 255, 204) * TrailFade(), 0.4f);

                Dust.NewDustPerfect(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5f, 5f), 0, new Color(214, 178, 36) * TrailFade(), 0.4f);

                Dust.NewDustPerfect(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), ModContent.DustType<Dusts.Glow>(),
                    (Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15)).DirectionTo(laserEnd).RotatedByRandom(0.3f) * Main.rand.NextFloat(8f), 0, new Color(214, 178, 36) * TrailFade(), 0.65f);

                Dust.NewDustPerfect(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15), ModContent.DustType<Dusts.Glow>(),
                    (Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15)).DirectionTo(laserEnd).RotatedByRandom(0.3f) * Main.rand.NextFloat(12f), 0, new Color(254, 255, 204) * TrailFade(), 0.55f);

                if (switchTimer % 7 == 0)
                    SoundID.Item60.PlayWith(Projectile.Center, 0.25f, 0.15f, 1.15f);

                if (switchTimer % 15 == 0)
                    owner.Bombus().shakeTimer += (int)MathHelper.Lerp(12, 2, switchTimer / 240f);

                if (++switchTimer >= 240)
                {
                    attackState = 0;
                    switchTimer = 0;
                    attackTimer = 0;
                }
            }
        }

        private void ManageCaches()
        {
            cache = new List<Vector2>();
            for (int i = 0; i < 20; i++)
            {
                cache.Add(Vector2.Lerp(Projectile.Center + new Vector2(15 * Projectile.spriteDirection, 15) + Projectile.velocity, laserEnd, i / 20f));
            }
            cache.Add(laserEnd);
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 21, new TriangularTip(12), factor => 20f, factor =>
            {
                return new Color(141, 72, 0) * TrailFade();
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = laserEnd;

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 21, new TriangularTip(12), factor => 10f, factor =>
            {
                return new Color(254, 255, 204) * TrailFade();
            });

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = laserEnd;
        }

        private void DrawTrail(SpriteBatch spriteBatch)
        {
            if (attackState == QueenAttackState.HoneyBeam && attackTimer >= 122)
            {
                spriteBatch.End();
                Effect effect = Terraria.Graphics.Effects.Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * -0.03f);
                effect.Parameters["repeats"].SetValue(1);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
                trail?.Render(effect);

                trail2?.Render(effect);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);
                trail?.Render(effect);

                trail2?.Render(effect);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.02f);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);
                trail?.Render(effect);

                trail2?.Render(effect);
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        private float TrailFade()
        {
            if (switchTimer < 205f)
                return 1f;

            float fade = MathHelper.Lerp(1f, 0f, (switchTimer - 205f) / 35f);
            return fade;
        }

        private void Animate(bool dashing = false)
        {
            if (dashing)
            {
                if (++Projectile.frameCounter % 4 == 0)
                {
                    if (++Projectile.frame >= 4)
                        Projectile.frame = 0;
                }
            }
            else
            {
                if (++Projectile.frameCounter % 4 == 0)
                {
                    if (Projectile.frame < 4)
                        Projectile.frame = 4;

                    Projectile.frame++;
                    if (Projectile.frame >= 12)
                        Projectile.frame = 4;
                }
            }
        }
    }
}











