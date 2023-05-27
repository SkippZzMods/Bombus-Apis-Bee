using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class TrueStingerScimitar : BeeDamageItem
    {
        public override void SafeSetDefaults()
        {
            Item.damage = 50;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StingerFriendly>();
            Item.shootSpeed = 22f;
            Item.UseSound = SoundID.Item1;
            Item.scale = 1.25f;
            Item.crit = 4;
            beeResourceCost = 3;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 4 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage / 3, knockback, player.whoAmI);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300, false);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BrokenHeroSword, 1).AddIngredient(ModContent.ItemType<StingerScimitar>()).AddIngredient(ModContent.ItemType<Pollen>(), 25).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}