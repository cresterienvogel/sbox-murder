using Sandbox;

partial class Game
{
	[ClientRpc]
	void ShowWinner(string t)
	{
		Vitals.Current?.ShowWinner(t);
	}
}