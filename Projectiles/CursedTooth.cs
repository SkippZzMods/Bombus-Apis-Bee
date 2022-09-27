using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BombusApisBee.Projectiles
{
    public class CursedTooth : BeeProjectile
    {
        int enemyWhoAmI;
        bool stuck = false;
        Vector2 offset = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Tooth");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            Main.projFrames[Type] = 2;
        }

        public override void SafeSetDefaults()
        {
            Projectile.frame = Main.rand.Next(2);
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;

            Projectile.penetrate = 5;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            NPC target = Main.npc[enemyWhoAmI];
            if (stuck)
            {
                Projectile.position = target.position + offset;
                if (!target.active)
                    Projectile.Kill();

                return false;
            }
            return base.PreAI();
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 330)
                Projectile.velocity.Y += 0.94f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Main.rand.NextBool())
                Dust.NewDustPerfect(Projectile.Center, DustID.Bone, null).noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CursedToothGlobalNPC gnpc = target.GetGlobalNPC<CursedToothGlobalNPC>();
            if (!stuck && target.life > 0 && gnpc.StuckTeeth < 3)
            {
                gnpc.StuckTeeth++;
                stuck = true;
                Projectile.friendly = false;
                Projectile.tileCollide = false;
                enemyWhoAmI = target.whoAmI;
                offset = Projectile.position - target.position;
                offset -= Projectile.velocity;
                Projectile.timeLeft = 360;
                Projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (stuck)
                Main.npc[enemyWhoAmI].GetGlobalNPC<CursedToothGlobalNPC>().StuckTeeth--;

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Bone, null);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (!stuck)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0);
                }
            }
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(verticalFrames: 2, frameY: Projectile.frame), lightColor, Projectile.rotation, tex.Frame(verticalFrames: 2, frameY: Projectile.frame).Size() / 2f, Projectile.scale, Projectile.direction == -1 ? (SpriteEffects)1 : 0, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return base.OnTileCollide(oldVelocity);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stuck);
            writer.WritePackedVector2(offset);
            writer.Write(enemyWhoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stuck = reader.ReadBoolean();
            offset = reader.ReadPackedVector2();
            enemyWhoAmI = reader.ReadInt32();
        }
    }

    public class CursedToothGlobalNPC : GlobalNPC
    {
        public int StuckTeeth;

        public override bool InstancePerEntity => true;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (StuckTeeth <= 0)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= StuckTeeth * 20;

            if (damage < 5)
                damage = 5;
        }
    }
}
