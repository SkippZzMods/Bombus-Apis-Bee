using BombusApisBee.Dusts.Pixelized;
using BombusApisBee.Items.Other.Crafting;
using Terraria.DataStructures;

namespace BombusApisBee.Items.Weapons.BeeKeeperDamageClass
{
    public class WoodenApiary : ApiaryItem
    {
        public override void AddStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Apiary");
            Tooltip.SetDefault("Hold <left> to rapidly fire bees\nHold <right> to fire bees slower, but take control over the bees causing them to deal an additonal 3 true damage");
        }

        public override void AddDefaults()
        {
            Item.damage = 5;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 14;
            Item.useAnimation = 14;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.15f;

            Item.value = Item.sellPrice(silver: 1);

            Item.rare = ItemRarityID.White;

            Item.autoReuse = true;

            Item.shoot = ProjectileType<WoodenApiaryHoldout>();

            Item.shootSpeed = 6f;

            Item.UseSound = SoundID.Item11;

            honeyCost = 1;
            altHoneyCost = 2;
        }

        public override bool SafeCanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 28;
                Item.useAnimation = 28;

            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;

            }

            return base.SafeCanUseItem(player);
        }

        public override void ModifyApiaryHit(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage.Base += 3;
        }

        public override void HoldAI(Projectile Projectile)
        {
            base.HoldAI(Projectile);

            if (Main.rand.NextBool(90))
            {
                Color color = Color.Lerp(new Color(255, 150, 0, 0), new Color(255, 225, 0, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), DustType<PixelStar>(), Vector2.Zero, 0, color, 0.15f);
            }
        }

        public override void PostDrawApiaryBees(Projectile projectile, Color lightColor, bool active)
        {
            Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Player player = Main.player[projectile.owner];

            int holdTimer = player.GetModPlayer<ApiaryPlayer>().holdTimer;

            Color color = Color.Lerp(new Color(255, 150, 0, 0), new Color(255, 225, 0, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));

            if (holdTimer > 0)
                Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, color * (holdTimer / 20f) * 0.2f, 0f, tex.Size() / 2f, 0.25f, 0, 0f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Pollen>(5).
                AddIngredient(ItemID.Wood, 30).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }

    public class WoodenApiaryHoldout : ApiaryHoldout
    {
        public override Color GlowColor => Color.Lerp(new Color(255, 180, 0), new Color(255, 225, 0), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f));
        public override string Texture => "BombusApisBee/Items/Weapons/BeeKeeperDamageClass/WoodenApiary";

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
