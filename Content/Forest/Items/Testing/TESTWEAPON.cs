using BombusApisBee.Content.Hell.Items.HellcombShard;
using BombusApisBee.Core.BeekeeperClass;
using BombusApisBee.Core.Systems.ParticleSystem;
using Terraria;
using Terraria.DataStructures;

namespace BombusApisBee.Content.Forest.Items.Testing
{
    public class TESTWEAPON : BeekeeperWeapon
    {
        public override void SafeSetStaticDefaults()
        {
            DisplayName.SetDefault("Test Honeycomb");
            Tooltip.SetDefault("TEST DONT USE");

            SHOWCASEMODE = true;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 1;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = 10000;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<HellcombShardExplosion>();
            Item.shootSpeed = 10;
            Item.UseSound = SoundID.Item11;
            Item.scale = 1;
            Item.crit = 4;
            SHOWCASEMODE = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.Beekeeper().SHOWCASEMODETIMER = 0;
            Vector2 pos = Main.MouseWorld;

            ParticleHandler.SpawnParticle(new StarImpactParticle(pos, Color.Yellow with { A = 0 }, new(0.5f, 0.3f), new(2.5f, 0.1f), 120));
            return false;
        }

        public override void HoldItem(Player player)
        {
            player.Beekeeper().SHOWCASEMODETIMER = 6000;
            player.Beekeeper().BeeResourceReserved = 0;
            player.IncreaseBeeCrit(100);
        }
    }

    public class TestAccessory : BeekeeperAccessory
    {
        public TestAccessory() : base("Test Accessory", "For debugging purposes") { }
        public override void SafeUpdateEquip(Player Player)
        {
            Player.Beekeeper().BeeResourceCurrent = 100;
            Player.IncreaseBeeDamage(-0.8f);
            Player.Beekeeper().SHOWCASEMODETIMER = 20;
            
            if (Player.HeldItem.ModItem is BeekeeperWeapon)
               (Player.HeldItem.ModItem as BeekeeperWeapon).SHOWCASEMODE = true;
        }
    }
}