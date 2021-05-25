using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy Spawn Info")]
	public bool spawnEnemies = true;

	[HideInInspector]
	public bool useECS = false;
	public float enemySpawnRadius = 10f;
	public GameObject enemyPrefab;

	[Header("Enemy Spawn Timing")]
	[Range(1, 100)] public int spawnsPerInterval = 1;
	[Range(.1f, 2f)] public float spawnInterval = 1f;
	
	EntityManager manager;
	Entity enemyEntityPrefab;

	float cooldown;
	BlobAssetStore blobAssetStore;

	void Start()
	{
		if (useECS)
		{

			blobAssetStore = new BlobAssetStore();
			manager = World.DefaultGameObjectInjectionWorld.EntityManager;
			var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
			enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);
		}
	}

	void Update()
    {
		if (!spawnEnemies || Settings.IsPlayerDead())
			return;

		cooldown -= Time.deltaTime;

		if (cooldown <= 0f)
		{
			cooldown += spawnInterval;
			Spawn();
		}
    }

	void Spawn()
	{
		for (int i = 0; i < spawnsPerInterval; i++)
		{
			Vector3 pos = Settings.GetPositionAroundPlayer(enemySpawnRadius);

			if (!useECS)
			{
				Instantiate(enemyPrefab, pos, Quaternion.identity);
			}
			else
			{
				Entity enemy = manager.Instantiate(enemyEntityPrefab);
				manager.SetComponentData(enemy, new Translation { Value = pos });
			}
		}
	}

    private void OnDestroy()
    {
		blobAssetStore.Dispose(); //必须写这个 不然会报错
	}
}
