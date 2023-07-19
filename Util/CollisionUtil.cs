using Terraria;
using Microsoft.Xna.Framework;

namespace ModBridge.Util {
	public static class CollisionUtil {
		public static Vector2 LaserScan(Vector2 pos, Vector2 dir, float width, float maxDistance = 2000, int samples = 400) {
			float[] hitDists = new float[samples];
			Collision.LaserScan(pos, dir, width, maxDistance, hitDists);

			float dist = 0f;
			foreach (float d in hitDists) {
				dist += d;
			}
			dist /= hitDists.Length;

			return pos + dir * dist;
		}
	}
}
