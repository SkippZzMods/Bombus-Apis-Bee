
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class GelBounce : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("GelBounce");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 10;               //The width of projectile hitbox
            Projectile.height = 10;              //The height of projectile hitbox
            Projectile.aiStyle = 14;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?          //Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 4;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 300;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)           //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.2f;            //How much light emit around the projectile
            Projectile.ignoreWater = false;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 0;            //Set to above 0 if you want the projectile to update multiple time in a frame   
                                                    //Act exactly like default Bullet
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 300)
            {
                const int Repeats = 35;
                for (int i = 0; i < Repeats; ++i)
                {
                    float angle2 = 6.2831855f * (float)i / (float)Repeats;
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Water, null, 0, default, 1.1f);
                    dust3.velocity = Utils.ToRotationVector2(angle2) * 1f;
                    dust3.noGravity = true;
                }
            }
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
                dust.noGravity = false;
                dust.scale = 1.5f;
            }

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}