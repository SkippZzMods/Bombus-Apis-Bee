using Terraria;
namespace BombusApisBee.Projectiles
{
    public class MiniHoneyNoteQuarter : BeeProjectile
    {
        public Vector2 mouse;
        public Vector2 startVelo;
        public int bounces = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Honey Quarter Note");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 6;

            Projectile.friendly = true;
            Projectile.timeLeft = 240;

            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 240 && Main.myPlayer == Projectile.owner)
            {
                mouse = Main.MouseWorld;
                startVelo = Projectile.velocity;
            }

            if (Projectile.timeLeft > 190)
            {
                Projectile.velocity = Vector2.Lerp(startVelo, Projectile.DirectionTo(mouse) * 14f, 1f - (Projectile.timeLeft - 190) / 50f);
                if (Projectile.Distance(mouse) < 15f)
                {
                    Projectile.timeLeft = 190;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 14f;
                }

            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Vector2.Zero, 100, default, 0.8f).noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 150), default, 1.1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(229, 114, 0), 0.3f).noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 200)
                Projectile.timeLeft = 200;

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (--bounces <= 0)
                Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0) * 0.5f, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
    public class MiniHoneyNoteEighth : BeeProjectile
    {
        public Vector2 mouse;
        public Vector2 startVelo;
        public int bounces = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Honey Eighth Note");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 6;

            Projectile.friendly = true;
            Projectile.timeLeft = 240;

            Projectile.penetrate = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 240 && Main.myPlayer == Projectile.owner)
            {
                mouse = Main.MouseWorld;
                startVelo = Projectile.velocity;
            }

            if (Projectile.timeLeft > 200)
            {
                Projectile.velocity = Vector2.Lerp(startVelo, Projectile.DirectionTo(mouse) * 17f, 1f - (Projectile.timeLeft - 200) / 40f);
                if (Projectile.Distance(mouse) < 15f)
                {
                    Projectile.timeLeft = 200;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 17f;
                }

            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Vector2.Zero, 100, default, 0.8f).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(2f, 2f), Main.rand.Next(50, 150), default, 1.1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(1.5f, 1.5f), 0, new Color(229, 114, 0), 0.3f).noGravity = true;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2CircularEdge(3f, 3f), Main.rand.Next(50, 150), default, 1.1f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2CircularEdge(2.5f, 2.5f), 0, new Color(229, 114, 0), 0.3f).noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 150), default, 1.1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(229, 114, 0), 0.3f).noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 200)
                Projectile.timeLeft = 200;

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (--bounces <= 0)
                Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0f, 0f);
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0) * 0.5f, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0f, 0f);

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
    public class MiniHoneyNoteEighthTied : BeeProjectile
    {
        public Vector2 mouse;
        public int bounces = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Honey Tied Eighth Note");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.timeLeft = 360;

            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && Projectile.Distance(n.Center) < 750f).OrderBy(n => Vector2.Distance(n.Center, Projectile.Center)).FirstOrDefault();
            if (target != default)
                Projectile.velocity = (Projectile.velocity * 25f + Utils.SafeNormalize(target.Center - Projectile.Center, Vector2.UnitX) * 15f) / 26f;
            else
            {
                Projectile.velocity *= 0.97f;
                if (Projectile.velocity.Length() < 1f)
                    Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.HoneyDustSolid>(), Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(50, 150), default, 1.1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, new Color(229, 114, 0), 0.3f).noGravity = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            if (--bounces <= 0)
                Projectile.Kill();

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, lightColor * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

                Main.spriteBatch.Draw(texGlow, (Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f) - Main.screenPosition, null, (new Color(229, 114, 0, 0) * 0.5f) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length),
                    Projectile.rotation, texGlow.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.5f, (i / (float)Projectile.oldPos.Length)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(229, 114, 0, 0) * 0.5f, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : 0f, 0f);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
