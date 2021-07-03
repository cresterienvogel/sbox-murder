using Sandbox;
using System;
using System.Linq;

partial class Inventory : BaseInventory
{
	public Inventory(Player player) : base (player) {}

	public override bool Add(Entity ent, bool makeActive = false)
	{
		var player = Owner as Player;
		var weapon = ent as Weapon;

		if (weapon != null && IsCarryingType(ent.GetType()))
		{
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;

			if (ammo > 0)
				player.GiveAmmo(ammoType, ammo);

			ItemRespawn.Taken(ent);

			ent.Delete();
			return false;
		}

		ItemRespawn.Taken(ent);
		return base.Add(ent, makeActive);
	}

	public bool IsCarryingType(Type t)
	{
		return List.Any(x => x.GetType() == t);
	}
}