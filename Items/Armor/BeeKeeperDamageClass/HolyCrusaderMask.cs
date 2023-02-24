using BombusApisBee.Items.Other.Crafting;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;

namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class HolyCrusaderMask : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("5% increased damage reduction and hymenoptra damage\nIncreases maximum honey by 40\nIncreases your amount of Bees by 1");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 14;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ModContent.ItemType<HolyCrusaderGreaves>() && body.type == ModContent.ItemType<HolyCrusaderArmor>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Taking over 50 damage in one hit causes a holy barrier to form around you\nThis holy barrier makes the next hit deal 50% less damage\n" +
                "This effect has a 15 second cooldown";
            player.GetModPlayer<HolyCrusaderPlayer>().FullArmorSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.05f);
            player.endurance += 0.05f;
            player.Hymenoptra().BeeResourceMax2 += 40;
            player.Hymenoptra().CurrentBees += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 18).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient(ModContent.ItemType<Pollen>(), 25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }

    class HolyCrusaderPlayer : ModPlayer
    {
        public bool FullArmorSet;

        private bool HolyShield;
        private int HolyShieldCooldown;

        public override void Load()
        {
            On.Terraria.Main.DrawInfernoRings += DrawShield;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawInfernoRings -= DrawShield;
        }

        private void DrawShield(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].outOfRange && !Main.player[i].dead)
                {
                    HolyCrusaderPlayer modPlayer = Main.player[i].GetModPlayer<HolyCrusaderPlayer>();
                    if (modPlayer.HolyShield)
                    {
                        Effect effect = Filters.Scene["HolyShieldShader"].GetShader().Shader;
                        effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
                        effect.Parameters["blowUpPower"].SetValue(2f);
                        effect.Parameters["blowUpSize"].SetValue(0.4f);

                        float noiseScale = MathHelper.Lerp(0.5f, 1f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.2f) * 0.4f + 0.4f);
                        effect.Parameters["noiseScale"].SetValue(noiseScale);
                        float opacity = MathHelper.Lerp(0.8f, 1f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.4f) * 0.6f + 0.6f);
                        effect.Parameters["shieldOpacity"].SetValue(opacity);
                        effect.Parameters["shieldEdgeBlendStrenght"].SetValue(4.5f);

                        effect.Parameters["shieldColor"].SetValue(new Color(225, 220, 125).ToVector3());
                        effect.Parameters["shieldColor"].SetValue(Color.Lerp(new Color(225, 220, 125), new Color(200, 165, 80), (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.15f)).ToVector3());

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                        Texture2D tex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/HolyNoise").Value;
                        Vector2 pos = new Vector2(Main.player[i].Center.X, Main.player[i].Center.Y + Main.player[i].gfxOffY) - Main.screenPosition;
                        Main.spriteBatch.Draw(tex, pos, null, Color.White, 0f, tex.Size() / 2f, MathHelper.Lerp(0.3f, 0.35f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f) * 0.6f + 0.6f), 0, 0f);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                    }
                    if (modPlayer.HolyShieldCooldown > 0)
                    {
                        if (PlayerRenderTarget.canUseTarget)
                        {
                            Main.spriteBatch.End();
                            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
                            Effect effect = Filters.Scene["MarkedEffect"].GetShader().Shader;
                            effect.Parameters["uImageSize0"].SetValue(PlayerRenderTarget.Target.Size());
                            float alpha = MathHelper.Lerp(1f, 0.5f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f));
                            effect.Parameters["alpha"].SetValue(alpha);
                            effect.Parameters["colorOne"].SetValue(new Color(225, 220, 125).ToVector4());
                            effect.Parameters["colorTwo"].SetValue(new Color(200, 165, 80).ToVector4());

                            effect.Parameters["whiteness"].SetValue(1f);

                            effect.CurrentTechnique.Passes[0].Apply();
                            Main.spriteBatch.Draw(PlayerRenderTarget.Target, new Rectangle((int)PlayerRenderTarget.getPlayerTargetPosition(i).X, (int)PlayerRenderTarget.getPlayerTargetPosition(i).Y, (int)PlayerRenderTarget.Target.Size().X, (int)PlayerRenderTarget.Target.Size().Y), Color.White);
                            Main.spriteBatch.End();
                            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                        }
                    }
                }
            }
            orig.Invoke(self);
        }

        public override void ResetEffects()
        {
            if (!FullArmorSet)
                HolyShield = false;

            FullArmorSet = false;
            if (HolyShieldCooldown > 0)
            {
                HolyShieldCooldown--;
                if (HolyShieldCooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.1f }, Player.Center);
                    for (int i = 0; i < 35; i++)
                    {
                        Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<Dusts.GlowFastDecelerate>(), 0, 0, 25,
                            new Color(225, 220, 125), Main.rand.NextFloat(0.4f, 0.6f)).velocity = Vector2.One.RotatedByRandom(6.28f) * 3f;
                    }
                }
            }
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (FullArmorSet)
            {
                if (!HolyShield)
                {
                    if (damage > 50 && HolyShieldCooldown <= 0)
                        HolyShield = true;
                }
                else
                {
                    HolyShield = false;
                    damage = damage / 2;
                    CombatText.NewText(Player.getRect(), new Color(225, 220, 125), (int)Main.CalculateDamagePlayersTake(damage, Player.statDefense));
                    SoundEngine.PlaySound(SoundID.NPCDeath6, Player.position);
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(Player.Center, ModContent.DustType<Dusts.GlowFastDecelerate>(), Vector2.One.RotatedByRandom(6.28f) * 3f, 25,
                            new Color(225, 220, 125), Main.rand.NextFloat(0.5f, 0.65f));

                        Dust.NewDustDirect(Player.position, Player.width, Player.height, ModContent.DustType<Dusts.Glow>(), 0, 0, 25,
                            new Color(225, 220, 125), Main.rand.NextFloat(0.5f, 0.65f)).velocity = Vector2.One.RotatedByRandom(6.28f) * 3f;
                    }
                    HolyShieldCooldown = 15 * 60;
                }
            }
            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource, ref cooldownCounter);
        }
    }
}

