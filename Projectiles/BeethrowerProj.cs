using BombusApisBee.Dusts;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Projectiles
{
    public class BeeThrowerProj : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BeeThrowerProj");     //The English name of the projectile
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 8;               //The width of projectile hitbox
            Projectile.height = 8;              //The height of projectile hitbox
            Projectile.aiStyle = 23;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.penetrate = 2;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 100;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;            //Set to above 0 if you want the projectile to update multiple time in a frame

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
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
            if (Main.rand.NextBool(3)) // only spawn 20% of the time
            {
                int choice = Main.rand.Next(3); // choose a random number: 0, 1, or 2
                if (choice == 0) // use that number to select dustID: 15, 57, or 58
                {
                    choice = DustID.Torch;
                }
                else if (choice == 1)
                {
                    choice = ModContent.DustType<HoneyDust>();
                }
                else
                {
                    choice = DustID.Bee;
                }
                // Spawn the dust
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, choice, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 25, default(Color), 2.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 25, default(Color), 1.1f);
                dust2.velocity *= Main.rand.NextFloat(-1.5f, 2f);
                dust2.noGravity = true;
            }
            if (Projectile.timeLeft % 30 == 0)
            {
                Player player = Main.player[Projectile.owner];
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int type = player.beeType();
                        int damage = player.beeDamage(Projectile.damage);
                        float knockBack = player.beeKB(Projectile.knockBack);
                        Vector2 vel = Projectile.velocity * (Main.rand.NextFloat(0.75f, 1.35f));
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, type, damage * 2 / 3, knockBack, player.whoAmI).DamageType = BeeUtils.BeeDamageClass();
                    }
                }
            }
        }
    }
}