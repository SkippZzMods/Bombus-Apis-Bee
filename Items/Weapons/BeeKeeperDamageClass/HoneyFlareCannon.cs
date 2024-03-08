using Terraria.DataStructures;


namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class HoneyFlareCannon : BeeDamageItem
    {
        public override void SafeSetStaticDefaults()
        {
            Tooltip.SetDefault("Rapidly fires honey flares");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 50;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 25;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HoneyFlare>();
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item11;
            honeyCost = 2;
            resourceChance = 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<HoneyFlareGun>(), 1).AddIngredient(ItemID.HallowedBar, 14).AddTile(TileID.MythrilAnvil).Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(6.AsRadians());
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.HoneyDust>(), velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
                Dust.NewDustPerfect(position, DustID.Honey2, velocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.25f), Main.rand.Next(100)).noGravity = true;
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }
    }
}
