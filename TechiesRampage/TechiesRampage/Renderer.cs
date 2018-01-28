using System;
using System.ComponentModel;
using Ensage;

using SharpDX;

namespace TechiesRampage
{
	internal class Renderer
	{
		private Config config { get; set; }

		private Vector2 Screen { get; set; }

		public Renderer(Config _config)
		{
			config = _config;

			Screen = new Vector2(Drawing.Width - 160, Drawing.Height);

			Drawing.OnDraw += OnDraw;

		}

		public void Dispose()
		{
			Drawing.OnDraw -= OnDraw;
		}

		private void Text(string text, float heightpos, Color color)
		{
			var pos = new Vector2(Screen.X/2, Screen.Y * heightpos);

			Drawing.DrawText(text, "Arial", pos, new Vector2(21), color, FontFlags.None);
		}

		private void OnDraw(EventArgs args)
		{
			//float pos = 0;
			//pos += 0.03f;
			//Text($"{(config.updateMode.count)}",
			//	0.78f + pos,
			//	(Color.Yellow));
			
		}
	}
}

