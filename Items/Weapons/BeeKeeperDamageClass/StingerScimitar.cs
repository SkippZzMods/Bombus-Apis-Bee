using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class StingerScimitar : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            // DisplayName.SetDefault("Honeycomb"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Fires a random amount of stingers when swung");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 25;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(gold: 2, silver: 75);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.HornetStinger;
            Item.shootSpeed = 17f;
            Item.UseSound = SoundID.Item1;
            Item.scale = 1.25f;
            Item.crit = 4;
            beeResourceCost = 1;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 2 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage * 1 / 3, knockback, player.whoAmI);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300, false);
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Stinger, 7).AddIngredient(ItemID.EnchantedSword).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();
        }
    }
}