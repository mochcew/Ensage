using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Ensage;
using Ensage.Common;
using Ensage.Common.AbilityInfo;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Service;
using Ensage.SDK.Handlers;
using Ensage.SDK.Renderer.Particle;
using Ensage.Common.Threading;
using System.Threading;
using System.Threading.Tasks;
using SharpDX;
using System;




namespace TechiesRampage
{
	internal class UpdateMode
	{
		private Config config { get; set; }

		private IServiceContext context { get; set; }

		public TaskHandler onUpdateHandler;
		public TaskHandler onUpdateHandler2;

		private Hero main { get; set; }

		public int count { get; set; }

		public List<Hero> enemies { get; set; }
		public List<Unit> greenbombsnearby { get; set; }
		public UpdateMode (Config _config)
		{
			config = _config;
			context = config.techiesRampage.context;
			main = context.Owner as Hero;

		//	UpdateManager.Subscribe (OnUpdate, 25);
		//	UpdateManager.Subscribe (OnUpdate2, 25);
			onUpdateHandler = UpdateManager.Run(OnUpdate);
			onUpdateHandler2 = UpdateManager.Run(OnUpdate2);
		}
		public void Dispose()
		{
		///	UpdateManager.Unsubscribe (OnUpdate);
		//	UpdateManager.Unsubscribe (OnUpdate2);
			onUpdateHandler?.Cancel();
			onUpdateHandler2?.Cancel();
		}
		private async Task OnUpdate(CancellationToken token)
		{
			//List<Hero> enemies = new List<Hero> ();
			//List<Unit> greenbombsnearby = new List<Unit> ();
			//greenbombsnearby.Clear ();
			//enemies.Clear ();

			if (config.EnableCheat) {

				greenbombsnearby = new List<Unit>();
				if (config.techiesRampage.RemoteMines != null) {
					enemies = ObjectManager.GetEntities<Hero> ().Where (x => (x.Team != main.Team)).ToList();
					if (enemies.Count > 0) {

						foreach (Hero enemy in enemies) {
							int bombsneeded = 0;
							float damage = 0.0f;
							int bombscounter = 0;
							damage = AbilityDamage.CalculateDamage (config.techiesRampage.RemoteMines, context.Owner, enemy as Unit, 0, 0, enemy.MagicDamageResist,enemy.Health);
							//damage = getDamage(enemy);
							greenbombsnearby = ObjectManager.GetEntities<Unit> ().Where (x => x.IsInRange (enemy, 400) && x.IsAlive && x.Name == "npc_dota_techies_remote_mine" && x.Team == main.Team && x.IsControllableByPlayer (ObjectManager.LocalPlayer)).ToList ();
							bombsneeded = 0;
							float damageneeded = 0;
							while (greenbombsnearby.Count > 0 && damageneeded < enemy.Health) {
								damageneeded += damage;
								bombsneeded++;
							}

							if (greenbombsnearby.Count>0 && greenbombsnearby.Count >= bombsneeded) 
							{
								foreach (Unit bomb in greenbombsnearby) {
									if (bombscounter < bombsneeded) {
										Ability selfdetonate = UnitExtensions.GetAbilityById (bomb, AbilityId.techies_remote_mines_self_detonate);
										if (selfdetonate != null) {
											selfdetonate.UseAbility ();
											bombscounter++;
										}

									}
								}
							}
						}
					}
				}
			}

			await Task.Delay (25, token);

		}

		private async Task OnUpdate2(CancellationToken token)
		{
				
			if(config.techiesRampage.ForceStaff!=null && config.EnableCheat)
			{
				
				List<Hero> enemies = ObjectManager.GetEntities<Hero> ().Where (x => (x.Team != main.Team) && x.IsVisible && x.IsInRange(context.Owner,config.techiesRampage.ForceStaff.CastRange,true)).ToList();
				if (enemies.Count > 0) {
					foreach (Hero enemy in enemies) {
						Vector3 pos = FindVector (enemy.Position, enemy.Rotation, config.techiesRampage.ForceStaff.PushLength);

						int bombsneeded = 0;
						float damage = 0.0f;
						damage = AbilityDamage.CalculateDamage (config.techiesRampage.RemoteMines, context.Owner, enemy as Unit, 0, 0, enemy.MagicDamageResist,enemy.Health);
						//damage = getDamage(enemy);
						greenbombsnearby = ObjectManager.GetEntities<Unit> ().Where (x => pos.Distance(x.Position) <= 400 && x.IsAlive && x.Name == "npc_dota_techies_remote_mine" && x.Team == main.Team && x.IsControllableByPlayer (ObjectManager.LocalPlayer)).ToList ();

						float damageneeded = 0;

						while (greenbombsnearby.Count>0 && damageneeded < enemy.Health) {
							damageneeded += damage;
							bombsneeded++;
						}

						if (greenbombsnearby.Count>0 && greenbombsnearby.Count >= bombsneeded && config.techiesRampage.ForceStaff.CanBeCasted) {
							config.techiesRampage.ForceStaff.UseAbility (enemy);
						
						}
							
					}

				}


				count = enemies.Count;
			}
			await Task.Delay (25, token);
		}
		private float getDamage(Hero enemy)
		{
			float totalMagicResistance = (1 - enemy.MagicDamageResist);
			float comboDamage = getMineDamage() * totalMagicResistance;

			return comboDamage;
		}
		private float getMineDamage()
		{
			float bombDamage = 0.0f;
			float totalSpellAmp = 0.0f;
			Ability bomb = config.techiesRampage.RemoteMines;

			if (bomb.Level > 0)
			{
				bombDamage += bomb.AbilitySpecialData.First(x => x.Name == "damage").GetValue(bomb.Level - 1);
			}

			totalSpellAmp += (100.0f + main.TotalIntelligence / 15.0f) / 100.0f;
			bombDamage *= totalSpellAmp;

			return bombDamage;
		}

		private Vector3 FindVector(Vector3 first, double ret, float distance)
		{
			var retVector = new Vector3(first.X + (float) Math.Cos(Utils.DegreeToRadian(ret)) * distance, first.Y + (float) Math.Sin(Utils.DegreeToRadian(ret)) * distance, 100);

			return retVector;
		}
	}
}

