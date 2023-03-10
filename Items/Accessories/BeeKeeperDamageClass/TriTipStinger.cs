using BombusApisBee.BeeDamageClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace BombusApisBee.Items.Accessories.BeeKeeperDamageClass
{
    public class TriTipStinger : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tri-Tip Stinger");
            Tooltip.SetDefault("Grants a chance on hit for hymenoptra attacks to apply an improved poison\nThe improved poison deals a percentage of the inflicted enemies max hp, and spreads between enemies");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TriTipStingerPlayer>().equipped = true;
        }
    }

    class TriTipStingerPlayer : ModPlayer
    {
        public bool equipped;

        public override void ResetEffects()
        {
            equipped = false;
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (!equipped)
                return;

            if (item.CountsAsClass<HymenoptraDamageClass>() && Main.rand.NextBool(10))
            {
                target.GetGlobalNPC<TriTipStingerNPC>().inflictionTimer = 600;
                target.GetGlobalNPC<TriTipStingerNPC>().inflicted = true;
                target.GetGlobalNPC<TriTipStingerNPC>().lastPlayerInflictedWhoAmI = Player.whoAmI;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!equipped)
                return;

            if (proj.CountsAsClass<HymenoptraDamageClass>() && Main.rand.NextBool(10))
            {
                target.GetGlobalNPC<TriTipStingerNPC>().inflictionTimer = 600;
                target.GetGlobalNPC<TriTipStingerNPC>().inflicted = true;
                target.GetGlobalNPC<TriTipStingerNPC>().lastPlayerInflictedWhoAmI = Player.whoAmI;
            }
        }
    }

    class TriTipStingerNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool inflicted;

        public int inflictionTimer;

        int timer;

        public int lastPlayerInflictedWhoAmI = -1;

        public override void ResetEffects(NPC npc)
        {
            inflicted = false;
            if (inflictionTimer > 0)
            {
                inflictionTimer--;
                inflicted = true;
            }
            else
                lastPlayerInflictedWhoAmI = -1;
        }

        public override void AI(NPC npc)
        {
            if (npc.dontTakeDamage)
                return;

            if (inflicted)
            {
                doInflictedEffects(npc);
                if (++timer >= 30)
                {
                    int whoAmI = npc.whoAmI;
                    if (npc.realLife >= 0)
                        whoAmI = npc.realLife;

                    NPC n = Main.npc[whoAmI];

                    if (!n.immortal)
                    {
                        int amount = (int)Math.Round(npc.lifeMax * 0.005f);
                        if (amount < 10)
                            amount = 10;

                        if (amount > 100)
                            amount = 100;

                        n.life -= amount;
                        CombatText.NewText(n.getRect(), new Color(180, 255, 0), amount, true, true);
                        if (lastPlayerInflictedWhoAmI >= 0)
                            Main.player[lastPlayerInflictedWhoAmI].addDPS(amount);

                        if (n.life <= 0)
                        {
                            n.life = 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                n.StrikeNPCNoInteraction(9999, 0, 0);
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, whoAmI, 9999f);
                            }
                        }

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC nPC = Main.npc[i];

                            if (nPC.active && nPC != npc && !nPC.GetGlobalNPC<TriTipStingerNPC>().inflicted && npc.Distance(nPC.Center) < 200f)
                                if (Main.rand.NextBool(15))
                                {
                                    nPC.GetGlobalNPC<TriTipStingerNPC>().inflictionTimer = 300;
                                    nPC.GetGlobalNPC<TriTipStingerNPC>().inflicted = true;
                                    nPC.GetGlobalNPC<TriTipStingerNPC>().lastPlayerInflictedWhoAmI = this.lastPlayerInflictedWhoAmI;
                                }
                        }
                    }

                    timer = 0;
                }
            }
            else
                timer = 0;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (inflicted)
                drawColor = new Color(80, 255, 20);
        }

        void doInflictedEffects(NPC npc)
        {
            float scale;
            if (npc.width > npc.height)
                scale = npc.width / 32f;
            else if (npc.height > npc.width)
                scale = npc.height / 32f;
            else
                scale = npc.width / 32f;

            if (scale < 1f)
                scale = 1f;


            if (Main.rand.NextBool(10))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2CircularEdge(npc.width / 2, npc.height / 2), ModContent.DustType<TriTipStingerDust>(), Vector2.Zero, 0, default, Main.rand.NextFloat(scale, scale * 1.5f)).noGravity = true;
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), ModContent.DustType<TriTipStingerDust>(), Vector2.Zero, 0, default, Main.rand.NextFloat(scale, scale * 1.5f)).noGravity = true;
            }    

            if (Main.rand.NextBool(15))
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2), ModContent.DustType<TriTipStingerDust>(), Vector2.UnitY * Main.rand.NextFloat(), 0, default, Main.rand.NextFloat(scale * 0.5f, scale * 2f));
        }
    }
}
