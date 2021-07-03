﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

partial class Weapon : BaseWeapon, IRespawnableEntity
{
	public virtual AmmoType AmmoType => AmmoType.Pistol;
	public virtual int ClipSize => 16;
	public virtual float ReloadTime => 3.0f;
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;
	public virtual int HoldType => 1;
	public virtual bool IsMelee => false;

	public virtual string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

	[Net, Predicted] public int AmmoClip { get; set; }
	[Net, Predicted] public TimeSince TimeSinceReload { get; set; }
	[Net, Predicted] public bool IsReloading { get; set; }
	[Net, Predicted] public TimeSince TimeSinceDeployed { get; set; }

	public PickupTrigger PickupTrigger { get; protected set; }

	public int AvailableAmmo()
	{
		var owner = Owner as Player;
		if (owner == null) 
			return 0;

		return owner.AmmoCount(AmmoType);
	}

	public override void ActiveStart(Entity ent)
	{
		base.ActiveStart(ent);
		TimeSinceDeployed = 0;
		IsReloading = false;
	}

	public override void Spawn()
	{
		base.Spawn();

		if (WorldModelPath != null)
			SetModel(WorldModelPath);

		PickupTrigger = new PickupTrigger();
		PickupTrigger.Parent = this;
		PickupTrigger.Position = Position;
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", (int)HoldType);
		anim.SetParam("aimat_weight", 1.0f);
	}

	public override void Reload()
	{
		if (IsMelee || IsReloading)
			return;

		if (AmmoClip >= ClipSize)
			return;

		TimeSinceReload = 0;

		if (Owner is Player player)
		{
			if (player.AmmoCount(AmmoType) <= 0)
				return;

			StartReloadEffects();
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool("b_reload", true);

		StartReloadEffects();
	}

	public override void Simulate(Client owner) 
	{
		if (TimeSinceDeployed < 0.6f)
			return;

		if (!IsReloading)
			base.Simulate(owner);

		if (IsReloading && TimeSinceReload > ReloadTime)
			OnReloadFinish();
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if (Owner is Player player)
		{
			var ammo = player.TakeAmmo(AmmoType, ClipSize - AmmoClip);
			if (ammo == 0)
				return;

			AmmoClip += ammo;
		}
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimBool("reload", true);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;

		ShootEffects();

		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 5000))
		{
			tr.Surface.DoBulletImpact(tr);

			if (!IsServer) 
				continue;

			if (!tr.Entity.IsValid()) 
				continue;

			using (Prediction.Off())
			{
				var damage = DamageInfo.FromBullet(tr.EndPos, Owner.EyeRot.Forward * 100, 15)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);

				tr.Entity.TakeDamage(damage);
			}
		}
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");

		if (IsLocalPawn)
			new Sandbox.ScreenShake.Perlin();
	}
	public virtual void ShootBullet(float spread, float force, float damage, float bulletSize)
	{
		var forward = Owner.EyeRot.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize))
		{
			tr.Surface.DoBulletImpact(tr);

			if (!IsServer)
				continue;

			if (!tr.Entity.IsValid()) 
				continue;

			using (Prediction.Off())
			{
				var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);

				tr.Entity.TakeDamage(damageInfo);
			}
		}
	}

	public bool TakeAmmo(int amount)
	{
		if (AmmoClip < amount)
			return false;

		AmmoClip -= amount;
		return true;
	}

	[ClientRpc]
	public virtual void DryFire() {}

	public bool IsUsable()
	{
		if (AmmoClip > 0) 
			return true;

		return AvailableAmmo() > 0;
	}

	public override void OnCarryStart(Entity carrier)
	{
		base.OnCarryStart(carrier);

		if (PickupTrigger.IsValid())
			PickupTrigger.EnableTouch = false;
	}

	public override void OnCarryDrop(Entity dropper)
	{
		base.OnCarryDrop(dropper);

		if (PickupTrigger.IsValid())
			PickupTrigger.EnableTouch = true;
	}
}