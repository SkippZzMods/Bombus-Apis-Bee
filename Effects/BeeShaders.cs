using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace BombusApisBee.Effects
{
    public class BeeShaders
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            //Credits to Calamity for this Shader Code
            Ref<Effect> screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/HolyShieldShader", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["HolyShieldShader"] = new Filter(new ScreenShaderData(screenRef, "HolyShieldPass"), EffectPriority.High);
            Filters.Scene["HolyShieldShader"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/SLRCeirosRing", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["SLRCeirosRing"] = new Filter(new ScreenShaderData(screenRef, "SLRCeirosRingPass"), EffectPriority.High);
            Filters.Scene["SLRCeirosRing"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/SLRGlowingDust", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["SLRGlowingDust"] = new Filter(new ScreenShaderData(screenRef, "SLRGlowingDustPass"), EffectPriority.High);
            Filters.Scene["SLRGlowingDust"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/MarkedEffect", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["MarkedEffect"] = new Filter(new ScreenShaderData(screenRef, "MarkedEffectPass"), EffectPriority.High);
            Filters.Scene["MarkedEffect"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/LightningTrail", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["LightningTrail"] = new Filter(new ScreenShaderData(screenRef, "LightningTrailPass"), EffectPriority.High);
            Filters.Scene["LightningTrail"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/RebarTrail", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["RebarTrail"] = new Filter(new ScreenShaderData(screenRef, "RebarTrailPass"), EffectPriority.High);
            Filters.Scene["RebarTrail"].Load();

            screenRef = new Ref<Effect>(ModContent.GetInstance<BombusApisBee>().Assets.Request<Effect>("Effects/FrostbrokenShader", (ReLogic.Content.AssetRequestMode)1).Value);
            Filters.Scene["FrostbrokenShader"] = new Filter(new ScreenShaderData(screenRef, "FrostbrokenShaderPass"), EffectPriority.High);
            Filters.Scene["FrostbrokenShader"].Load();
        }
    }
}
