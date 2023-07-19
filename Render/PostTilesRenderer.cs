
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ModBridge.Render {
	public class PostTilesRenderer : ModSystem {

		private Queue<Renderable> renderQueue = new Queue<Renderable>();

		public static PostTilesRenderer Instance => ModContent.GetInstance<PostTilesRenderer>();

		public void Render(Renderable renderable) {
			renderQueue.Enqueue(renderable);
		}

		public override void PostDrawTiles() {
			if (renderQueue.Count > 0) {
				Main.spriteBatch.Begin(
					SpriteSortMode.Deferred,
					BlendState.AlphaBlend,
					Main.DefaultSamplerState,
					DepthStencilState.None,
					Main.Rasterizer,
					null,
					Main.GameViewMatrix.TransformationMatrix
				);

				while (renderQueue.Count > 0) {
					renderQueue.Dequeue().Render(Main.spriteBatch);
				}

				Main.spriteBatch.End();
			}
		}
	}
}
