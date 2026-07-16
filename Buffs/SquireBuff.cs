using BombusApisBee.Content.Hallow.Items.BeeSquireShield;

namespace BombusApisBee.Buffs
{


    public class SquireBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SquireBee>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}