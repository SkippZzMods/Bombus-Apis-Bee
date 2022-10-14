using BombusApisBee.BeeDamageClass;
using BombusApisBee.Buffs;
using Microsoft.Xna.Framework.Graphics;

namespace BombusApisBee.Items.Other.OnPickupItems
{
    public class AstralStarPickup : ModItem
    {
        public override string Texture => "BombusApisBee/Projectiles/AstralStarSplitting";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Star");
            Tooltip.SetDefault("you shouldn't see this....");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
        }
        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange = 100;
        }
        public override bool ItemSpace(Player player)
        {
            return false;
        }
        public override bool OnPickup(Player player)
        {
            if (player.Hymenoptra().BeeResourceCurrent < player.Hymenoptra().BeeResourceMax2)
                player.GetModPlayer<BeeDamagePlayer>().BeeResourceCurrent += 6;
            if (player.Hymenoptra().BeeResourceCurrent > player.Hymenoptra().BeeResourceMax2)
                player.Hymenoptra().BeeResourceCurrent = player.Hymenoptra().BeeResourceMax2;

            CombatText.NewText(player.getRect(), BombusApisBee.honeyIncreaseColor, 6, false, false);

            BeeUtils.DrawStar(Item.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0, new Color(181, 127, 207), true, 5, 0.85f, 1f, 0.4f, 0.3f);

            player.AddBuff<AstralStarBuff>(240);
            return false;
        }   

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D bloomTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("BombusApisBee/Projectiles/AstralStar_Glow").Value;

            Main.spriteBatch.Draw(bloomTex, Item.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.65f, 0f, bloomTex.Size() / 2f, 0.65f, 0, 0);
            Main.spriteBatch.Draw(texGlow, Item.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.5f, rotation, texGlow.Size() / 2f, scale, 0f, 0f);
            Main.spriteBatch.Draw(bloomTex, Item.Center - Main.screenPosition, null, new Color(181, 127, 207, 0) * 0.65f, 0f, bloomTex.Size() / 2f, 0.65f, 0, 0);
        }
    }
}
