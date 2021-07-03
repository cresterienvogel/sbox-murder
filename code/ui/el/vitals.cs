using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
	public static Vitals Current;

	public Label Timer;
	public Label Round;
	public Label Role;
	public Label Winner;

	public Vitals()
	{
		Current = this;

		Timer = Add.Label("");
		Round = Add.Label("Waiting for players");
		Role = Add.Label("");
		Winner = Add.Label("");

		Timer.SetClass("timer", true);
		Round.SetClass("round", true);
		Role.SetClass("role", true);
		Winner.SetClass("winner", true);
	}

	public void ShowWinner(string p)
	{
		Winner.Text = p;
	}

	public override void Tick()
	{
		var player = Local.Client.Pawn;
		if (player == null) 
			return;

		var game = Game.Instance;
		if (game == null) 
			return;

		var round = game.Round;
		if (round == null) 
			return;

		Timer.Text = round.TimeLeftFormatted;
		Round.Text = " " + round.RoundName;

		if (player.Tags.Has("spectator"))
		{
			Role.Text = "Spectator";
			return;
		}

		if (player.Tags.Has("detective"))
		{
			Role.Text = "Detective";
			return;
		}

		if (player.Tags.Has("murderer"))
		{
			Role.Text = "Murderer";
			return;
		}

		Role.Text = "Bystander";
	}
}
