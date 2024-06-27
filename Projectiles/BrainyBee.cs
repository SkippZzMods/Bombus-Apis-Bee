using BombusApisBee.BeeHelperProj;
using Terraria;

namespace BombusApisBee.Projectiles
{
    public class BrainyBee : BaseBeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brainy Bee");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 240);
            if (Main.rand.NextBool())
            {
                target.GetGlobalNPC<BrainyBeeGlobalNPC>().stacks++;
                target.GetGlobalNPC<BrainyBeeGlobalNPC>().timer = 360;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, Main.rand.Next(80)).noGravity = true;
            }
        }

        public override bool SafePreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGiant = ModContent.Request<Texture2D>(Texture + "_Giant").Value;
            Rectangle frame = tex.Frame(verticalFrames: 4, frameY: Projectile.frame);
            if (Giant)
            {
                frame = texGiant.Frame(verticalFrames: 4, frameY: Projectile.frame);
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(texGiant, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
            else
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frame, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                        Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }

    class BrainyBeeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public const int maxStacks = 5;

        public int stacks;

        public int timer;

        public override void ResetEffects(NPC npc)
        {
            stacks = Utils.Clamp(stacks, 0, maxStacks);
            if (timer > 0)
                timer--;
            else
                stacks = 0;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (stacks <= 0)
                return;

            modifiers.SourceDamage *= 1f - 0.05f * stacks;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (stacks <= 0)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= stacks * 5;

            if (damage < 2)
                damage = 2;
        }

        public override void AI(NPC npc)
        {
            if (stacks <= 0)
                return;

            if (Main.rand.NextBool(65 - (stacks * 10)))
            {
                Vector2 velo = Main.rand.NextVector2Circular(3f, 3f);
                Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.QuestionmarkDust>(), velo.X, velo.Y).scale = Main.rand.NextFloat(0.8f, 1f);
            }
        }
    }
}