using BombusApisBee.BeeHelperProj;
using BombusApisBee.Content.Crossmod;
using BombusApisBee.Content.Dusts.Pixelized;
using BombusApisBee.Core.PixelationSystem;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Weapons.Sulphuric
{
    [JITWhenModsEnabled("CalamityMod")]
    public class SulphuricApiary : ApiaryItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Apiary");
            Tooltip.SetDefault("Hold <left> to fire sulphuric bees\n" +
                "Hold <right> to fire bees slower, but take control over the bees causing them to inflict Ionized\n" +
                "Ionized spreads between enemies causing explosions\n" +
                "Ionized enemies take 15% more damage from non-bee sources of hymenoptra damage");
        }

        public override void AddDefaults()
        {
            Item.damage = 15;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;

            Item.value = Item.sellPrice(silver: 40);

            Item.rare = ItemRarityID.Green;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<SulphuricApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
            altHoneyCost = 4;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 50;
                Item.useAnimation = 50;

            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;

            }

            return base.SafeCanUseItem(player);
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void OnApiaryHit(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<Ionized>(500);
            target.GetGlobalNPC<IonizedGlobalNPC>().owner = projectile.owner;
            target.GetGlobalNPC<IonizedGlobalNPC>().spread = false;
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(120))
            {
                Color color = Color.Lerp(new Color(140, 234, 87, 0), new Color(89, 93, 48, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.15f);
            }
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().holdTimer;

            Color color = Color.Lerp(new Color(140, 234, 87, 0), new Color(89, 93, 48, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, color * (holdTimer / 20f) * 0.13f, 0f, tex.Size() / 2f, 0.25f, 0, 0f);
        }

        /*public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WoodenApiary>().
                AddIngredient<PearlShard>(3).
                AddIngredient<Pollen>(15).
                AddIngredient(ItemID.GlowingMushroom, 15).
                AddTile(TileID.Anvils).
                Register();
        }*/
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class SulphuricApiaryHoldout : ApiaryHoldout
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override Color GlowColor => Color.Lerp(new Color(140, 234, 87, 0), new Color(89, 93, 48, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override string Texture => "BombusApisBee/Content/Crossmod/Calamity/Items/Weapons/Sulphuric/SulphuricApiary";

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        protected override void Shoot()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Owner.UseBeeResource(Owner.altFunctionUse == 2 ? (Owner.HeldItem.ModItem as ApiaryItem).altHoneyCost : (Owner.HeldItem.ModItem as ApiaryItem).honeyCost))
                {
                    shakeTimer = 15;

                    SoundID.Item97.PlayWith(Projectile.Center, 0, 0.1f, 1.25f);
                    BombusApisBee.HoneycombWeapon.PlayWith(Projectile.Center, volume: 0.5f);

                    for (int j = 0; j < 6; j++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(4f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(0.4f, 0.6f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(4f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(0.4f, 0.8f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(5f, 5f), DustType<SmokeDust2>(),
                            Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(4f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(0.4f, 1f)).noGravity = true;

                        Dust.NewDustPerfect(Projectile.Center, DustType<PixeelatedGlowAltWhite>(), Projectile.velocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 4f), 0, GlowColor with { A = 0 }, 0.25f);
                    }

                    for (int i = 0; i < 1 + Main.rand.Next(1, 4); i++)
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Projectile.velocity * Main.rand.NextFloat(7f, 8f), ProjectileType<SulphuricBee>(), Projectile.damage, Projectile.knockBack);
                    }
                }
                else
                    Projectile.Kill();
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class SulphuricBee : BaseBeeProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Bee");
            Main.projFrames[Type] = 4;
        }

        public override void SafeAI()
        {
            if (Main.rand.NextBool(120))
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixeelatedGlowAltWhite>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            /*for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                    DustID.MushroomTorch, Main.rand.NextVector2Circular(3f, 3f), 0, default, 0.8f).noGravity = true;
            }*/
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    public class Ionized : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ionized");
            Description.SetDefault("straight up got a valence electron up in this joint");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<IonizedGlobalNPC>().inflicted = true;

            if (Main.rand.NextBool(15))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixeelatedGlowAltWhite>(),
                   Main.rand.NextVector2Circular(5f, 5f), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.25f, 0.5f)).noGravity = true;
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class IonizedGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override bool InstancePerEntity => true;

        public bool spread;
        public bool inflicted;
        public int owner;

        public override void ResetEffects(NPC npc)
        {
            if (!inflicted)
            {
                owner = -1;
                spread = false;
            }

            inflicted = false;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (inflicted)
            {
                // increase damage of non-bee hymenoptra damage by 15%
                if (projectile.CountsAsClass<HymenoptraDamageClass>() && !BeeUtils.IsBee(projectile.whoAmI))
                {
                    modifiers.FinalDamage *= 1.15f;
                }
            }
        }

        public override void AI(NPC npc)
        {
            if (inflicted && owner >= 0 && !spread && Main.rand.NextBool(120))
            {
                NPC closest = Main.npc.Where(n => n.CanBeChasedBy() && !n.HasBuff<Ionized>() && n.Distance(npc.Center) < 250f).OrderBy(n => n.Distance(npc.Center)).FirstOrDefault();

                Player player = Main.player[owner];

                if (player is null)
                {
                    inflicted = false;
                    return;
                }

                if (closest != default)
                {
                    if (Main.myPlayer == owner)
                    {
                        new SoundStyle("BombusApisBee/Sounds/Item/FireHit").PlayWith(closest.Center, 0.5f);
                        new SoundStyle("BombusApisBee/Sounds/Crossmod/Calamity/Explosion_1").PlayWith(closest.Center, 0.5f);

                        player.Bombus().AddShake(12);

                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(closest.Center + Main.rand.NextVector2CircularEdge(10f, 10f), DustType<PixeelatedGlowAltWhite>(),
                                Main.rand.NextVector2Circular(8f, 8f), 0, new Color(140, 234, 87, 0), Main.rand.NextFloat(0.5f, 1f)).noGravity = true;

                            Dust.NewDustPerfect(closest.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                                Main.rand.NextVector2Circular(4f, 4f), Main.rand.Next(120, 200), new Color(80, 93, 48), Main.rand.NextFloat(1f, 1.2f)).noGravity = true;
                            
                            Dust.NewDustPerfect(closest.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                                Main.rand.NextVector2Circular(5f, 5f), Main.rand.Next(120, 200), new Color(137, 162, 74), Main.rand.NextFloat(1f, 1.4f)).noGravity = true;

                            Dust.NewDustPerfect(closest.Center + Main.rand.NextVector2CircularEdge(25f, 25f), DustType<SmokeDust2>(),
                                Main.rand.NextVector2Circular(6f, 6f), Main.rand.Next(120, 200), new Color(140, 234, 87), Main.rand.NextFloat(1f, 1.6f)).noGravity = true;
                        }

                        Projectile.NewProjectile(npc.GetSource_FromThis(), closest.Center, Vector2.Zero, ProjectileType<SulphuricExplosion>(), 25, 2f, owner, 60);
                    }

                    spread = true;
                }
            }
        }
    }

    [JITWhenModsEnabled("CalamityMod")]
    class SulphuricExplosion : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        private List<Vector2> cache;

        private Trail trail;
        private Trail trail2;
        public override string Texture => "BombusApisBee/ExtraTextures/Invisible";
        private float Progress => 1 - (Projectile.timeLeft / 30f);

        private float Radius => Projectile.ai[0] * EaseBuilder.EaseQuinticOut.Ease(Progress);

        public bool fromSpear => Projectile.ai[2] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.DamageType = BeeUtils.BeeDamageClass();
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Explosion");
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManageCaches();
                ManageTrail();
            }

            /*for (int k = 0; k < 6; k++)
            {
                float rot = Main.rand.NextFloat(0, 6.28f);

                Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * Radius, ModContent.DustType<PixelatedGlow>(),
                    Vector2.One.RotatedBy(rot) * 0.5f, 0, Main.rand.NextBool() ? new Color(200, 40, 20, 0) : new Color(255, 90, 20, 0), Main.rand.NextFloat(0.5f, 0.6f));
            }*/
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 line = targetHitbox.Center.ToVector2() - Projectile.Center;
            line.Normalize();
            line *= Radius;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + line);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitives();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!fromSpear)
            {
                target.AddBuff<Ionized>(500);
                target.GetGlobalNPC<IonizedGlobalNPC>().owner = Projectile.owner;
                target.GetGlobalNPC<IonizedGlobalNPC>().spread = false;
            }           
        }

        private void ManageCaches()
        {
            if (cache is null)
            {
                cache = new List<Vector2>();

                for (int i = 0; i < 41; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            for (int k = 0; k < 41; k++)
            {
                cache[k] = (Projectile.Center + Vector2.One.RotatedBy(k / 40f * 6.28f) * (Radius));
            }

            while (cache.Count > 41)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 41, new TriangularTip(1), factor => 20 * (1f - Progress), factor =>
            {
                return Color.Lerp(new Color(255, 255, 255), new Color(137, 162, 74), EaseBuilder.EaseCircularOut.Ease(Progress));
            });

            trail2 = trail2 ?? new Trail(Main.instance.GraphicsDevice, 41, new TriangularTip(1), factor => 10 * (1f - Progress), factor =>
            {
                return Color.Lerp(new Color(255, 255, 255), new Color(140, 234, 87), EaseBuilder.EaseCircularOut.Ease(Progress));
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = cache[40];

            trail2.Positions = cache.ToArray();
            trail2.NextPosition = cache[40];
        }

        public void DrawPrimitives()
        {
            GetInstance<PixelationSystem>().QueueRenderAction("UnderProjectiles", () =>
            {
                Effect effect = Filters.Scene["SLRCeirosRing"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.EffectMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["time"].SetValue(Projectile.timeLeft * -0.01f);
                effect.Parameters["repeats"].SetValue(5f);
                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/GlowTrail").Value);

                trail?.Render(effect);
                trail2?.Render(effect);

                effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("BombusApisBee/ShaderTextures/EnergyTrail").Value);

                trail2?.Render(effect);
            });
        }
    }
}
