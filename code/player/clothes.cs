using Sandbox;
using System;
using System.Linq;

public class Clothing : ModelEntity {}

partial class Player
{
	ModelEntity Coat;
	ModelEntity Shirt;
	ModelEntity Pants;
	ModelEntity Shoes;
	ModelEntity Hair;

	bool dressed = false;

	public void Dress()
	{
		if (dressed) 
			return;

		Coat = new Clothing();
		Coat.SetModel("models/citizen_clothes/jacket/labcoat.vmdl");
		Coat.SetParent(this, true);
		Coat.EnableShadowInFirstPerson = true;
		Coat.EnableHideInFirstPerson = true;

		Shirt = new Clothing();
		Shirt.SetModel("models/citizen_clothes/shirt/shirt_longsleeve.scientist.vmdl");
		Shirt.SetParent(this, true);
		Shirt.EnableShadowInFirstPerson = true;
		Shirt.EnableHideInFirstPerson = true;

		Pants = new Clothing();
		Pants.SetModel("models/citizen_clothes/trousers/trousers.lab.vmdl");
		Pants.SetParent(this, true);
		Pants.EnableShadowInFirstPerson = true;
		Pants.EnableHideInFirstPerson = true;

		Shoes = new Clothing();
		Shoes.SetModel("models/citizen_clothes/shoes/shoes.workboots.vmdl");
		Shoes.SetParent(this, true);
		Shoes.EnableShadowInFirstPerson = true;
		Shoes.EnableHideInFirstPerson = true;

		Hair = new Clothing();
		Hair.SetModel("models/citizen_clothes/hair/hair_malestyle02.vmdl");
		Hair.SetParent(this, true);
		Hair.EnableShadowInFirstPerson = true;
		Hair.EnableHideInFirstPerson = true;

		dressed = true;
	}
}