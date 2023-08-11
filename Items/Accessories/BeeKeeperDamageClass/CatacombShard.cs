using BombusApisBee.Items.Other.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class CatacombShard : BeeKeeperItem
    {
        public override void Load()
        {
            BombusApisBeeGlobalProjectile.StrongBeeOnHitEvent += BombusApisBeeGlobalProjectile_StrongBeeOnHitEvent;
        }

        private void BombusApisBeeGlobalProjectile_StrongBeeOnHitEvent(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[proj.owner].Bombus().HasCatacombShard && Main.rand.NextBool(15))
            {

            }
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases the chance to strengthen friendly bees by 60%\nStrengthened bees have a chance to spawn water bolts on hit");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Hymenoptra().BeeStrengthenChance += 0.6f;
            player.Bombus().HasCatacombShard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Pollen>(50).
                AddIngredient(ItemID.Bone, 50).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class CatacombShardProjectile : BeeProjectile
    {
        public override string Texture => "BombusApisBee/Projectiles/BlankProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waterbolt");
        }


        public override void SafeSetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 390 && target.CanBeChasedBy(Projectile, false);
        }

        public override void AI()
        {

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[num].noGravity = true;
                    Dust obj = Main.dust[num];
                    obj.velocity *= 0f;
                }
            }
            if (Projectile.timeLeft < 390)
            {
                float homingVelocity = 12f;
                float N = 20f;
                NPC locatedTarget = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 1500f).OrderBy(n => Projectile.Distance(n.Center)).FirstOrDefault();

                if (locatedTarget != null)
                {
                    Vector2 homeDirection = Utils.SafeNormalize(locatedTarget.Center - Projectile.Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * N + homeDirection * homingVelocity) / (N + 1f);
                    return;
                }
            }
        }
    }
}
