namespace BombusApisBee.Projectiles
{
    public class TrueBeeSwordProjectile : BeeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("TrueBeeSwordProjectile");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 20;               //The width of projectile hitbox
            Projectile.height = 20;              //The height of projectile hitbox
            Projectile.aiStyle = 27;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.penetrate = 2;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 1050;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)           //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 1f;            //How much light emit around the projectile
            Projectile.ignoreWater = false;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 0;
            Projectile.alpha = 255;
            //Set to above 0 if you want the projectile to update multiple time in a frame          //Act exactly like default Bullet
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
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>());
                dust.noGravity = true;
                dust.scale = 0.9f;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int numberProjectiles = 3 + Main.rand.Next(2); // 4 or 5 shots
            for (int i = 0; i < numberProjectiles; i++)
            {
                Player player = Main.player[Projectile.owner];
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int type = player.beeType();
                        int damage1 = player.beeDamage(Projectile.damage);
                        float knockBack = player.beeKB(Projectile.knockBack);
                        Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                        Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), Projectile.Center, vel, type, damage1 * 3 / 4, knockBack, player.whoAmI).DamageType = BeeUtils.BeeDamageClass(); ;
                    }
                }
            }
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), 0f, 0f, 25, default, 1.1f);
                dust2.velocity *= 0.052f;
                dust2.fadeIn = 1f;
                dust2.noGravity = true;
            }
        }
    }
}