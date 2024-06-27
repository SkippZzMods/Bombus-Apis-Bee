using BombusApisBee.Items.Other.OnPickupItems;
using Terraria;

namespace BombusApisBee.Projectiles
{
    public class AstralStarSplitting : ModProjectile
    {
        public bool split;
        public int splitCounter = 5;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Splitting Star");
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.timeLeft = 240;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (splitCounter <= 0)
                Projectile.Kill();

            if (split)
            {
                Projectile.timeLeft = 2;

                for (int i = 0; i < 4; i++)
                {
                    float rand = Main.rand.NextFloat(6.28f);
                    float x = (float)Math.Cos(rand);
                    float y = (float)Math.Sin(rand);
                    float mult = ((Math.Abs(((rand * (5f / 2)) % (float)Math.PI) - (float)Math.PI / 2)) * 0.3f) + 0.5f;
                    Vector2 pos = Projectile.Center + new Vector2(x, y).RotatedBy(Projectile.rotation) * mult * 35f;
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center) * 0.25f, 0, new Color(181, 127, 207), 0.35f);
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(15f, 15f);
                    Dust.NewDustPerfect(pos, ModContent.DustType<Dusts.GlowFastDecelerate>(), pos.DirectionTo(Projectile.Center) * 0.25f, 0, new Color(181, 127, 207), 0.35f);
                }
                return;
            }

            Projectile.rotation += 0.35f * Projectile.direction;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                Terraria.Audio.SoundEngine.PlaySound(in SoundID.Item9, Projectile.position);
            }

            //dust
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 1.5f, ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.UnitX.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2) * 1.85f, 0, new Color(181, 127, 207), 0.35f);
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 1.5f, ModContent.DustType<Dusts.GlowFastDecelerate>(), -Vector2.UnitX.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2) * 1.85f, 0, new Color(181, 127, 207), 0.35f);
        }

        public override void OnKill(int timeLeft)
        {
            if (split)
            {
                BeeUtils.DrawStarWithRot(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0, new Color(181, 127, 207), true, 5, 2.25f, 1f, 0.6f, 0.35f, rot: Projectile.rotation);
                if (Main.myPlayer == Projectile.owner && !Projectile.noDropItem)
                {
                    int item = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, ModContent.ItemType<AstralStarPickup>());
                    Main.item[item].noGrabDelay = 60;
                    if (Main.netMode == NetmodeID.MultiplayerClient && item >= 0)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (split)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/AstralStar_Glow").Value;
            Texture2D starTex = TextureAssets.Extra[91].Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, Color.White * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(texGlow, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, (new Color(181, 127, 207, 0) * 0.5f) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(112, 83, 163, 0), 0f, bloomTex.Size() / 2f, 0.65f, 0, 0);

            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition - Projectile.velocity + Vector2.One.RotatedBy(Projectile.rotation * 0.5f) * (i == 0 ? 5f : -5f), null, new Color(181, 127, 207, 0) * 0.45f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, starTex.Size() / 2f, 0.5f, 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.5f, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.85f, 0f, bloomTex.Size() / 2f, 0.45f, 0, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SplitEffects(Projectile.velocity);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (split)
                return false;

            SplitEffects(oldVelocity);

            return false;
        }

        public void SplitEffects(Vector2 velocity)
        {
            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center, -0.1f, 0.15f);

            for (int i = 0; i < 5; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<AstralStarSplittingShards>(), Projectile.damage / 5, 1f, Projectile.owner);
                (proj.ModProjectile as AstralStarSplittingShards).parent = Projectile;
                proj.frame = i;
            }

            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(5.5f, 5.5f), 0, new Color(181, 127, 207), 0.65f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Glow>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.4f), 0, new Color(181, 127, 207), 0.4f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f), 0, new Color(112, 83, 163), 0.4f);
            }

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Stardust>(), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.55f), 0, default, 1.15f);
            }

            Main.player[Projectile.owner].Bombus().AddShake(3);
            Projectile.velocity = Vector2.Zero;
            Projectile.friendly = false;
            split = true;
        }
    }

    class AstralStarSplittingShards : ModProjectile
    {
        public Projectile parent;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Shard");
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 6;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 240;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {

            if (parent is null)
                return;

            if (Projectile.timeLeft > 215)
                return;

            Vector2 parentCenter = parent.Center;
            Vector2 pos = Projectile.Center;

            float betweenX = parentCenter.X - pos.X;
            float betweenY = parentCenter.Y - pos.Y;

            float distance = (float)Math.Sqrt(betweenX * betweenX + betweenY * betweenY);
            float speed = Utils.Clamp(distance * 0.065f, 1f, 20f);
            float adjust = 1f;

            distance = speed / distance;
            betweenX *= distance;
            betweenY *= distance;

            if (Projectile.velocity.X < betweenX)
            {
                Projectile.velocity.X += adjust;
                if (Projectile.velocity.X < 0f && betweenX > 0f)
                    Projectile.velocity.X += adjust;
            }
            else if (Projectile.velocity.X > betweenX)
            {
                Projectile.velocity.X -= adjust;
                if (Projectile.velocity.X > 0f && betweenX < 0f)
                    Projectile.velocity.X -= adjust;
            }
            if (Projectile.velocity.Y < betweenY)
            {
                Projectile.velocity.Y += adjust;
                if (Projectile.velocity.Y < 0f && betweenY > 0f)
                    Projectile.velocity.Y += adjust;
            }
            else if (Projectile.velocity.Y > betweenY)
            {
                Projectile.velocity.Y -= adjust;
                if (Projectile.velocity.Y > 0f && betweenY < 0f)
                    Projectile.velocity.Y -= adjust;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            if (Vector2.Distance(Projectile.Center, parent.Center) < 10f)
            {
                (parent.ModProjectile as AstralStarSplitting).splitCounter--;
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(181, 127, 207), 0.35f);

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f) * Main.rand.NextFloat(0.75f), 0, new Color(112, 83, 163), 0.2f);
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.Stardust>(), Main.rand.NextVector2Circular(2.5f, 2.5f) * Main.rand.NextFloat(0.55f), 0, default, 0.8f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Rectangle frameRect = tex.Frame(verticalFrames: 5, frameY: Projectile.frame);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, frameRect, Color.White * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.oldRot[i], frameRect.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.75f, (i / (float)Projectile.oldPos.Length)), 0, 0);

                Main.spriteBatch.Draw(bloomTex, (Projectile.oldPos[i] + (Projectile.Size * 0.5f)) - Main.screenPosition, null, new Color(181, 127, 207, 0) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, bloomTex.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.25f, 0.05f, (i / (float)Projectile.oldPos.Length)), 0, 0);
            }

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(112, 83, 163, 0), 0f, bloomTex.Size() / 2f, 0.35f, 0, 0);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frameRect, Color.White, Projectile.rotation, frameRect.Size() / 2f, Projectile.scale, 0f, 0f);

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.85f, 0f, bloomTex.Size() / 2f, 0.15f, 0, 0);
            return false;
        }
    }
}