namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class SkeletalBeeHelmet : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("10% increased hymenoptra damage and critical strike chance\nIncreases maximum honey by 30");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SkeletalBeeChestplate>() && legs.type == ModContent.ItemType<SkeletalBeeLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Summons a skeletal hornet to protect you\nGetting a kill or critical hit enrages the hornet for a short time\nGetting a kill or critical hit while the hornet is already enraged increases the amount of time it is enraged for";
            player.IncreaseBeeDamage(0.07f);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletalHornetProjectile>()] <= 0 && Main.myPlayer == player.whoAmI)
            {
                int whoami = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.UnitY, ModContent.ProjectileType<SkeletalHornetProjectile>(), player.ApplyHymenoptraDamageTo(18), 2f, player.whoAmI);
                player.Bombus().SkeletalHornetWhoAmI = whoami;
            }

            player.Bombus().SkeletalSet = true;

        }

        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.1f);
            player.IncreaseBeeCrit(10);
            var modPlayer = BeeDamagePlayer.ModPlayer(player);
            modPlayer.BeeResourceMax2 += 30;
        }


        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Bone, 13).AddIngredient(ItemID.BeeWax, 6).AddIngredient(ItemID.HellstoneBar, 8).AddTile(TileID.Anvils).Register();
        }
    }
}
