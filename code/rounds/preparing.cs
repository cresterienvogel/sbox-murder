using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Preparing : BaseRound
{
	public override string RoundName => "Preparing";

	public override int RoundDuration { get; set; } = 20;

	public override void OnPlayerLeave(Player player)
	{
		base.OnPlayerLeave(player);
	}

	public override void OnPlayerSpawn(Player player)
	{
		if (!Players.Contains(player))
			AddPlayer(player);
			
		base.OnPlayerSpawn(player);
	}

	protected override void OnStart()
	{
		Game.Instance.RespawnEnabled = true;

		using (Prediction.Off())
			Game.Instance.ShowWinner(To.Everyone, "");

		foreach (var client in Client.All)
		{
			if (client.Pawn is not Player player)
				continue;

			if (player.Tags.Has("murderer")) 
				player.Tags.Remove("murderer");

			if (player.Tags.Has("detective")) 
				player.Tags.Remove("detective");

			if (player.Ragdoll != null)
				player.Ragdoll.Delete();
				player.Ragdoll = null;

			player.Respawn();
		}
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
		Game.Instance.ChangeRound(new Playing());
		base.OnTimeUp();
	}
}