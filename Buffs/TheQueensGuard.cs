using BombusApisBee.Projectiles;

namespace BombusApisBee.Buffs
{
    public class TheQueensGuard : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Queen's Guard");
            Description.SetDefault("The Queen will protect you!");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TheQueen>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

