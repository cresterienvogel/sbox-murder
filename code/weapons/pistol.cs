using Sandbox;


[Library("murder_pistol", Title = "Pistol")]
[Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
partial class Pistol : Weapon
{ 
	public override float PrimaryRate => 1.0f;
	public override float ReloadTime => 2.0f;

	public override int Bucket => 1;

	public override void Spawn()
	{
		base.Spawn();

		SetModel("weapons/rust_pistol/rust_pistol.vmdl");
		AmmoClip = 12;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;

		if (!TakeAmmo(1))
		{
			DryFire();
			return;
		}

		ShootEffects();
		PlaySound("rust_pistol.shoot");

		ShootBullet(0.05f, 5.0f, 150.0f, 1);
	}
}
