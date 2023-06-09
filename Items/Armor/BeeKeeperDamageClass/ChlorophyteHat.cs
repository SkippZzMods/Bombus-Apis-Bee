namespace BombusApisBee.Items.Armor.BeeKeeperDamageClass
{
    [AutoloadEquip(EquipType.Head)]
    public class ChlorophyteHat : BeeKeeperItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chlorophyte Veil");
            Tooltip.SetDefault("16% increased hymenoptra damage\nIncreases maximum honey by 50\nIncreases your amount of Loyal Bees by 5");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 4;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return legs.type == ItemID.ChlorophyteGreaves && body.type == ItemID.ChlorophytePlateMail;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Striking enemies with hymenoptra projectiles has a chance to inflict them with a stack of Chloro-infested\nFor every stack of Chloro-infested inflicted, they take 3% more damage, up to 10 stacks\n" +
                "Critically striking an enemy with maximum stacks of Chloro-infested causes all the chlorophyte to materialize into Chloro-honey, getting rid of all of the stacks";
            player.Bombus().ChloroSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.IncreaseBeeDamage(0.14f);
            BeeDamagePlayer.ModPlayer(player).BeeResourceMax2 += 50;
            player.Hymenoptra().CurrentBees += 5;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.ChlorophyteBar, 13).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    class ChloroInfestedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int ChloroStacks;
        public int DebuffTime;

        public override void ResetEffects(NPC npc)
        {
            ChloroStacks = Utils.Clamp(ChloroStacks, 0, 10);
            if (DebuffTime > 0)
            {
                DebuffTime--;
                if (DebuffTime <= 0)
                    ChloroStacks = 0;
            }
        }

        public override void AI(NPC npc)
        {
            if (ChloroStacks > 0)
                if (Main.rand.NextBool(11 - ChloroStacks))
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.Glow>(), 0f, 0f, 25, new Color(161, 236, 0), Main.rand.NextFloat(0.3f, 0.5f));
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (ChloroStacks > 0)
                damage = (int)(damage * (1f + (0.03f * ChloroStacks)));
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ChloroStacks > 0)
                damage = (int)(damage * (1f + (0.03f * ChloroStacks)));
        }
    }
}
