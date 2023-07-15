namespace BombusApisBee.Projectiles
{
    public class TheWaspNestProjectile : BeeProjectile
    {
        internal int BeeTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 14f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 325f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15.5f;
            DisplayName.SetDefault("Wasp Nest");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            // aiStyle 99 is used for all yoyos, and is Extremely suggested, as yoyo are extremely difficult without them
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;

        }

        public override void PostAI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HoneyDust>(), Alpha: Main.rand.Next(50, 150));
                dust.noGravity = true;
                dust.scale = 1.1f;
            }

            BeeTimer++;
            if (BeeTimer > 15)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-5, 5));
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileID.Wasp, Projectile.damage / 2, 1, Projectile.owner).DamageType = BeeUtils.BeeDamageClass();
                    BeeTimer = 0;

                    if (!player.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).beeResourceCost))
                        Projectile.ai[0] = -1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.85f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);
            return false;
        }
    }
}