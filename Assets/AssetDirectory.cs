namespace BombusApisBee.Assets
{
    /// <summary>
    /// Includes commonly used textures as well as directories to each Biome subfolder in Content to easier access textures
    /// </summary>
    public static class AssetDirectory    
    {
        public static string Assets = "BombusApisBee/Assets/";
        public static string ExtraTextures = Assets + "ExtraTextures/";

        public static string BeeGlow = ExtraTextures + "BeeGlow";
        public static string WaspGlow = ExtraTextures + "BeeGlow_Wasp";
        public static string GiantBeeGlow = ExtraTextures + "BeeGlow_Giant";

        public static string BeeWhite = ExtraTextures + "BeeWhite";
        public static string WaspWhite = ExtraTextures + "BeeWhite_Wasp";
        public static string GiantBeeWhite = ExtraTextures + "BeeWhite_Giant";

        public static string BeeOutline = ExtraTextures + "BeeOutline";
        public static string WaspOutline = ExtraTextures + "BeeOutline_Wasp";
        public static string GiantBeeOutline = ExtraTextures + "BeeOutline_Giant";

        public static string ApiaryOutline = ExtraTextures + "Apiary_Outline";
        public static string ApiaryGlow = ExtraTextures + "Apiary_Glowy";
    }
}
