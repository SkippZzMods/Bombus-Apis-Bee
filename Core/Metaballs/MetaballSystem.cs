using System.Threading;

namespace BombusApisBee.Core.Metaballs
{
    public class MetaballSystem : IOrderedLoadable
    {
		public static Semaphore actorsSem = new(1, 1);

		public static List<MetaballActor> actors = new();

		//We intentionally load after screen targets here so our extra RT swapout applies after the default ones.
		public float Priority => 1.1f;

		public void Load()
		{
			if (Main.dedServ)
				return;

			On_Main.DrawNPCs += DrawTargets;
			On_Main.CheckMonoliths += BuildTargets;
			On_Main.DrawDust += DrawDustTargets;
		}

		public void Unload()
		{
			On_Main.DrawNPCs -= DrawTargets;
			On_Main.CheckMonoliths -= BuildTargets;
			On_Main.DrawDust -= DrawDustTargets;

			actorsSem.WaitOne();
			actors = null;
			actorsSem.Release();
		}

		private void DrawTargets(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles = false)
		{
			if (behindTiles)
			{
				actorsSem.WaitOne();
				var toDraw = actors.Where(n => !n.OverEnemies).ToList();
				toDraw.ForEach(a => a.DrawTarget(Main.spriteBatch));
				actorsSem.Release();
			}

			orig(self, behindTiles);

			if (!behindTiles)
			{
				actorsSem.WaitOne();
				var toDraw = actors.Where(n => n.OverEnemies).ToList();
				toDraw.ForEach(a => a.DrawTarget(Main.spriteBatch));
				actorsSem.Release();
			}
		}

		private void BuildTargets(On_Main.orig_CheckMonoliths orig)
		{
			orig();

			if (!Main.gameMenu && Main.spriteBatch != null && Main.graphics.GraphicsDevice != null)
			{
				actorsSem.WaitOne();
				actors.ForEach(a => a.DrawToTarget(Main.spriteBatch, Main.graphics.GraphicsDevice));
				actorsSem.Release();
			}
		}

		private void DrawDustTargets(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);

            foreach (MetaballActor a in actors)
            {
                if (a.actAsDust)
                {
                    a.DrawDustTarget(Main.spriteBatch);
                }
            }
        }
    }
}
