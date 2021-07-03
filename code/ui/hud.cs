using Sandbox.UI;

public partial class HudEntity : Sandbox.HudEntity<RootPanel>
{
	public static HudEntity Current;

	public HudEntity()
	{
		if (!IsClient)
			return;

		Current = this;
			
		RootPanel.SetTemplate("ui/hud.html");

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<Scoreboard>();
		RootPanel.AddChild<VoiceList>();
	}
}