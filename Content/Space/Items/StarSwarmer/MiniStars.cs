using BombusApisBee.Content.Projectiles;

namespace BombusApisBee.Content.Space.Items.StarSwarmer
{
    public class MiniStars : BeeProjectile
    {
        public int mousetimer;
        public override string Texture => "BombusApisBee/Content/Space/Items/StarStrap/AstralStar";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Star");     
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;  
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;        
        }

        public override void SafeSetDefaults()
        {
            Projectile.scale = 0.8f;
            Projectile.width = 8;               
            Projectile.height = 8;             
            Projectile.aiStyle = 5;           
            Projectile.friendly = true;       
            Projectile.hostile = false;        
            Projectile.penetrate = -1;        
            Projectile.timeLeft = 400;       
            Projectile.alpha = 255;            
            Projectile.ignoreWater = true;          
            Projectile.tileCollide = false;       
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
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
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