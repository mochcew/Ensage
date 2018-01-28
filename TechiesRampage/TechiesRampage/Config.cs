using System;
using System.Collections.Generic;
using System.Windows.Input;

using Ensage.Common.Menu;
using Ensage.SDK.Menu;

using SharpDX;

namespace TechiesRampage
{
	internal class Config
	{
		private MenuFactory Factory { get; set; }

		public UpdateMode updateMode { get; set; }

		public Renderer renderer { get; set; }

		public TechiesRampage techiesRampage { get; set; }

		public MenuItem<bool> EnableCheat { get; set; }

		private bool Disposed { get; set; }

		public Config (TechiesRampage _techiesRampage)
		{
			techiesRampage = _techiesRampage;

			Factory = MenuFactory.Create("Techies Rampage", "npc_dota_hero_techies");
			EnableCheat = Factory.Item("Enable", true);

			updateMode = new UpdateMode(this);
			renderer = new Renderer(this);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}

			if (disposing)
			{
				renderer.Dispose();
				updateMode.Dispose();
				Factory.Dispose();
			}

			Disposed = true;
		}
	}
}

