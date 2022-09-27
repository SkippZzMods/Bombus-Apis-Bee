using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;


namespace BombusApisBee.BeeHelperProj
{
    public abstract class BeeHelper : ModProjectile
    {
        public int BeeAITimer;
        public bool Velocity = true;
        public bool otherGiant;
        public bool Giant => CanBeGiant && Projectile.ai[0] == 1f;

        public virtual int GiantWidth => 16;

        public virtual int GiantHeight => 16;

        public virtual int SmallWidth => 8;

        public virtual int SmallHeight => 8;
        public virtual bool CanBeGiant => true;

        public bool Initialized;
        public virtual int FrameTimer => 3;

        public virtual int GiantBeePenetrate => 3;

        public virtual int RegularBeePenetrate => 2;

        public virtual void SafeAI()
        {
        }
        public override void SetStaticDefaults()
        {
        }
        public virtual void SafeSetDefaults()
        {

        }

        public override void OnSpawn(IEntitySource source)
        {
            if (!CanBeGiant)
                return;

            if (source is EntitySource_ItemUse_WithAmmo { Entity: Player owner, Item: Item item })
            {
                if (owner.strongBees)
                    Projectile.ai[0] = Main.rand.NextBool() ? 1f : 0f;
            }
            else if (source is EntitySource_Death { Entity: Projectile proj })
            {
                if (Main.player[proj.owner].strongBees)
                    Projectile.ai[0] = Main.rand.NextBool() ? 1f : 0f;
            }
            else if (source is EntitySource_Parent { Entity: Projectile projectile })
            {
                if (Main.player[projectile.owner].strongBees)
                    Projectile.ai[0] = Main.rand.NextBool() ? 1f : 0f;
            }
            else if (source is EntitySource_OnHit { EntityStriking: Projectile proj2 })
            {
                if (Main.player[proj2.owner].strongBees)
                    Projectile.ai[0] = Main.rand.NextBool() ? 1f : 0f;
            }
            else if (source is EntitySource_OnHit { EntityStriking: Player player })
            {
                if (player.strongBees)
                    Projectile.ai[0] = Main.rand.NextBool() ? 1f : 0f;
            }
        }

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = GetInstance<HymenoptraDamageClass>();

            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            SafeSetDefaults();
        }

        public sealed override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public virtual void SafePreAI() { }
        public sealed override bool PreAI()
        {
            SafePreAI();
            Player player = Main.player[Projectile.owner];

            if (player.GetModPlayer<BombusApisBeePlayer>().IgnoreWater)
                Projectile.ignoreWater = true;
            else if (Projectile.wet && !Projectile.honeyWet)
                Projectile.Kill();

            if (!Initialized)
            {
                Projectile.penetrate = (Giant && CanBeGiant) ? GiantBeePenetrate : RegularBeePenetrate;
                Projectile.width = (Giant && CanBeGiant) ? GiantWidth : SmallWidth;
                Projectile.height = (Giant && CanBeGiant) ? GiantHeight : SmallHeight;
                if (Giant && CanBeGiant)
                    otherGiant = true;
                Initialized = true;
            }

            return base.PreAI();
        }
        public sealed override void AI()
        {
            SafeAI();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity) + ((Projectile.spriteDirection == 1) ? 0f : 3.1415927f);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);
            if (++Projectile.frameCounter >= FrameTimer)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            float findClosest = 1000f;
            float num262 = Projectile.position.X;
            float num263 = Projectile.position.Y;
            bool flag63 = false;
            BeeAITimer++;
            if (BeeAITimer > 30f)
            {
                BeeAITimer = 30;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this) && (!npc.wet || npc.type == 370))
                    {
                        float num267 = npc.position.X + (float)(npc.width / 2);
                        float num268 = npc.position.Y + (float)(npc.height / 2);
                        float dist = Vector2.Distance(Projectile.Center, npc.Center);
                        if ((dist < (otherGiant ? 1200f : 800f)) && dist < findClosest && (!Projectile.tileCollide || Collision.CanHit(Projectile.position, 1, 1, npc.position, 1, 1)))
                        {
                            findClosest = dist;
                            num262 = num267;
                            num263 = num268;
                            flag63 = true;
                        }
                    }
                }
            }
            if (!flag63)
            {
                num262 = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                num263 = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }
            float num270 = 6f;
            float num271 = 0.1f;
            if (otherGiant)
            {
                num270 = 6.8f;
                num271 = 0.14f;
            }
            Vector2 vector18 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num272 = num262 - vector18.X;
            float num273 = num263 - vector18.Y;
            float num274 = (float)Math.Sqrt(num272 * num272 + num273 * num273);
            float num275 = num274;
            num274 = num270 / num274;
            num272 *= num274;
            num273 *= num274;
            if (Projectile.velocity.X < num272)
            {
                Projectile.velocity.X += num271;
                if (Projectile.velocity.X < 0f && num272 > 0f)
                {
                    Projectile.velocity.X += num271 * 2f;
                }
            }
            else if (Projectile.velocity.X > num272)
            {
                Projectile.velocity.X -= num271;
                if (Projectile.velocity.X > 0f && num272 < 0f)
                {
                    Projectile.velocity.X -= num271 * 2f;
                }
            }
            if (Projectile.velocity.Y < num273)
            {
                Projectile.velocity.Y += num271;
                if (Projectile.velocity.Y < 0f && num273 > 0f)
                {
                    Projectile.velocity.Y += num271 * 2f;
                }
            }
            else if (Projectile.velocity.Y > num273)
            {
                Projectile.velocity.Y -= num271;
                if (Projectile.velocity.Y > 0f && num273 < 0f)
                {
                    Projectile.velocity.Y -= num271 * 2f;
                }
            }
        }

        public virtual bool SafePreDraw(ref Color lightColor) { return true; }
        public sealed override bool PreDraw(ref Color lightColor)
        {
            if (SafePreDraw(ref lightColor))
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
                Texture2D giantTex = Giant ? ModContent.Request<Texture2D>(Texture + "_Giant").Value : ModContent.Request<Texture2D>(Texture).Value;
                Rectangle sourceRectangle = Giant ? giantTex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame) : tex.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);
                Main.spriteBatch.Draw(Giant ? giantTex : tex, Projectile.Center - Main.screenPosition, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, sourceRectangle.Size() / 2f,
                    Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            return false;
        }

        public virtual void SafeModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
        public sealed override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Giant)
            {
                damage = (int)(damage * 1.15f);
                knockback = knockback * 1.25f;
            }

            SafeModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
        }
    }
}