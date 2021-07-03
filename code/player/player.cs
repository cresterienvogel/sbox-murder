using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Player : Sandbox.Player
{
	[Net] public bool IsDead { get; set; }
	[Net, Local] public ModelEntity Ragdoll { get; set; }

	public Player()
	{
		Inventory = new Inventory(this);
		Animator = new StandardPlayerAnimator();
	}

	public override void Respawn()
	{
		if (!Game.Instance.RespawnEnabled)
			return;

		SetModel("models/citizen/citizen.vmdl");

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		Camera = new ThirdPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		base.Respawn();

		Inventory.DeleteContents();

		Dress();
		ClearAmmo();

		if (this.Tags.Has("murderer"))
			Inventory.Add(new Knife(), true);

		if (this.Tags.Has("detective"))
		{
			Inventory.Add(new Pistol(), true);
			GiveAmmo(AmmoType.Pistol, 100);
		}

		IsDead = false;

		if (this.Tags.Has("penalty"))
			this.Tags.Remove("penalty");
	}

	public override void Simulate(Client cl)
	{
		base.Simulate(cl);
		SimulateActiveChild(cl, ActiveChild);
	}

	public void MakeSpectator()
	{
		EnableAllCollisions = false;
		EnableDrawing = false;
		Controller = null;
		Camera = new SpectateCamera();
	}

	public override void OnKilled()
	{
		base.OnKilled();
		Inventory.DeleteContents();

		if (GetModelName() != null)
		{
			var ragdoll = new ModelEntity();
			ragdoll.SetModel(GetModelName());  
			ragdoll.Position = this.Position;
			ragdoll.SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);

			ragdoll.CopyBonesFrom(this);
			ragdoll.SetRagdollVelocityFrom(this);

			foreach (Entity child in this.Children)
			{
				if (child is ModelEntity e)
				{
					string model = e.GetModelName();
					if (model == null || !model.Contains("clothes"))
						continue;

					ModelEntity clothing = new ModelEntity();
					clothing.SetModel(model);
					clothing.SetParent(ragdoll, true);
					clothing.RenderColor = e.RenderColor;
				}
			}

			Ragdoll = ragdoll;
		}

		EnableAllCollisions = false;
		EnableDrawing = false;

		Controller = null;
		Camera = new SpectateCamera();

		IsDead = true;
	}
}
