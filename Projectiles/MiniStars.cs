namespace BombusApisBee.Projectiles
{
    public class MiniStars : BeeProjectile
    {
        public int mousetimer;
        public override string Texture => "BombusApisBee/Projectiles/AstralStar";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Star");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        //The recording mode
        }

        public override void SafeSetDefaults()
        {
            Projectile.scale = 0.8f;
            Projectile.width = 8;               //The width of projectile hitbox
            Projectile.height = 8;              //The height of projectile hitbox
            Projectile.aiStyle = 5;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.penetrate = -1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 400;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255;             //The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;          //Can the projectile collide with tiles?
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
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
            Projectile.tileCollide = false;
            Projectile.ai[0] = 1f;
            mousetimer++;
            if (Projectile.ai[0] >= 5)
            {
                Projectile.velocity *= 3 / 4f;
                Projectile.ai[0] = 0;
            }
            if (mousetimer == 35 && Projectile.owner == Main.myPlayer)
            {
                Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                float speed = 10f;
                Vector2 vector3 = Vector2.Normalize(value3 - Projectile.Center);
                vector3 *= speed;
                Projectile.velocity = vector3;
                Projectile.netUpdate = true;
            }

        }
    }
}