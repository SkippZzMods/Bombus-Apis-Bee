using BombusApisBee.Core.Helpers;
using BombusApisBee.Core.Systems.PixelationSystem;

namespace BombusApisBee.Core.Systems.ParticleSystem
{
    internal partial class ParticleHandler : ILoadable
    {
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
                DrawAllParticles(sb, RenderLayer.UnderTiles);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindNPCs))
            {
                DrawAllParticles(sb, RenderLayer.UnderNPCs);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsBehindProjectiles))
            {
                DrawAllParticles(sb, RenderLayer.UnderProjectiles);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverPlayers))
            {
                DrawAllParticles(sb, RenderLayer.OverPlayers);
            }

            if (projCache.Equals(Main.instance.DrawCacheProjsOverWiresUI))
            {
                DrawAllParticles(sb, RenderLayer.OverWiresUI);
            }

            if (startSpriteBatch)
                sb.End();
        }
    }
}
