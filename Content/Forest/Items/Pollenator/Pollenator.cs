using BombusApisBee.Content.Forest.Items.WoodenApiary;
using BombusApisBee.Core.Common.Smoker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public PollenatorHoldout() : base(ItemType<PollenatorItem>(), 4, 30) { }

        public override void AttackBehavior()
        {
            base.AttackBehavior();

            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(8f, 12f),
                    ProjectileType<PollenatorSmoke>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    public class PollenatorSmoke : SmokerSmokeProjectile
    {
        private class PollenatorBuff : SmokerBuff
        {
            public override void ModifyBuffedBeeHit(Projectile p, NPC target, ref NPC.HitModifiers modifiers)
            {
                Main.NewText("Pollenator Buffed Hit");
            }

            public override void UpdateDebuffedNPC(NPC n)
            {
                Main.NewText("Pollenator Debuff Active");
            }
        }

        internal Color drawColor;

        public override void SetStaticDefaults()
        {
            _buffID = SmokerBuffLoader.smokerBuffTypes["PollenatorBuff"];
            _buffTime = 600;
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

            Projectile.scale *= Main.rand.NextFloat(1.25f, 2.5f);
            Projectile.rotation = Main.rand.NextFloat(6.28f);

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;

            Projectile.frame = Main.rand.Next(3);

            drawColor = Color.Yellow;
        }

        public override void AI()
        {
            base.AI();

            Projectile.velocity *= 0.96f;
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Rectangle frame = tex.Frame(1, 3, 0, Projectile.frame);

            float progress = 1f - Projectile.timeLeft / 600f;

            float fadeIn;
            if (progress < 0.25f)
                fadeIn = progress / 0.25f;
            else
                fadeIn = 1f - (progress - 0.25f) / 0.75f;

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, drawColor * 0.5f * fadeIn, Projectile.rotation, frame.Size() / 2f, Projectile.scale * MathHelper.Lerp(1f, 1.5f, progress), SpriteEffects.None, 0f);

            return false;
        }
    }
}