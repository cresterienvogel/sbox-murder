using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IRespawnableEntity {}

public class ItemRespawn
{
	public class Record
	{
		public Transform Transform;
		public string ClassName;
	}

	static Dictionary<Entity, Record> Records = new();

	public static void Init()
	{
		Records = new();
		foreach (var entity in Entity.All)
		{
			if (entity is IRespawnableEntity)
				AddRecordFromEntity(entity);
		}
	}
	public static void AddRecordFromEntity(Entity ent)
	{
		var record = new Record
		{
			Transform = ent.Transform,
			ClassName = ent.ClassInfo.Name
		};
		Records[ent] = record;
	}

	public static void Taken(Entity ent)
	{
		if (Records.Remove(ent, out var record))
			_ = RespawnAsync(record);
	}

	static async Task RespawnAsync(Record record)
	{
		await GameTask.Delay(1000 * 30);

		Sound.FromWorld("dm.item_respawn", record.Transform.Position + Vector3.Up * 50);

		var ent = Library.Create<Entity>(record.ClassName);
		ent.Transform = record.Transform;

		Records[ent] = record;
	}
}
