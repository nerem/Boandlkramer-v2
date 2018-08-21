using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoG.Math2Int;
using LoG.Map;
using LoG.Flocking;
using Random = UnityEngine.Random;
using Rect = LoG.Math2Int.Rect;

public class MapGenerator : MonoBehaviour {

	public int seed;
	public MapTheme Theme;
	public MapObject Entrance;
	public MapObject Exit;

	public GameObject SpawnPoint;

	void Start () {
		//Random.InitState (seed);
		//TestRandomGen ();
	}

	public void TestRandomGen () {
		List<Rect> rects = new List<Rect> ();
		for (int i = 0; i < 150; i++)
			rects.Add (Rect.GetRandom (() => RandomVector2.GetRandomInCircle (() => RandomNormal (std: 10, range: 40)), () => Vector.GetRandom (1, 6)));
		Flock flock = new Flock (rects);
		flock.Separate ();
		flock.LockToGrid ();
		Map map = Map.Create (flock.ToRect (), rule: (i, r) => i < r.Count * 0.2f ? TileType.Room : TileType.Platform);
		map.ConsolidateRooms ();
		foreach (Grid2D<MapTile> grid in map.Grids) {
			//GizmoRenderer.CreateWireSphere (null, pos: (Vector3)grid.Center);
			//GridRenderer.Create (grid, parent: new GameObject ("Map").transform, rule: x => x.Type == TileType.Room ? Highlight : Material);
		}
		Graph graph = map.GetMST (0.25f);
		//GraphRenderer renderer = GraphRenderer.Create (graph, material: Material);
		//renderer.transform.localPosition = new Vector3 (0f, 1f, 0f);
		//GridRenderer.Create (map.Grid, parent: new GameObject ("GRID").transform, rule: x => x.Type == TileType.Room ? Highlight : Material);
		map.Connect (graph);
		/*
		foreach (Grid2D<MapTile> grid in map.Grids) {
			//GizmoRenderer.CreateWireSphere (null, pos: (Vector3)grid.Center);
			GridRenderer.Create (grid, parent: new GameObject ("Map").transform, rule: x => x.Type == TileType.Room ? Highlight : Material);
		}
		*/
		//GridRenderer.Create (map.Grid, new GameObject ("Map").transform, rule: x => x.Type == TileType.Room ? Highlight : Material);
		//GridRenderer.Create (map.Grid, parent: new GameObject ("Map").transform, rule: x => x.Type == TileType.Room ? Highlight : Material);
		Transform t = new GameObject ("MAP").transform;
		t.position += new Vector3 (0, 0, 0);
		Theme.Build (map, t);
		CreateEntrance (map);
		CreateExit (map);
	}

	public float RandomNormal (float mean = 0, float std = 1, float range = 3f) {
		float x = 1f - Random.Range (0f, 1f);
		float y = 1f - Random.Range (0f, 2f * Mathf.PI);
		float z = Mathf.Sqrt (-2f * Mathf.Log (x)) * Mathf.Cos (y);
		return Mathf.Clamp (std * z + mean, -range, range);
	}

	private void CreateEntrance (Map map) {
		MapNode wall = map.RandomWall;
		MapObject obj = Instantiate (Entrance);
		MapObject old = wall.Object;
		wall.Object = obj;
		GameObject instance = Instantiate (obj.Object, (Vector3)old.Node.Position, (Quaternion)old.Node.Rotation);
		if (old.Object.transform.parent != null)
			instance.transform.SetParent (old.Object.transform.parent, false);
		wall.Object.Object = instance;
		Destroy (old.Object.gameObject);
		SpawnPoint.transform.position = instance.transform.position;
	}

	private void CreateExit (Map map) {
		MapNode wall = map.RandomWall;
		MapObject obj = Instantiate (Exit);
		MapObject old = wall.Object;
		wall.Object = obj;
		GameObject instance = Instantiate (obj.Object, (Vector3)old.Node.Position, (Quaternion)old.Node.Rotation);
		if (old.Object.transform.parent != null)
			instance.transform.SetParent (old.Object.transform.parent, false);
		wall.Object.Object = instance;
		Destroy (old.Object.gameObject);
	}
}
