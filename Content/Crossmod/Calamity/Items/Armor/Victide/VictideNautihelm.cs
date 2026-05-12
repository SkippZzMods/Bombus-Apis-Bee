using BombusApisBee.Content.Forest.Items.Pollen;
using CalamityMod;
using CalamityMod.Items.Armor.Victide;
using CalamityMod.Items.Materials;

namespace BombusApisBee.Content.Crossmod.Calamity.Items.Armor.Victide
{
    [JITWhenModsEnabled("CalamityMod")]
    [AutoloadEquip(EquipType.Head)]
    class VictideNautihelm : BeeKeeperItem
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Nauti-helm");
            Tooltip.SetDefault("5% increased beekeeper damage");
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<VictideBreastplate>() && legs.type == ItemType<VictideGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases maximum honey by 30\n+3 life regen and 10% increased beekeeper damage when submerged in liquid\n" +
                "15% further increased beekeeper damage when submerged in honey\n" +
                "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                "This seashell does true damage and does not benefit from any damage class\n" +
                "Provides increased underwater mobility and slightly reduces breath loss in the abyss";

            player.Beekeeper().BeeResourceMax2 += 30;

            if (player.honeyWet)
                player.IncreaseBeeDamage(0.15f);

            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir, false))
            {
                player.IncreaseBeeDamage(0.1f);
                player.lifeRegen += 3;
            }

            player.ignoreWater = true;
            player.Calamity().victideSet = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<SeaRemains>(), 3);
            recipe.AddIngredient(ItemType<PollenItem>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
