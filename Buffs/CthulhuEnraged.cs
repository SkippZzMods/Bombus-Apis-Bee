using BombusApisBee.Core.BeekeeperClass;

namespace BombusApisBee.Buffs
{
    public class CthulhuEnraged : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<BeekeeperDamageClass>() += 0.12f;
        }
    }
}
