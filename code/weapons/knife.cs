using Sandbox;


[Library("murder_knife", Title = "Knife")]
[Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
partial class Knife : Weapon
{ 
	public override string WorldModelPath => null;

	public override float PrimaryRate => 2.0f;

	public override int Bucket => 1;

	public override int HoldType => 0;

	public override bool IsMelee => false;

	public virtual int MeleeDistance => 80;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
	}

	public virtual void MeleeStrike(float damage, float force)
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;

		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 3f))
		{
			if (!tr.Entity.IsValid()) 
				continue;

			tr.Surface.DoBulletImpact(tr);

			if (!IsServer) 
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

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;

		PlaySound("Knife.Attack");
		MeleeStrike(150f, 1.5f);
	}
}
