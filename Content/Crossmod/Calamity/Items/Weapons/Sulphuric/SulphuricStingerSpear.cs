using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.PixelationSystem;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Sulphuric
{
    public class SulphuricStingerSpear : CalamityDamageItem
    {
        int cooldown;
        internal float shootRotation;
        internal int shootDirection;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurstinger Spear");
            Tooltip.SetDefault("Impales enemies, causing Sulphurstruck enemies to explode\n<right> to throw the spear, inflicting Sulphurstruck");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileType<SulphuricStingerSpearProjectile>();
            Item.shootSpeed = 1;

            Item.value = Item.sellPrice(silver: 40);

            honeyCost = 2;
            altHoneyCost = 10;
        }

        public override bool AltFunctionUse(Player player) => cooldown <= 0;

        public override void UpdateInventory(Player player)
        {
            if (cooldown > 0)
                cooldown--;
        }

        public override bool CanUseItem(Player Player)
        {
            shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
            shootDirection = (Main.MouseWorld.X < Player.Center.X) ? -1 : 1;

            if (Player.altFunctionUse == 2)
            {
                //cooldown = 60;

                Item.useTime = 25;
                Item.useAnimation = 25;
                Item.shootSpeed = 20f;
                Item.UseSound = SoundID.Item1;
                Item.shoot = ModContent.ProjectileType<SulphuricStingerSpearThrownProjectile>();

                Player.Bombus().AddShake(3);
            }
            else
            {
                Item.useTime = 75;
                Item.useAnimation = 75;
                Item.shootSpeed = 1f;
                Item.UseSound = null;
                Item.shoot = ModContent.ProjectileType<SulphuricStingerSpearProjectile>();
            }

            return base.CanUseItem(Player);
        }

        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (Main.myPlayer == player.whoAmI)
                    player.direction = shootDirection;

                float animProgress = 1f - ((float)player.itemTime / (float)player.itemTimeMax);
                float rotation = shootRotation * player.gravDir + 1.5707964f;

                if (animProgress < 0.6f)
                {
                    float lerper = animProgress / 0.6f;
                    rotation += MathHelper.Lerp(-1.57f, 1.57f, EaseBuilder.EaseCircularOut.Ease(lerper)) * player.direction;

                    //Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), DustType<Glow>(), rotation.ToRotationVector2() * 0.5f, 0, Color.Lerp(new Color(130, 200, 70), new Color(55, 180, 220), animProgress), 0.25f);
                }
                else
                {
                    rotation += 1.57f * player.direction;
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class SulphuricStingerSpearProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private int oldTimeLeft;
        private int pauseTimer;
        private int hits;

        public bool playedSound;

        public float rotDir;
        public float rot;
        public Vector2 offset;
        public ref float MaxTimeLeft => ref Projectile.ai[0];
        public float Progress => 1f - Projectile.timeLeft / MaxTimeLeft;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurstinger Spear");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
            Projectile.hide = true;

            rotDir = Main.rand.NextBool(2) ? -1 : 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = Owner.itemAnimation;
            MaxTimeLeft = Projectile.timeLeft;
        }

        public override bool? CanDamage()
        {
            return Progress >= 0.7f;
        }
        public override bool PreAI()
        {
            if (pauseTimer > 0)
            {
                Projectile.timeLeft = oldTimeLeft;

                pauseTimer--;
            }

            return true;
        }

        public override void AI()
        {
            if (Owner.HeldItem.ModItem is not SulphuricStingerSpear)
            {
                Projectile.Kill();
                return;
            }

            ControlsPlayer controlsPlayer = Owner.GetModPlayer<ControlsPlayer>();
            if (Owner == Main.LocalPlayer)
                controlsPlayer.mouseRotationListener = true;

            //Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Projectile.rotation = Projectile.velocity.ToRotation() + rot;

            if (Progress < 0.7f)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(controlsPlayer.mouseWorld), 0.15f);

                float lerper = Progress / 0.7f;

                offset = Vector2.Lerp(new Vector2(50f, 0), new Vector2(35f, 0f), EaseBuilder.EaseCircularInOut.Ease(lerper));

                rot = MathHelper.Lerp(0f, -0.05f, lerper) * rotDir;
            }
            else
            {
                if (!playedSound)
                {
                    Owner.Bombus().AddShake(3);

                    SoundID.DD2_MonkStaffSwing.PlayWith(Projectile.Center);

                    playedSound = true;
                }

                float lerper = (Progress - 0.7f) / 0.3f;

                offset = Vector2.Lerp(new Vector2(35f, 0f), new Vector2(100f, 0f), EaseBuilder.EaseBackOut.Ease(lerper));

                rot = MathHelper.Lerp(-0.05f, 0.1f, lerper) * rotDir;
            }
            
            Projectile.Center = Owner.MountedCenter + offset.RotatedBy(Projectile.rotation);

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Owner.ChangeDir(Projectile.direction);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<Sulphurstruck>())
            {
                target.GetGlobalNPC<SulphurstrukGlobalNPC>().explosionTimer = 60;
            }

            if (hits <= 0)
                pauseTimer = 5;

            oldTimeLeft = Projectile.timeLeft;

            hits = 1;

            Owner.Bombus().AddShake(6);

            if (BeeUtils.IsFleshy(target))
            {
                new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(target.Center, -0.25f, 0.2f);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = target.Center.DirectionTo(Owner.Center);

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(5.5f), 50, default, 2f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(3.5f), 50, default, 2f).fadeIn = 1f;
                }
            }
            else
            {
                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/Clink").PlayWith(target.Center, -0.15f, 0.1f);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = target.Center.DirectionTo(Owner.Center);

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixeelatedGlowAltWhite>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(5.5f), 50, new Color(255, 156, 41, 0), .2f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(), velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(15f), 50, new Color(255, 156, 41, 0), .2f);
                }
            }

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FireBladeStab").PlayWith(target.Center, -0.25f, 0.2f, 0.5f);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = target.Center.DirectionTo(Owner.Center);

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(4.5f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(5.5f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1.25f, 1.25f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(0.7f, 0.9f)).noGravity = true;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texBlur = ModContent.Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texOutline = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Texture2D texWhite = ModContent.Request<Texture2D>(Texture + "_White").Value;

            Vector2 offset = new Vector2(-55, 0).RotatedBy(Projectile.rotation);

            float fade = 1f;

            if (Progress < 0.4f)
            {
                fade = Progress / 0.4f;
            }           
            else if (Progress > 0.8f)
            {
                fade = 1f - (Progress - 0.8f) / 0.2f;
            }

            if (Progress < 0.7f)
            {
                float lerper = Progress / 0.7f;

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, new Color(137, 162, 74, 0)
                    * fade * lerper, Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);
            }
            else
            {
                float lerper = (Progress - 0.7f) / 0.3f;

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, new Color(137, 162, 74, 0)
                    * fade * (1f - lerper), Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);

                Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, new Color(255, 255, 255, 0)
                    * fade * (1f - lerper), Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, Color.White * fade, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, 0, 0);
            
            if (Progress < 0.7f)
            {
                float lerper = Progress / 0.7f;
            }
            else
            {
                float lerper = (Progress - 0.7f) / 0.3f;

                Main.spriteBatch.Draw(texBlur, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, new Color(255, 255, 255, 0)
                    * fade * (1f - lerper) * 0.5f, Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);
            }

            return false;
        }
    }

    // holy yap fest
    [JITWhenModsEnabled("CalamityMod")]
    public class SulphuricStingerSpearThrownProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public int hitStopTimer;

        public Vector2 oldVelo;
        public bool tileHit = false;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Sulphuric/SulphuricStingerSpearProjectile";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurstinger Spear");
        }

        public override void SafeSetDefaults()
        {
            Projectile.timeLeft = 360;

            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
           
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.hide = true;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.penetrate > 1;
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.timeLeft > 60 && !tileHit && hitStopTimer <= 0;
        }

        public override bool PreAI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            if (hitStopTimer > 0)
                hitStopTimer--;

            if (tileHit)
            {
                Projectile.velocity = oldVelo;

                Projectile.timeLeft--;
                Projectile.rotation = Projectile.velocity.ToRotation() + Main.rand.NextFloat(-0.15f, 0.15f) * EaseBuilder.EaseCircularIn.Ease(Projectile.timeLeft / 60f);
                return false;
            }

            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft < 350)
            {
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 12f)
                        Projectile.velocity.Y *= 1.05f;
                    else
                        Projectile.velocity.Y *= 1.03f;
                }

                if (Projectile.velocity.Y > 30f)
                    Projectile.velocity.Y = 30f;

                Projectile.velocity.X *= 0.985f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!tileHit)
            {
                SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center, -0.25f, 0.2f, 1.5f);

                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FireBladeStab").PlayWith(Projectile.Center, -0.25f, 0.2f, 0.15f);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = -Projectile.rotation.ToRotationVector2();

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<PixelatedGlowAlt>(),
                        Main.rand.NextVector2Circular(5f, 5f), 0, new Color(80, 93, 48, 0), 0.8f);
                    
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<PixelatedGlowAlt>(),
                        Main.rand.NextVector2Circular(5f, 5f), 0, new Color(137, 162, 74, 0), 0.8f);

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<SmokeDust2>(),
                        velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(5.5f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<SmokeDust2>(),
                        velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(5.5f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<SmokeDust2>(),
                        Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<SmokeDust2>(),
                        Main.rand.NextVector2Circular(1.25f, 1.25f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f) + -velocity * 30f, DustType<SmokeDust2>(),
                        Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(0.7f, 0.9f)).noGravity = true;
                }

                Owner.Bombus().AddShake(5, true);

                oldVelo = oldVelocity;
                Projectile.timeLeft = 60;
                tileHit = true;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == 4 && target.GetGlobalNPC<SulphurstrukGlobalNPC>().cooldown <= 0)
                target.AddBuff<Sulphurstruck>(600);

            hitStopTimer = 8;

            Owner.Bombus().AddShake(3);

            if (BeeUtils.IsFleshy(target))
            {
                new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(target.Center, -0.25f, 0.2f);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = -Projectile.rotation.ToRotationVector2();

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.5f) * 4.5f, 50, default, 1.25f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(4.65f), 50, default, 1.5f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(5.5f), Main.rand.Next(120, 200), new Color(101, 13, 13), Main.rand.NextFloat(0.2f, 0.3f)).noGravity = true;
                    
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(6.5f), Main.rand.Next(120, 200), new Color(215, 29, 29), Main.rand.NextFloat(0.3f, 0.4f)).noGravity = true;
                    
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                        velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(7.5f), Main.rand.Next(120, 200), new Color(150, 15, 15), Main.rand.NextFloat(0.4f, 0.5f)).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(8.5f), 50, default, 2f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(5.5f), 50, default, 2f).fadeIn = 1f;
                }
            }
            else
            {
                new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/Clink").PlayWith(target.Center, -0.15f, 0.1f);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = -Projectile.rotation.ToRotationVector2();

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixeelatedGlowAltWhite>(), velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(5.5f), 50, new Color(255, 156, 41, 0), .2f);

                    Dust.NewDustPerfect(Projectile.Center, DustType<PixelImpactLineDustGlow>(), velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(15f), 50, new Color(255, 156, 41, 0), .2f);
                }
            }

            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/FireBladeStab").PlayWith(target.Center, -0.25f, 0.2f, 0.5f);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = target.Center.DirectionTo(Owner.Center);

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(4.5f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(5.5f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1f, 1f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1.25f, 1.25f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(1.5f, 1.5f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(0.7f, 0.9f)).noGravity = true;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texBlur = ModContent.Request<Texture2D>(Texture + "_Blur").Value;
            Texture2D texOutline = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Texture2D texWhite = ModContent.Request<Texture2D>(Texture + "_White").Value;

            Texture2D starTex = ModContent.Request<Texture2D>("BombusApisBee/Assets/ExtraTextures/StarAlpha").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Vector2 offset = new Vector2(-35, 0).RotatedBy(Projectile.rotation);

            float fade = 1f;

            if (Projectile.timeLeft > 320)
            {
                fade = 1f - (Projectile.timeLeft - 320) / 40f;
            }

            if (Projectile.timeLeft < 60)
            {
                fade = Projectile.timeLeft / 60f;
            }
            
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition + offset, null, new Color(137, 162, 74, 0) * fade, Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + offset, null, Color.White * fade, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, 0, 0);
            
            Main.spriteBatch.Draw(texOutline, Projectile.Center - Main.screenPosition + offset, null, new Color(140, 234, 87) * fade, Projectile.rotation + MathHelper.PiOver4, texOutline.Size() / 2f, Projectile.scale, 0, 0);
            
            Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition + offset, null, new Color(140, 234, 87, 0) * fade * 0.5f, Projectile.rotation + MathHelper.PiOver4, texGlow.Size() / 2f, Projectile.scale, 0, 0);

            if (Projectile.timeLeft > 330)
            {
                Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null,
                    new Color(140, 234, 87, 0) * ((Projectile.timeLeft - 330) / 30f) * 0.35f, 0f, bloomTex.Size() / 2f, 2f, 0, 0);

                Main.spriteBatch.Draw(starTex, Projectile.Center - Main.screenPosition, null,
                    new Color(140, 234, 87, 0) * ((Projectile.timeLeft - 330) / 30f), 0f, starTex.Size() / 2f, 0.65f, 0, 0);
            }

            return false;
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 10; i++)
                {
                    cache.Add(Projectile.Center + Projectile.velocity);
                }
            }

            cache.Add(Projectile.Center + Projectile.velocity);

            while (cache.Count > 10)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 4.5f, factor =>
            Color.Lerp(new Color(137, 162, 74), new Color(140, 234, 87), EaseBuilder.EaseQuarticOut.Ease(1f - factor.X)));

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;

            trail2 ??= new Trail(Main.instance.GraphicsDevice, 10, new TriangularTip(190), factor => factor * 4f, factor =>
            Color.Lerp(new Color(137, 162, 74), new Color(140, 234, 87), EaseBuilder.EaseQuarticOut.Ease(1f - factor.X)));

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public void DrawPrimitives()
        {
            ModContent.GetInstance<PixelationSystem>().QueueRenderAction("UnderTiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());

                // !!! IMPORTANT WHEN PIXELIZING, MAKE SURE TO USE Main.GameViewMatrix.EffectMatrix IMPORTANT !!!

                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
                effect.Parameters["repeats"].SetValue(1f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);
                trail?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/FireTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);
            });
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class Sulphurstruck : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurstruck");
            Description.SetDefault("gorkin off the sulphur straight strikin it");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<SulphurstrukGlobalNPC>().inflicted = true;
            npc.GetGlobalNPC<SulphurstrukGlobalNPC>().buffIndex = buffIndex;

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixeelatedGlowAltWhite>(),
                   new Vector2(0f, -Main.rand.NextFloat(5f)), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class SulphurstrukGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool InstancePerEntity => true;

        public bool inflicted;
        public int owner;

        public int explosionTimer;
        public int cooldown;

        public int buffIndex;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
            if (cooldown > 0)
                cooldown--;
        }

        public override void AI(NPC npc)
        {
            if (explosionTimer > 0)
            {
                explosionTimer--;

                float lerper = 1f - explosionTimer / 60f;

                if (explosionTimer == 1)
                {
                  
                    Explode(npc);

                    npc.DelBuff(npc.FindBuffIndex(ModContent.BuffType<Sulphurstruck>()));
                    inflicted = false;
                    cooldown = 60;

                    return;
                }

                if (explosionTimer % 10 == 0)
                {
                    SoundID.MaxMana.PlayWith(npc.Center);

                    Vector2 dir = Main.rand.NextVector2CircularEdge(5f, 5f);

                    float peakVelocity = MathHelper.Lerp(0.75f, 2f, lerper);

                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDustPerfect(npc.Center, DustType<PixeelatedGlowAltWhite>(),
                            dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.1f, peakVelocity), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(npc.Center, DustType<SmokeDust2>(),
                            dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.1f, peakVelocity), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.5f, 0.7f)).noGravity = true;

                        Dust.NewDustPerfect(npc.Center, DustType<SmokeDust2>(),
                            dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.1f, peakVelocity), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.6f, 0.8f)).noGravity = true;

                        Dust.NewDustPerfect(npc.Center, DustType<SmokeDust2>(),
                            dir.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.1f, peakVelocity), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(0.7f, 0.9f)).noGravity = true;
                    }
                }
            }
        }

        private void Explode(NPC npc)
        {
            new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(npc.Center, 0.75f);
            new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/Explosion_1").PlayWith(npc.Center, 0.75f);

            Main.player[owner].Bombus().AddShake(15);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixeelatedGlowAltWhite>(),
                    Main.rand.NextVector2Circular(8f, 8f), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;

                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(4f, 4f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(1f, 1.2f)).noGravity = true;

                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(1f, 1.4f)).noGravity = true;

                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                    Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(1f, 1.6f)).noGravity = true;
            } 

            Projectile.NewProjectileDirect(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<SulphuricExplosion>(), 65, 2f, owner, 60).ai[2] = 1f;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (inflicted)
            {
                drawColor = new Color(0, 255, 0);
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
