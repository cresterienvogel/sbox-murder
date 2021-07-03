using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

[Library("murder", Title = "Murder")]
public partial class Game : Sandbox.Game
{
	[Net] public BaseRound Round { get; private set; }
	
	[Net] public bool RespawnEnabled { get; set; } = true;

	public static Game Instance
	{
		get => Current as Game;
	}

	public Game()
	{
		if (IsServer)
			new HudEntity();
			
		_ = StartTickTimer();
	}

	public override void ClientJoined(Client client)
	{
		base.ClientJoined(client);

		var player = new Player();
		client.Pawn = player;

		player.Respawn();
	}

	public override void DoPlayerNoclip(Client player) {}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();
		_ = StartSecondTimer();
	}

	public async Task StartSecondTimer()
	{
		while (true)
		{
			await Task.DelaySeconds(1);
			OnSecond();
		}
	}

	public async Task StartTickTimer()
	{
		while (true)
		{
			await Task.NextPhysicsFrame();
			OnTick();
		}
	}

	public void ChangeRound(BaseRound round)
	{
		Assert.NotNull(round);
		
		Round?.Finish();
		Round = round;
		Round?.Start();
	}

	private void OnSecond()
	{
		CheckMinimumPlayers();
		Round?.OnSecond();
	}

	private void OnTick()
	{
		Round?.OnTick();
	}

	public override void OnKilled(Entity ent)
	{
		if (ent is Player player)
			Round?.OnPlayerKilled(player);

		base.OnKilled(ent);
	}

	private void CheckMinimumPlayers()
	{
		if (Client.All.Count >= 2)
			if (Round == null)
				ChangeRound(new Preparing());
	}
}