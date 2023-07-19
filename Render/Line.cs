using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModBridge.Render {
	public struct Line : Renderable {

		private Vector2 start, end;
		private Color color;

		public Line(Vector2 start, Vector2 end, Color color) {
			this.start = start;
			this.end = end;
			this.color = color;
		}

		public void Render(SpriteBatch batch) {
			Utils.DrawLine(batch, start, end, color, color, 1f);
		}

	}
}
