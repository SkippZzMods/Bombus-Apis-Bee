using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.PixelationSystem;

namespace BombusApisBee.Core.Systems.ParticleSystem
{
    internal partial class ParticleHandler : ILoadable
    {
        private void DrawItemParticles(On_Main.orig_DrawItems orig, Main self)
        {
            orig(self);

            SmokeTargetSystem.DrawCompositeSmoke(6, false);

            DrawAllParticles(Main.spriteBatch, RenderLayer.AboveItems);
        }

        private void DrawDustParticles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            
            SmokeTargetSystem.DrawCompositeSmoke(5, true);

            DrawAllParticles(Main.spriteBatch, RenderLayer.Dusts);
        }

        private void DrawParticles(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            SpriteBatch sb = Main.spriteBatch;

            orig(self, projCache, startSpriteBatch);

            if (startSpriteBatch)
                sb.BeginDefault();

            foreach (Particle particle in Particles)
            {
                if (particle is null || !particle.drawPixellated)
                    continue;

                RenderLayer layer = particle.PixelLayer;

                string key = "UnderTiles";

                switch (layer)
                {
                    case RenderLayer.UnderNPCs:
                        key = "UnderNPCs";
                        break;
                    case RenderLayer.UnderProjectiles:
                        key = "UnderProjectiles";
                        break;
                    case RenderLayer.OverPlayers:
                        key = "OverPlayers";
                        break;
                    case RenderLayer.Dusts:
                        key = "Dusts";
                        break;
                    case RenderLayer.OverWiresUI:
                        key = "OverWiresUI";
                        break;
                }

                GetInstance<PixelationSystem.PixelationSystem>().QueueRenderAction(key, () =>
                {
                    particle.PixelatedDraw(sb);
                });
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCsAndTiles))
            {
                SmokeTargetSystem.DrawCompositeSmoke(0, !startSpriteBatch);

                DrawAllParticles(sb, RenderLayer.UnderTiles);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCs))
            {
                SmokeTargetSystem.DrawCompositeSmoke(1, !startSpriteBatch);

                DrawAllParticles(sb, RenderLayer.UnderNPCs);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindProjectiles))
            {
                SmokeTargetSystem.DrawCompositeSmoke(2, !startSpriteBatch);

                DrawAllParticles(sb, RenderLayer.UnderProjectiles);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverPlayers))
            {
                SmokeTargetSystem.DrawCompositeSmoke(3, !startSpriteBatch);

                DrawAllParticles(sb, RenderLayer.OverPlayers);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverWiresUI))
            {
                SmokeTargetSystem.DrawCompositeSmoke(4, startSpriteBatch);

                DrawAllParticles(sb, RenderLayer.OverWiresUI);
            }

            if (startSpriteBatch)
                sb.End();
        }
    }
}
