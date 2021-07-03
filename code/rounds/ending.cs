using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Ending : BaseRound
{
	public override string RoundName => "Ending";

	public override int RoundDuration { get; set; } = 20;

	public override void OnPlayerKilled(Player player)
	{
		player.Respawn();
	}

	public override void OnPlayerLeave(Player player)
	{
		base.OnPlayerLeave(player);

		if (Players.Contains(player))
			Players.Remove(player);
	}

	public override void OnPlayerSpawn(Player player)
	{
		if (!Players.Contains(player))
			AddPlayer(player);
			
		player.MakeSpectator();
		base.OnPlayerSpawn(player);
	}
	protected override void OnStart()
	{
		Game.Instance.RespawnEnabled = false;
		base.OnStart();
	}

	public override void OnSecond()
	{
		base.OnSecond();
	}

	public override void OnTick()
	{
		base.OnTick();
	}

	public override void OnTimeUp()
	{
		Game.Instance.ChangeRound(new Preparing());
		base.OnTimeUp();
	}
}