using BombusApisBee.Core.Systems.ParticleSystem;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace BombusApisBee.Content.Jungle.Items.RoyalJelly
{
    public class RoyalJelly : ModItem
    {
        bool useAlternateTexture = false;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Permanently increases maximum honey by 25\n'A delicacy that's highly sought after for its refined taste and healing properties'");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 24;

            Item.consumable = true;

            Item.rare = ItemRarityID.Orange;

            Item.useStyle = ItemUseStyleID.EatFood;

            Item.value = Item.sellPrice(gold: 2);

            Item.useTime = Item.useAnimation = 20;

            Item.UseSound = SoundID.Item3;
        }

        public override bool? UseItem(Player player)
        {
            if (player.Bombus().RoyalJelly)
                return null;

            player.Bombus().RoyalJelly = true;

            CombatText.NewText(player.getRect(), BombusApisBee.honeyIncreaseColor, 25);

            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            useAlternateTexture = Main.rand.NextBool(5);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var tex = useAlternateTexture ? ModContent.Request<Texture2D>(Texture + "_Alternate").Value : ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, position, frame, drawColor, 0f, origin, scale, 0f, 0f);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var tex = useAlternateTexture ? ModContent.Request<Texture2D>(Texture + "_Alternate").Value : ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, lightColor, rotation, tex.Size() / 2f, scale, 0f, 0f);

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (useAlternateTexture)
            {
                if (Main.rand.NextBool(60))
                {
                    ParticleHandler.SpawnParticle(new StarParticle(Item.Center + Main.rand.NextVector2Circular(15f, 15f), Vector2.Zero, Color.Red, Color.Yellow, 0.4f, 50)
                    {
                        Rotation = 0f,
                        Layer = Core.Systems.PixelationSystem.RenderLayer.AboveItems
                    });
                }           
            }
            else if (Main.rand.NextBool(120))
            {
                ParticleHandler.SpawnParticle(new StarParticle(Item.Center + Main.rand.NextVector2Circular(15f, 15f), Vector2.Zero, Color.LightYellow, Color.Orange, 0.4f, 50)
                {
                    Rotation = 0f,
                    Layer = Core.Systems.PixelationSystem.RenderLayer.AboveItems
                });
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["RoyalJellyAlternateTexture"] = useAlternateTexture;
        }

        public override void LoadData(TagCompound tag)
        {
            useAlternateTexture = tag.GetBool("RoyalJellyAlternateTexture");
        }
    }
}
