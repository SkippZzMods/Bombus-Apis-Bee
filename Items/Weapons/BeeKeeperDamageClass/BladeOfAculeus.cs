using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class BladeOfAculeus : BeeDamageItem
    {
        private int combo = 0;

        int cooldown;
        public override bool AltFunctionUse(Player player) => cooldown <= 0;
        public override bool SafeCanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<AculeusBladeHoldout>()] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<AculeusBladeHoldoutAlt>()] <= 0;
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Aculeus");
            Tooltip.SetDefault("Performs a heavy combo of swings and stabs\nPress <right> to throw the blade, embedding itself in enemies\n" +
                "Press <right> whilst the blade is embedded to return it, performing a powerful dash afterwards\nRends the armor of enemies");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 43;
            Item.useAnimation = 43;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<AculeusBladeHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 3, silver: 25);
            Item.rare = ItemRarityID.Green;
            Item.Size = new Vector2(50);
        }
        public override void HoldItem(Player player)
        {
            if (cooldown > 0)
            {
                if (cooldown == 1)
                {
                    SoundID.MaxMana.PlayWith(player.Center);
                }
                cooldown--;
            }


        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AculeusBladeHoldoutAlt>(), (int)(damage * 1.5f), knockback, player.whoAmI);
                combo = 0;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, combo);
                combo++;
                if (combo > 4)
                {
                    combo = 0;
                }
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Stinger, 7).AddIngredient(ItemID.EnchantedSword).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddTile(TileID.Anvils).Register();
        }
    }
}