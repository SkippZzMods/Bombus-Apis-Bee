namespace BombusApisBee.Projectiles
{
    public class ManaHoneycombHoldout : BeeProjectile
    {
        public float MAXCHARGE;
        public ref float Charge => ref Projectile.ai[0];
        public override string Texture => BombusApisBee.BeeWeapon + "ManaInfusedHoneycomb";
        private Player Owner => Main.player[Projectile.owner];
        public override void SafeSetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Bombus().HeldProj = true;
        }
        public override void AI()
        {
            UpdateHeldProjectile();

            Vector2 tipPos = Projectile.Center + Projectile.velocity * 25f;
            Vector2 starPos = Owner.MountedCenter + new Vector2(25, 15 * Owner.direction).RotatedBy(Projectile.velocity.ToRotation());

            if (MAXCHARGE == 0f)
                MAXCHARGE = Owner.ApplyHymenoptraSpeedTo(Owner.GetActiveItem().useAnimation);

            if (Charge < MAXCHARGE)
            {
                if (Charge == MAXCHARGE - 1)
                {
                    SoundID.MaxMana.PlayWith(Projectile.Center);
                    for (float k = 0; k < 6.28f; k += 0.1f)
                    {
                        float rand = 0;

                        float x = (float)Math.Cos(k + rand);
                        float y = (float)Math.Sin(k + rand);
                        float mult = ((Math.Abs(((k * 5 / 2) % (float)Math.PI) - (float)Math.PI / 2)) * 0.6f) + 0.5f;
                        Dust.NewDustPerfect(starPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), new Vector2(x, y) * mult * 1.2f, 0, new Color(18, 18, 255), 0.35f);
                    }
                }
                Charge++;
            }

            //Owner.Bombus().shakeTimer = 2;

            var modPlayer = Owner.Hymenoptra();
            modPlayer.BeeResourceRegenTimer = -120;
            if (!Owner.channel || modPlayer.BeeResourceCurrent <= 0)
            {
                if (Charge >= MAXCHARGE)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.Bombus().AddShake(4);
                        SoundID.Item109.PlayWith(Projectile.Center, -0.15f, 0.1f, 1.25f);

                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), tipPos, Projectile.velocity.RotatedByRandom(0.25f) * 6f, ModContent.ProjectileType<ManaBee>(), Projectile.damage, 0, Owner.whoAmI);
                        }

                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(tipPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(4f), 0, new Color(18, 18, 255), 0.45f);

                            Dust.NewDustPerfect(tipPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(4f), 0, new Color(100, 96, 255), 0.45f);

                            Dust.NewDustPerfect(tipPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(18, 18, 255), 0.55f);

                            Dust.NewDustPerfect(tipPos, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2f, 2f), 0, new Color(100, 96, 255), 0.55f);
                        }

                        for (int i = 0; i < 12; i++)
                        {
                            Dust.NewDustPerfect(tipPos, ModContent.DustType<Dusts.ManaStardust>(), Projectile.velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(4f), 0, default, Main.rand.NextFloat(0.5f, 1.3f));
                        }
                    }

                    Owner.UseBeeResource((Main.player[Projectile.owner].HeldItem.ModItem as BeeDamageItem).honeyCost);

                    Owner.CheckMana(15, true);
                }
                Projectile.Kill();
                return;
            }

            float lerper = MathHelper.Lerp(50f, 5f, Charge / MAXCHARGE);
            Vector2 pos = tipPos + Main.rand.NextVector2CircularEdge(lerper, lerper);
            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(tipPos), 0, new Color(18, 18, 255), 0.35f);
            pos = tipPos + Main.rand.NextVector2CircularEdge(lerper, lerper);
            Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(tipPos), 0, new Color(100, 96, 255), 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Projectile.velocity * 25f - Main.screenPosition, null, new Color(18, 18, 255, 0) * 0.5f * MathHelper.Lerp(0f, 1f, Charge / MAXCHARGE), 0f, bloomTex.Size() / 2f, 0.55f, 0, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Projectile.Center + Projectile.velocity * 25f - Main.screenPosition, null, new Color(100, 96, 255, 0) * MathHelper.Lerp(0f, 1f, Charge / MAXCHARGE), 0f, bloomTex.Size() / 2f, 0.35f, 0, 0);
            return false;
        }

        public override bool? CanDamage() => false;
        private void UpdateHeldProjectile()
        {
            float lerper = MathHelper.Lerp(0.25f, 0.75f, Charge / MAXCHARGE);
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) + Main.rand.NextVector2Circular(lerper, lerper);
            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
            Projectile.velocity = Utils.ToRotationVector2(Projectile.rotation);
            Projectile.Center += Utils.ToRotationVector2(Projectile.rotation) * 30f;
            Projectile.direction = (Projectile.spriteDirection = Utils.ToDirectionInt(Math.Cos(Projectile.rotation) > 0.0));
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = WrapAngle90Degrees(Projectile.rotation);
            Projectile.rotation += 0.7853982f;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += 1.5707964f;

            Projectile.timeLeft = 2;
        }

        private float WrapAngle90Degrees(float theta)
        {
            if (theta > MathF.PI)
                theta -= MathF.PI;

            if (theta > MathF.PI / 2f)
                theta -= MathF.PI;

            if (theta < -MathF.PI / 2f)
                theta += MathF.PI;

            return theta;
        }
    }
}
