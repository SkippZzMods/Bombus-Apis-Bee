using BombusApisBee.Content.Forest.Items.WoodenApiary;
using BombusApisBee.Core.Common.Smoker;
using BombusApisBee.Core.Systems.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace BombusApisBee.Content.Forest.Items.Pollenator
{
    public class PollenatorItem : SmokerItem
    {
        public override void AddDefaults()
        {
            Item.Size = new(20);

            Item.damage = 4;
            Item.knockBack = 0.2f;
            
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.White;
            Item.useTime = Item.useAnimation = 60;

            Item.value = Item.sellPrice(silver: 1);

            Item.autoReuse = true;

            Item.shoot = ProjectileType<PollenatorHoldout>();

            Item.shootSpeed = 1f;

            honeyCost = 5;
        }
    }

    public class PollenatorHoldout : SmokerHoldout
    {
        public PollenatorHoldout() : base(ItemType<PollenatorItem>(), 4, 30, new Color(233, 186, 88) * 0.66f) { }

        public override void AttackBehavior()
        {
            SoundID.DoubleJump.PlayWith(Projectile.Center, -0.5f, 0.1f, 0.5f);

            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.25f);
            _recoilTimer += Main.rand.Next(10, 15);

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(4f, 12f),
                    ProjectileType<PollenatorSmoke>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (Main.rand.NextBool())
                    ParticleHandler.SpawnParticle(new WhiteFlowerParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(0.9f, 1.2f), 60 + Main.rand.Next(90)));
                else
                    ParticleHandler.SpawnParticle(new PinkFlowerParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(0.9f, 1.2f), 60 + Main.rand.Next(90)));
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.Grass, Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 12f), 180, default, 1.35f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.Honey2, Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 8f), 180, default, 1.25f).noGravity = true;

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust2>(), Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 6f), 200, new Color(149, 116, 24), 0.75f);
            }
        }
    }

    public class PollenatorSmoke : SmokerSmokeProjectile
    {
        private class PollenatorBuff : SmokerBuff
        {
            public override void ModifyBuffedBeeHit(Projectile p, NPC target, ref NPC.HitModifiers modifiers)
            {
                modifiers.FinalDamage *= 1.05f;
            }

            public override void OnBuffedBeeHit(Projectile p, NPC target, NPC.HitInfo hit, int damageDone)
            {
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool())
                        ParticleHandler.SpawnParticle(new WhiteFlowerParticle(p.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), 0.65f, 70));
                    else
                        ParticleHandler.SpawnParticle(new PinkFlowerParticle(p.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), 0.65f, 70));
                }
                    
            }

            public override void PreUpdateDebuffedNPC(NPC n)
            {
                n.GetGlobalNPC<PollenatorBuffNPC>().active = true;
            }

            class PollenatorBuffNPC : GlobalNPC
            {
                public bool active;
                public int cooldown;

                public override bool InstancePerEntity => true;

                public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
                {
                    return entity.CanBeChasedBy();
                }

                public override void ResetEffects(NPC npc)
                {
                    if (cooldown > 0)
                        cooldown--;

                    active = false;
                }

                public override void AI(NPC npc)
                {
                    if (active && Main.rand.NextBool(30) && cooldown <= 0)
                        Sneeze(npc);
                }

                internal void Sneeze(NPC npc)
                {
                    SoundID.DoubleJump.PlayWith(npc.Center, -0.1f, 0.2f, 0.45f);
                    SoundID.SplashWeak.PlayWith(npc.Center, 0.2f, volume: 0.5f);

                    cooldown = 240;

                    var info = new NPC.HitInfo
                    {
                        Damage = Main.rand.Next(5, 11)
                    };

                    npc.StrikeNPC(info, false, true);

                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 velocity = Vector2.UnitX * npc.direction * Main.rand.NextFloat(5f);

                        Dust.NewDustPerfect(npc.Center, DustID.Cloud, velocity.RotatedByRandom(0.5f), 120, default, 1.5f).noGravity = true;

                        Dust.NewDustPerfect(npc.Center, DustID.Water, velocity.RotatedByRandom(0.5f), 120, default, 1.5f).noGravity = true;
                    }
                }
            }
        }

        internal Color drawColor;

        public float Progress => 1f - Projectile.timeLeft / 600f;

        public override void SetStaticDefaults()
        {
            _buffID = SmokerBuffLoader.smokerBuffTypes["PollenatorBuff"];
            _buffTime = 600;
        }

        public override bool? CanDamage()
        {
            return Progress < 0.85f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = BeeUtils.BeeDamageClass();

            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;

            Projectile.scale *= Main.rand.NextFloat(2.25f, 3.5f);
            Projectile.rotation = Main.rand.NextFloat(6.28f);

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;

            Projectile.frame = Main.rand.Next(3);

            drawColor = Color.Lerp(new Color(179, 145, 45), new Color(149, 116, 24), Main.rand.NextFloat()) with { A = (byte)Main.rand.Next(150, 200) };
        }

        public override void AI()
        {
            base.AI();

            Projectile.velocity *= 0.96f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;

            float fadeIn;
            if (Progress < 0.15f)
                fadeIn = Progress / 0.15f;
            else
                fadeIn = 1f - (Progress - 0.15f) / 0.75f;

            if (Main.rand.NextBool(60))
            {
                ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(0.5f, 0.5f),
                    drawColor * 0.9f * fadeIn, drawColor * 0.66f * fadeIn, Main.rand.NextFloat(0.1f, 0.2f) * MathHelper.Lerp(0.75f, 1.5f, Progress), 50 + Main.rand.Next(30, 70), false, false, extraUpdateAction: SmokeBehavior));

                static void SmokeBehavior(Particle p)
                {
                    p.Velocity.X *= 0.98f;
                }
            }

            if (Main.rand.NextBool(120))
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(50, 50), DustID.Honey2, Main.rand.NextVector2Circular(2f, 2f), (int)MathHelper.Lerp(255, 150, fadeIn), default, 1.25f).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Rectangle frame = tex.Frame(1, 3, 0, Projectile.frame);

            float fadeIn;
            if (Progress < 0.15f)
                fadeIn = Progress / 0.15f;
            else
                fadeIn = 1f - (Progress - 0.15f) / 0.75f;

            Main.spriteBatch.Draw(bloomTex, Projectile.Center - Main.screenPosition, null, drawColor with { A = 0 } * 0.06f * fadeIn, 0f, bloomTex.Size() / 2f, Projectile.scale * 0.8f, 0f, 0f);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, drawColor * 0.5f * fadeIn, Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(0.5f, 1.5f, Progress), SpriteEffects.None, 0f);

            return false;
        }
    }
}
