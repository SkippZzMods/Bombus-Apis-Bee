using BombusApisBee.BeeDamageClass;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace BombusApisBee.Items.Other.OnPickupItems
{
    public class HoneycombChunkPickup : ModItem
    {
        public string TextureString = "BombusApisBee/Items/Other/OnPickupItems/HoneycombChunkPickup_1";
        public override ModItem Clone(Item newEntity)
        {
            ModItem clone = base.Clone(newEntity);
            (clone as HoneycombChunkPickup).TextureString = this.TextureString;

            return clone;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Chunk");
            Tooltip.SetDefault("you shouldn't see this....");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player)
        {
            return false;
        }

        public override bool OnPickup(Player player)
        {
            BeeDamagePlayer mp = player.Hymenoptra();

            HoneycombChunkPlayer hp = player.GetModPlayer<HoneycombChunkPlayer>();

            if (mp.HasBees)
            {
                switch (mp.CurrentBeeState)
                {
                    case (int)BeeDamagePlayer.BeeState.Defense:
                        hp.DefenseStacks++;
                        hp.DefenseStackDecayTimer = 0;
                        break;

                    case (int)BeeDamagePlayer.BeeState.Offense:
                        hp.OffenseStacks++;
                        hp.OffenseStackDecayTimer = 0;
                        break;

                    case (int)BeeDamagePlayer.BeeState.Gathering:
                        player.IncreaseBeeResource(2);

                        hp.GatheringStacks++;
                        hp.GatheringStackDecayTimer = 0;
                        break;
                }
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("BombusApisBee/ExtraTextures/GlowAlpha").Value;

            Texture2D tex = ModContent.Request<Texture2D>(TextureString).Value;

            spriteBatch.Draw(tex, Item.position - Main.screenPosition, null, lightColor, 0f, tex.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(glowTex, Item.position - Main.screenPosition, null, new Color(255, 150, 20, 0) * MathHelper.Lerp(.5f, 1f, Utils.Clamp((float)Math.Sin(1f + Main.GlobalTimeWrappedHourly * 4f), 0, 1)), 0f, glowTex.Size() / 2f, 0.4f, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class HoneycombChunkPlayer : ModPlayer
    {
        public int GatheringStacks;

        public int GatheringStackDecayTimer;

        public int OffenseStacks;

        public int OffenseStackDecayTimer;

        public int DefenseStacks;

        public int DefenseStackDecayTimer;

        public int MaxStacks = 10;

        public bool AnyStacks => (GatheringStacks > 0) || (OffenseStacks > 0) || (DefenseStacks > 0);

        public override void ResetEffects()
        {
            if (GatheringStacks > 0)
            {
                GatheringStackDecayTimer++;
                if (GatheringStackDecayTimer > 300)
                {
                    GatheringStacks--;
                    GatheringStackDecayTimer = 0;
                }
            }
            else
                GatheringStackDecayTimer = 0;

            if (OffenseStacks > 0)
            {
                OffenseStackDecayTimer++;
                if (OffenseStackDecayTimer > 300)
                {
                    OffenseStacks--;
                    OffenseStackDecayTimer = 0;
                }
            }
            else
                OffenseStackDecayTimer = 0;

            if (DefenseStacks > 0)
            {
                DefenseStackDecayTimer++;
                if (DefenseStackDecayTimer > 300)
                {
                    DefenseStacks--;
                    DefenseStackDecayTimer = 0;
                }
            }
            else
                DefenseStackDecayTimer = 0;

            GatheringStacks = Utils.Clamp(GatheringStacks, 0, MaxStacks);

            DefenseStacks = Utils.Clamp(DefenseStacks, 0, MaxStacks);

            OffenseStacks = Utils.Clamp(OffenseStacks, 0, MaxStacks);

            MaxStacks = 10;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.CountsAsClass<HymenoptraDamageClass>() && Main.rand.NextFloat() < 0.05f)
            {
                Item item = Main.item[Item.NewItem(target.GetSource_OnHurt(proj), target.getRect(), ModContent.ItemType<HoneycombChunkPickup>())];

                item.noGrabDelay = 60;

                (item.ModItem as HoneycombChunkPickup).TextureString = "BombusApisBee/Items/Other/OnPickupItems/HoneycombChunkPickup_" + Main.rand.Next(1, 5);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI, 1f);
                }
            }
        }

        public override void UpdateEquips()
        {
            Player.Hymenoptra().BeeResourceIncrease += GatheringStacks * 2;

            Player.statDefense += DefenseStacks;

            Player.IncreaseBeeCrit(OffenseStacks);
            Player.GetArmorPenetration<HymenoptraDamageClass>() += OffenseStacks;

            if (AnyStacks && !Player.HasBuff<HiveBlessingBuff>())
                Player.AddBuff<HiveBlessingBuff>(120);
        }
    }

    public class HiveBlessingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Blessing");
            Description.SetDefault("You are blessed by the Hive");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            HoneycombChunkPlayer hp = player.GetModPlayer<HoneycombChunkPlayer>();

            if (hp.AnyStacks)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            HoneycombChunkPlayer hp = Main.LocalPlayer.GetModPlayer<HoneycombChunkPlayer>();

            rare = ItemRarityID.Yellow;

            tip = "You have been blessed by the Hive!:\n" +
                "Stacks of Sweetness: " + hp.GatheringStacks + ", Effects: \n" + "Honey regeneration per second increased by " + hp.GatheringStacks * 2 + "\n" +
                "Stacks of Shielding: " + hp.DefenseStacks + ", Effects: \n" + "Defense increased by " + hp.DefenseStacks + "\n" +
                "Stacks of Strength: " + hp.OffenseStacks + ", Effects: \n" + "Hymenoptra critical strike chance increased by " + hp.OffenseStacks + "%\n" +
                "Hymenoptra armor penetration increased by " + hp.OffenseStacks + "\n";
        }
    }
}
