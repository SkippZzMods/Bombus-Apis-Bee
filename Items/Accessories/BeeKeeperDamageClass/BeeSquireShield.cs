using BombusApisBee.BeeDamageClass;
using BombusApisBee.Buffs;
using BombusApisBee.Items.Other.Crafting;
using BombusApisBee.Projectiles;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class BeeSquireShield : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Squire Shield");
            Tooltip.SetDefault("'The most noble of bee squires shall protect you!'\nSummons a bee squire that hovers near you, firing arrows at nearby enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3);
            Item.defense = 5;
            Item.buffType = ModContent.BuffType<SquireBuff>();
            Item.damage = 45;
            Item.DamageType = ModContent.GetInstance<HymenoptraDamageClass>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<SquireBuff>(), 20, true);
            var modPlayer = player.GetModPlayer<BombusApisBeePlayer>();
            modPlayer.squire = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SquireBee>()] < 1)
            {
                Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), player.Center, Vector2.One, ModContent.ProjectileType<SquireBee>(), (int)player.GetTotalDamage<HymenoptraDamageClass>().ApplyTo(Item.damage), 1, player.whoAmI)
                    .originalDamage = Item.damage;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.IronBar, 15).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddIngredient(ItemID.UnicornHorn, 3).AddTile(TileID.MythrilAnvil).Register();

            CreateRecipe(1).AddIngredient(ItemID.LeadBar, 15).AddIngredient(ModContent.ItemType<Pollen>(), 15).AddIngredient(ItemID.UnicornHorn, 3).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}