using BombusApisBee.Content.Forest.Items.Pollen;
using BombusApisBee.Core.Systems.PrimitiveSystem;
using CalamityMod.Items.Materials;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Victide
{
    public class Coralcomb : CalamityDamageItem
    {
        internal float shootRotation;
        internal int shootDirection;
        internal int armDirection;
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a serrated honeycomb capable of shredding through enemies\nCritical hits bleed enemies\nThe last hit always crits and deals 50% more damage");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 11;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<CoralcombProjectile>();
            Item.shootSpeed = 18f;
            Item.UseSound = SoundID.Item1;
            honeyCost = 3;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player Player)
        {
            shootRotation = (Player.Center - Main.MouseWorld).ToRotation();
            shootDirection = Main.MouseWorld.X < Player.Center.X ? -1 : 1;

            if (armDirection != 1 && armDirection != -1)
                armDirection = 1;

            armDirection *= -1;

            return base.CanUseItem(Player);
        }

        public override void UseItemFrame(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.direction = shootDirection;

            float animProgress = 1f - player.itemTime / (float)player.itemTimeMax;
            float rotation = shootRotation * player.gravDir + 1.5707964f;

            if (animProgress < 0.6f)
            {
                float lerper = animProgress / 0.6f;
                rotation += MathHelper.Lerp(0f, -1.57f * armDirection, EaseFunction.EaseCircularOut.Ease(lerper)) * player.direction;

                //Dust.NewDustPerfect(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation), DustType<Glow>(), rotation.ToRotationVector2() * 0.5f, 0, Color.Lerp(new Color(130, 200, 70), new Color(55, 180, 220), animProgress), 0.25f);
            }
            else
            {
                rotation += -1.57f * armDirection * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient<PollenItem>(20).
                AddIngredient<SeaRemains>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class CoralcombProjectile : BeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;

        private List<Vector2> cache;
        private Trail trail;
        private Trail trail2;

        public int hitStopTimer;
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Victide/Coralcomb";

        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coralcomb");
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;

            Projectile.penetrate = 6;
            Projectile.timeLeft = 360;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;

            Projectile.hide = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return hitStopTimer <= 0;
        }

        public override void AI()
        {
            if (hitStopTimer > 0)
                hitStopTimer--;

            Projectile.rotation += Projectile.velocity.Length() * 0.01f * (hitStopTimer > 0 ? 0.4f : 1f);

            if (Projectile.timeLeft < 350 && hitStopTimer <= 0)
            {
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 0)
                {
                    if (Projectile.velocity.Y < 12f)
                        Projectile.velocity.Y *= 1.06f;
                    else
                        Projectile.velocity.Y *= 1.03f;
                }

                if (Projectile.velocity.Y > 20f)
                    Projectile.velocity.Y = 20f;

                Projectile.velocity.X *= 0.985f;
            }

            if (Main.rand.NextBool(2))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), 176, -Projectile.velocity.RotatedByRandom(0.5f) * 0.5f, 50, default, 1f).noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 6;

            if (Projectile.penetrate == 1)
            {
                modifiers.SetCrit();
                modifiers.FinalDamage *= 1.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
                target.AddBuff<CoralcombBleed>(240);

            hitStopTimer = 8;
            Projectile.velocity *= 0.9f;

            Owner.Bombus().AddShake(3, true);

            SoundID.DD2_WitherBeastCrystalImpact.PlayWith(Projectile.Center);

            new SoundStyle("BombusApisBee/Sounds/Item/Impale").PlayWith(Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, 176, -Projectile.velocity.RotatedByRandom(0.5f) * 0.5f, 50, default, 1f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, -Projectile.velocity.RotatedByRandom(0.5f) * 0.5f, 50, default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, -Projectile.velocity.RotatedByRandom(0.5f) * 0.5f, 50, default, 1.25f).fadeIn = 1f;

                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, -Projectile.velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(0.65f), 50, default, 1.45f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, -Projectile.velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(0.65f), 50, default, 1.45f).fadeIn = 1f;

                Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), -Projectile.velocity.RotatedByRandom(0.75f) * Main.rand.NextFloat(0.5f), 50, default, 1.45f).fadeIn = 1f;
            }

            if (Projectile.penetrate == 1)
            {
                SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center);

                //SoundID.DD2_WitherBeastCrystalImpact.PlayWith(Projectile.Center);

                SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center);

                new SoundStyle("BombusApisBee/Sounds/Item/GoreLight").PlayWith(Projectile.Center);

                Owner.Bombus().AddShake(10, true);

                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, 176, -Projectile.velocity.RotatedByRandom(6.28f) * 1.5f, 50, default, 1f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Projectile.velocity.RotatedByRandom(6.28f) * 0.5f, 50, default, 1.25f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Projectile.velocity.RotatedByRandom(6.28f) * 0.5f, 50, default, 1.25f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Projectile.velocity.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.65f), 50, default, 1.45f).noGravity = true;

                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Projectile.velocity.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.65f), 50, default, 1.45f).fadeIn = 1f;

                    Dust.NewDustPerfect(Projectile.Center, DustType<GraveBlood>(), Projectile.velocity.RotatedByRandom(6.28f) * Main.rand.NextFloat(1f), 50, default, 1.45f).fadeIn = 1f;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void OnKill(int timeLeft)
        {
            Owner.Bombus().AddShake(6, true);

            SoundID.DD2_WitherBeastDeath.PlayWith(Projectile.Center);

            SoundID.DD2_MonkStaffGroundImpact.PlayWith(Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, 176, -Projectile.velocity.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.25f), 50, default, 1f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, 176, -Projectile.velocity.RotatedByRandom(6.28f) * 1.25f, 50, default, 1f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D texGlow = Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D texOutline = Request<Texture2D>(Texture + "_Outline").Value;

            Texture2D bloomTex = Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, new Color(50, 150, 200, 0), Projectile.rotation, bloomTex.Size() / 2f, 0.6f, 0, 0f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition, null, lightColor * MathHelper.Lerp(0.5f, 0.25f, i / (float)Projectile.oldPos.Length),
                    Projectile.rotation, tex.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 0.25f, i / (float)Projectile.oldPos.Length), 0, 0);
            }

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(texOutline, Projectile.Center - Main.screenPosition, null, new Color(50, 150, 200), Projectile.rotation, texOutline.Size() / 2f, Projectile.scale, 0, 0f);

            if (hitStopTimer > 0)
            {
                Main.spriteBatch.Draw(texOutline, Projectile.Center - Main.screenPosition, null, new Color(155, 20, 0) * (hitStopTimer / 8f), Projectile.rotation, texOutline.Size() / 2f, Projectile.scale, 0, 0f);

                Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(155, 20, 0, 0) * (hitStopTimer / 8f), Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, 0, 0f);
            }

            return false;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class CoralcombBleed : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleeding");
            Description.SetDefault("Ouch!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2f, npc.height / 2f), DustID.Blood, Main.rand.NextVector2Circular(5f, 5f), 50, default, 1.45f).fadeIn = 1f;

                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2f, npc.height / 2f), DustType<GraveBlood>(), Main.rand.NextVector2Circular(5f, 5f), 50, default, 1.45f).fadeIn = 1f;
            }

            npc.GetGlobalNPC<CoralcombBleedNPC>().inflicted = true;
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class CoralcombBleedNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!inflicted)
                return;

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= 40;

            if (damage < 1)
                damage = 5;
        }
    }
}
