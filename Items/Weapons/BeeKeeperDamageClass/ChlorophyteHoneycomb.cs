﻿using BombusApisBee.BeeDamageClass;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class ChlorophyteHoneycomb : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Chloro-comb");
            Tooltip.SetDefault("Throws a fragile honeycomb which shatters into homing fragments, chloro-spores, and chloro-bees\nChloro-bees materialize into chloro-energy upon death");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 49;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 20;

            Item.useTime = 65;
            Item.useAnimation = 65;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Lime;

            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ChlorophyteHoneycombProjectile>();
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.noUseGraphic = true;

            beeResourceCost = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 12).AddIngredient(ModContent.ItemType<Pollen>(), 20).AddTile(TileID.MythrilAnvil).Register();

        }
    }
}