using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace BombusApisBee.Core
{
    public class NectarVialCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
		public bool CanDrop(DropAttemptInfo info)
		{
			return info.player.ZoneJungle && info.npc.lifeMax > 5 && info.npc.HasPlayerTarget && !info.npc.friendly && info.npc.value > 0f && !info.IsInSimulation;
		}

		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Drops from Jungle enemies";
		
	}

	public class TriTipStingerCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
		List<int> hornets = new() { NPCID.Hornet, NPCID.HornetHoney, NPCID.HornetFatty, NPCID.HornetLeafy, NPCID.HornetSpikey, NPCID.HornetStingy};
		public bool CanDrop(DropAttemptInfo info)
		{
			return NPC.downedQueenBee && hornets.Contains(info.npc.type) && info.npc.HasPlayerTarget && !info.IsInSimulation;
		}

		public bool CanShowItemDropInUI() => true;
		public string GetConditionDescription() => "Drops from Hornets";
	}
}
