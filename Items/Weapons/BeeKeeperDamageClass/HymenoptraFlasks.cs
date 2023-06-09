using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HymenoptraFlasks : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Throws a variety of bee related flasks\n'Might wanna wear some goggles'");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 60;
            Item.noMelee = true;
            Item.width = 25;
            Item.height = 25;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.reuseDelay = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 11, 75, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<HymenoptraFlask_Honey>();
            Item.shootSpeed = 15f;


            beeResourceCost = 3;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));

            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HymenoptraFlask_Nectar>(), ModContent.ProjectileType<HymenoptraFlask_Stinger>() });
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundID.Item106.PlayWith(player.Center, pitchVariance: 0.1f);
            player.UseBeeResource(3);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ToxicFlask, 1).
                AddIngredient(ItemID.BottledHoney, 5).
                AddIngredient(ItemID.Stinger, 10).
                AddIngredient(ModContent.ItemType<Pollen>(), 35).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}