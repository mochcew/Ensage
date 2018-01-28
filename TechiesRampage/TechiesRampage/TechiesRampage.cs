using System.ComponentModel.Composition;
using System.Reflection;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_sven;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;
using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;
using log4net;

using PlaySharp.Toolkit.Logging;
namespace TechiesRampage
{
	[ExportPlugin(
		name: "TechiesRampage",
		mode: StartupMode.Auto,
		author: "PIER", 
		version: "1.2.1.1",
		units: HeroId.npc_dota_hero_techies)]
	internal class TechiesRampage : Plugin
	{
		private Config config { get; set; }

		public IServiceContext context { get; set; }

		public ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Ability RemoteMines { get; set; }


		[ImportingConstructor]
		public TechiesRampage([Import] IServiceContext _context)
		{
			context = _context;
		}

		[ItemBinding]
		public item_force_staff ForceStaff { get; set; }

		protected override void OnActivate()
		{
			// Init everything (menu, grab abilities, etc)
			config = new Config(this);

			this.RemoteMines = UnitExtensions.GetAbilityById(context.Owner, AbilityId.techies_remote_mines);

			this.context.Inventory.Attach(this);

			base.OnActivate();
		}

		protected override void OnDeactivate()
		{
			// Free stuff again (menu
			context.Inventory.Detach(this);

			config?.Dispose();
		}
		
	}
}
