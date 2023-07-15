using BombusApisBee.Items.Other.Crafting;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class LivingFlowerCrown : BeeKeeperItem
    {
        public int FlowerTimer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Flower Crown");
            Tooltip.SetDefault("3% increased hymenoptra critical strike chance\nIncreases maximum honey by 15");
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 50);

            Item.rare = ItemRarityID.White;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LivingFlowerChestplate>() && legs.type == ModContent.ItemType<LivingFlowerLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "'Get it? Cause yaknow, bees like flowers'\nSpawns dayblooms every 2 seconds, with a maximum of 3 dayblooms\n4% chance to not consume honey per daybloom alive\nWhile you have 3 dayblooms alive, double tap " + (Main.ReversedUpDownArmorSetBonuses ? "Up " : "Down ") + "to fire them toward the cursor, leeching honey on hit";
            player.Bombus().LivingFlower = true;
            for (int i = 0; i < player.ownedProjectileCounts[ModContent.ProjectileType<DaybloomProj>()]; i++)
            {
                player.Hymenoptra().ResourceChanceAdd += 0.04f;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<DaybloomProj>()] < 3 && FlowerTimer >= 120)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ModContent.ProjectileType<DaybloomProj>(), (int)player.ApplyHymenoptraDamageTo(10), 1f, player.whoAmI, GetFlowerInt(player));
                FlowerTimer = 0;
            }

            if (player.ownedProjectileCounts<DaybloomProj>() < 3)
                FlowerTimer++;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeCrit(3);
            player.Hymenoptra().BeeResourceMax2 += 15;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddIngredient(ModContent.ItemType<Pollen>(), 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public int GetFlowerInt(Player player)
        {
            if (!Main.projectile.Any(n => n.owner == player.whoAmI && n.type == ModContent.ProjectileType<DaybloomProj>() && n.active && n.ai[0] == 0))
                return 0;
            else if (!Main.projectile.Any(n => n.owner == player.whoAmI && n.type == ModContent.ProjectileType<DaybloomProj>() && n.active && n.ai[0] == 1))
                return 1;

            return 2;
        }
    }
}