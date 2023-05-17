using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace BombusApisBee.Projectiles
{
    public class TheTraitorsSaxophoneHoldout : BeeProjectile
    {
        public float flashTimer;
        public ref float MaxCharge => ref Projectile.ai[1];
        public ref float Charge => ref Projectile.ai[0];
        public Player owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/TheTraitorsSaxophone";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Traitor's Saxophone");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld);
        }

        public override void AI()
        {
            Charge++;
            if (flashTimer > 0)
                flashTimer--;

            if (!owner.channel || owner.noItems || owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (MaxCharge == 0f)
                MaxCharge = owner.ApplyHymenoptraSpeedTo(owner.GetActiveItem().useAnimation);

            Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
            armPos += Projectile.velocity.SafeNormalize(owner.direction * Vector2.UnitX) * 10f;
            if (Main.myPlayer == Projectile.owner)
                armPos += Vector2.UnitY.RotatedBy(owner.DirectionTo(Main.MouseWorld).ToRotation()) * 13f * (Main.MouseWorld.X < owner.Center.X ? -1 : 1);

            if (Charge % MaxCharge == 0)
            {
                if (owner.UseBeeResource(1))
                {
                    flashTimer = 15;
                    string sax = "Sax" + Main.rand.Next(1, 7);
                    new SoundStyle("BombusApisBee/Sounds/Item/" + sax).PlayWith(Projectile.Center, 0f, 1f);

                    int projType = Main.rand.Next(new int[] { ModContent.ProjectileType<MiniHoneyNoteQuarter>(), ModContent.ProjectileType<MiniHoneyNoteEighth>(), ModContent.ProjectileType<MiniHoneyNoteEighthTied>() });
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), armPos + new Vector2(12, 15 * Projectile.direction).RotatedBy(Projectile.rotation), Vector2.UnitY * -Main.rand.NextFloat(5f, 10f), projType, Projectile.damage, Projectile.knockBack, Projectile.owner);

                    if (Main.rand.NextBool(3))
                    {
                        int type = Main.rand.Next(new int[] { ModContent.ProjectileType<HoneyNoteQuarter>(), ModContent.ProjectileType<HoneyNoteEighth>(), ModContent.ProjectileType<HoneyNoteEighthTied>() });
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), armPos + new Vector2(12, 15 * Projectile.direction).RotatedBy(Projectile.rotation), Vector2.UnitY * -Main.rand.NextFloat(8f, 15f), type, (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                        owner.UseBeeResource(2);
                    }

                    for (float k = 0; k < 6.28f; k += 0.1f)
                    {
                        float x = (float)Math.Cos(k) * 60;
                        float y = (float)Math.Sin(k) * 25;

                        Vector2 pos = armPos + new Vector2(14, 10 * Projectile.direction).RotatedBy(Projectile.rotation);
                        Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDustSolid>(), new Vector2(x, y).RotatedBy(Projectile.rotation + 0.5f * Projectile.direction) * 0.045f, 75, default, 1.1f).noGravity = true;
                    }

                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 pos = armPos + new Vector2(14, 10 * Projectile.direction).RotatedBy(Projectile.rotation);
                        Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.HoneyDustSolid>(), Vector2.UnitY.RotatedBy(0.5f * Projectile.direction).RotatedByRandom(0.45f) * -Main.rand.NextFloat(5f), Main.rand.Next(50, 150), default, 1.25f).noGravity = true;
                    }
                }
                else
                    Projectile.Kill();
            }

            Projectile.spriteDirection = owner.direction;
            owner.ChangeDir(Projectile.direction);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            Projectile.timeLeft = 2;
            Projectile.rotation = Utils.ToRotation(Projectile.velocity);
            owner.itemRotation = Utils.ToRotation(Projectile.velocity * Projectile.direction);

            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Projectile.position = armPos - Projectile.Size * 0.5f;

            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);

                Vector2 oldVelocity = Projectile.velocity;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, owner.DirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glowy").Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : 0f, 0f);
            if (flashTimer > 0)
                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0) * MathHelper.Lerp(1f, 0f, 1f - flashTimer / 15f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipVertically : 0f, 0f);

            return false;
        }
    }
}
