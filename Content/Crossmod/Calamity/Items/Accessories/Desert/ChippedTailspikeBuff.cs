namespace BombusApisBee.Content.Crossmod.Calamity.Items.Accessories.Desert
{
    [JITWhenModsEnabled("CalamityMod")]
    public class ChippedTailspikeBuff : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => CrossMod.Calamity.Enabled;
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.IncreaseBeeCrit(25);
        }
    }
}
