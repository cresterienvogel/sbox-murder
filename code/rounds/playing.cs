using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Playing : BaseRound
{
	public override string RoundName => "Playing";

	public override int RoundDuration { get; set; } = 240;
	
	public override void OnPlayerKilled(Player player)
	{
		if (Players.Contains(player))
			Players.Remove(player);

		player.MakeSpectator();
		player.IsDead = true;

		var LastAttacker = player.LastAttacker;

		if (LastAttacker != null)
		{
			if (LastAttacker.Tags.Has("detective") && !player.Tags.Has("murderer"))
			{
				if (LastAttacker.Tags.Has("penalty"))
					LastAttacker.Inventory.DeleteContents();
				else
					LastAttacker.Tags.Add("penalty");
			}
		}

		var alivePlayers = 0;
		foreach (Client client in Client.All)
		{
			if (client.Pawn is not Player pl)
				continue;

			if (!pl.IsDead)
				alivePlayers += 1;
		}

		if (player.Tags.Has("murderer"))
		{
			using (Prediction.Off())
				Game.Instance.ShowWinner(To.Everyone, "Bystanders won !");

			Game.Instance.ChangeRound(new Ending() {
				Players = Players
			});
		}
		else
		{
			if (alivePlayers <= 1)
			{
				using (Prediction.Off())
					Game.Instance.ShowWinner(To.Everyone, "Murderer won !");

				Game.Instance.ChangeRound(new Ending() {
					Players = Players
				});
			}
		}
	}

	public override void OnPlayerLeave(Player player)
	{
		base.OnPlayerLeave(player);
	}

	public override void OnPlayerSpawn(Player player)
	{
		player.MakeSpectator();

		if (Players.Contains(player))
			Players.Remove(player);

		base.OnPlayerSpawn(player);
	}

	void GetDetective()
	{
		var found = false;

		foreach (Client client in Client.All)
		{
			if (client.Pawn is not Player player)
				continue;

			if (player.Tags.Has("murderer"))
				continue;

			found = true;

			player.Tags.Add("detective");
			break;
		}

		if (!found)
			GetDetective();
	} 

	protected override void OnStart()
	{
		Random rand = new Random();
		var target = Client.All[rand.Next(Client.All.Count)];
		target.Pawn.Tags.Add("murderer");

		GetDetective();

		foreach (Client client in Client.All)
		{
			if (client.Pawn is not Player player)
				continue;

			foreach (Entity child in player.Children)
			{
				if (child is ModelEntity clothing)
				{
					string model = clothing.GetModelName();
					if (model == null || !model.Contains("clothes"))
						continue;

					if (model.Contains("hair") || model.Contains("jacket")) 
						clothing.RenderColor = Color.Random;
				}
			}

			if (player.Ragdoll != null)
				player.Ragdoll.Delete();
				player.Ragdoll = null;

			player.Respawn();
		}

		Game.Instance.RespawnEnabled = false;
		base.OnStart();
	}

	protected override void OnFinish()
	{
		foreach (Client client in Client.All)
		{
			if (client.Pawn is not Player player)
				continue;

			if (player.Tags.Has("murderer")) 
				player.Tags.Remove("murderer");

			if (player.Tags.Has("detective")) 
				player.Tags.Remove("detective");

			player.Respawn();
		}
		
		base.OnFinish();
	}

	public override void OnTick()
	{
		base.OnTick();
	}

	public override void OnSecond()
	{
		base.OnSecond();
	}

	public override void OnTimeUp()
	{
		using (Prediction.Off())
			Game.Instance.ShowWinner(To.Everyone, "Bystanders won !");

		Game.Instance.ChangeRound(new Ending() {
			Players = Players
		});

		base.OnTimeUp();
	}
}