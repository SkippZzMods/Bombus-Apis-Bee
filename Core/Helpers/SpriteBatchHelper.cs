namespace BombusApisBee.Core.Helpers
{
    public static class SpriteBatchHelper
    {
        /// <summary>
        /// Ends & restarts the spriteBatch with default vanilla parameters.
        /// </summary>
        public static void RestartToDefault(this SpriteBatch batch)
        {
            batch.End();
            batch.BeginDefault();
        }

        /// <summary>
        /// Begins the spriteBatch with default vanilla parameters.
        /// </summary>
        /// <param name="batch"></param>
        public static void BeginDefault(this SpriteBatch batch) => batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

    }
}
