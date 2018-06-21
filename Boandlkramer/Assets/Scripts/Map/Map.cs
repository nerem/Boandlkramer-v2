using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;
using Rect = LoG.Math2Int.Rect;

namespace LoG.Map {

	#region MAP
	public class Map {

		#region PRIVATE VARIABLES
		private Dictionary<TileType, List<Grid2D<MapTile>>> _grids;
		private Grid2D<MapTile> _grid;
		private bool _gridChanged;
		#endregion

		#region PROPERTIES
		public List<MapPattern> Patterns { get; private set; }
		public Grid2D<MapTile> Grid {
			get {
				if (_gridChanged) {
					_grid = Grid2D<MapTile>.Merge (_grids.Values.SelectMany (x => x).ToList ());
					_gridChanged = false;
				}
				else if (_grid == null)
					_grid = new Grid2D<MapTile> ();
				return _grid;
			}
		}
		public List<Grid2D<MapTile>> Grids {
			get {
				List<Grid2D<MapTile>> list = new List<Grid2D<MapTile>> ();
				foreach (var g in _grids.Values.SelectMany (x => x))
					list.Add (new Grid2D<MapTile> (g.Elements));
				return list;
			}
		}
		#endregion

		#region CONSTRUCTOR
		public Map () {
			Initialize ();
		}

		public Map (Dictionary<TileType, List<Grid2D<MapTile>>> grids) {
			Initialize ();
			_grids = grids;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static Map Create (List<Rect> rects, Func<int, List<Rect>, TileType> rule = null) {
			if (rule == null)
				rule = (i, l) => i < l.Count * 0.1f ? TileType.Room : TileType.Platform;
			Map map = new Map ();
			rects = rects.OrderBy (x => x.Area).Reverse ().ToList ();
			for (int i = 0; i < rects.Count; i++)
				map.Add (rects[i], rule (i, rects));
			return map;
		}

		public static Map Create (List<Vector> vectors, TileType type = TileType.Empty) {
			Map map = new Map ();
			map._grids[type].Add (new Grid2D<MapTile> ());
			foreach (Vector vector in vectors)
				map._grids[type].Last ().Add (vector, new MapTile (map, map._grids[type].Last (), vector, type));
			return map;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Build () {
			foreach (Grid2D<MapTile> grid in _grids.Values.SelectMany (x => x))
				foreach (MapTile tile in grid.Elements.Values)
					tile.Build (this);
		}

		public void Add (Rect rect, TileType type) {
			_grids[type].Add (new Grid2D<MapTile> ());
			_gridChanged = true;
			for (int i = (int)rect.Min.x; i < (int)rect.Max.x; i++)
				for (int j = (int)rect.Min.y; j < (int)rect.Max.y; j++)
					_grids[type].Last ().Add (new Vector (i, j), new MapTile (this, _grids[type].Last(), new Vector (i, j), type));
		}

		public void ConsolidateRooms () {
			_grids[TileType.Room] = Grid2D<MapTile>.GetConnectedComponents (Grid2D<MapTile>.Merge (_grids[TileType.Room]));
		}

		public Graph GetMST (float percent = 0.2f) {
			return Graph.Kruskal (new Graph (Graph.Delaunay (_grids[TileType.Room].Select (g => g.Center).ToList ())), percent);
		} 

		public Grid2D<MapTile> Connect (Grid2D<MapTile> gridA, Grid2D<MapTile> gridB) {
			Grid2D<MapTile> grid = new Grid2D<MapTile> ();
			Grid2D<MapTile> map = new Grid2D<MapTile> (Grid.Elements);
			Vector start = gridA.Elements.Keys.OrderBy (x => Random.value).First ();
			Vector end = gridB.Elements.Keys.OrderBy (x => Random.value).First ();
			List<Vector> points = new List<Vector> ();
			if (Mathf.Abs (start.X - end.X) < 3)
				points = new List<Vector> () { start, new Vector (start.X, end.Y) };
			if (Mathf.Abs (start.Y - end.Y) < 3)
				points = new List<Vector> () { start, new Vector (end.X, start.Y) };
			else {
				Vector middle = Random.Range (0f, 1f) < 0.5f ? new Vector (start.X, end.Y) : new Vector (end.X, start.Y);
				points = new List<Vector> () { start, middle, end };
			}
			grid = Grid2D<MapTile>.Create (points, construct: x => new MapTile (this, null, x, TileType.Platform));
			List<Vector> intersect = grid.IntersectWith (map, rule: (x, y) => y.Type == TileType.Platform);
			foreach (Vector key in intersect)
				if (map.Elements.Keys.Contains (key)) {
					grid.MergeWith (map.Elements[key].Grid);
					map.Remove (map.Elements[key].Grid);
				}
			foreach (var room in _grids[TileType.Room])
				grid.Remove (room);
			foreach (Grid2D<MapTile> comp in Grid2D<MapTile>.GetConnectedComponents (grid))
				if (Grid2D<MapTile>.Merge (new List<Grid2D<MapTile>> () { comp, gridA, gridB }, (x, y) => x.Type == TileType.Room ? x : y).GetConnected (start).Elements.Keys.Contains (end)) {
					Vector[] keys = comp.Elements.Keys.ToArray ();
					foreach (Vector k in keys)
						comp.Add (k, new MapTile (this, comp, k, TileType.Platform));
					intersect = comp.IntersectWith (Grid, rule: (x, y) => y.Type == TileType.Platform);
					foreach (var g in intersect)
						_grids[TileType.Platform].Remove (Grid.Elements[g].Grid);
					_grids[TileType.Platform].Add (comp);
					_gridChanged = true;
					MakeDoor (gridA, comp);
					MakeDoor (gridB, comp);
					return comp;
				}
			return null;
		}

		public void Connect (Graph graph) {
			List<Grid2D<MapTile>> list = new List<Grid2D<MapTile>> (_grids[TileType.Platform]);
			Dictionary<Vector, Grid2D<MapTile>> dict = new Dictionary<Vector, Grid2D<MapTile>> ();
			foreach (Grid2D<MapTile> g in _grids.Values.SelectMany (x => x))
				dict.Add (g.Center, g);
			foreach (Edge e in graph.Edges) {
				for (int i = 0; i < 10; i++)
					if (Connect (dict[e.Start], dict[e.End]) != null)
						break;
			}
			foreach (var g in list)
				_grids[TileType.Platform].Remove (g);
			_gridChanged = true;
		}

		public void MakeDoor (Grid2D<MapTile> from, Grid2D<MapTile> to) {
			MapPattern pattern = MapPattern.Door ();
			Grid2D<MapTile> grid = Grid2D<MapTile>.Merge (new List<Grid2D<MapTile>> () { from, to });
			foreach (Vector v in from.Elements.Keys.OrderBy (x => Random.value))
				foreach (Rotation rot in Rotation.All)
					if (pattern.Matches (grid, v, rot)) {
						Grid.Elements[v].Nodes[NodeType.Door].Add (MapNode.Door (v, rot, this));
						return;
					}
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private void Initialize () {
			_grids = new Dictionary<TileType, List<Grid2D<MapTile>>> ();
			foreach (TileType t in Enum.GetValues (typeof (TileType)))
				_grids.Add (t, new List<Grid2D<MapTile>> ());
			_gridChanged = true;
			Patterns = new List<MapPattern> () { MapPattern.Floor (), MapPattern.Wall (), MapPattern.InsideCorner (), MapPattern.OutsideCorner (), MapPattern.Railing (), MapPattern.RailingInsideCorner (), MapPattern.RailingOutsideCorner () };
		}
		#endregion
	}
	#endregion

	#region MAP TILE
	#region ENUMS
	public enum TileType { Empty, Platform, Room }
	#endregion

	public struct MapTile {

		#region PROPERTIES
		public Map Map { get; private set; }
		public Grid2D<MapTile> Grid { get; private set; }
		public Vector Coordinates { get; private set; }
		public TileType Type { get; private set; }
		public Dictionary<NodeType, List<MapNode>> Nodes { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public MapTile (Map map, Grid2D<MapTile> grid, Vector coordinates, TileType type = TileType.Empty) {
			Map = map;
			Grid = grid;
			Coordinates = coordinates;
			Type = type;
			Nodes = new Dictionary<NodeType, List<MapNode>> ();
			foreach (NodeType n in Enum.GetValues (typeof (NodeType)))
				Nodes.Add (n, new List<MapNode> ());
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Build (Map map) {
			foreach (MapPattern pattern in map.Patterns) {
				foreach (MapNode node in Build (map.Grid, Coordinates, pattern))
					if (node != null && (node.Type == NodeType.Floor || Nodes[NodeType.Door].Count == 0))
						Nodes[node.Type].Add (node);
			}
		}

		public void SetType (TileType type) {
			Type = type;
		}
		#endregion

		#region PRIVATE FUNCTIONS
		private List<MapNode> Build (Grid2D<MapTile> grid, Vector pos, MapPattern pattern) {
			List<MapNode> nodes = new List<MapNode> ();
			foreach (Rotation rot in Rotation.All)
				if (pattern.Matches (grid, pos, rot)) {
					nodes.Add (new MapNode (pattern.Type, Coordinates, rot, null));
					if (pattern.Type != NodeType.Railing)
						return nodes;
				}
			return nodes;
		}
		#endregion

		#region OVERRIDE
		public override bool Equals (object obj) {
			return (MapTile)obj == this;
		}

		public override int GetHashCode () {
			return base.GetHashCode ();
		}
		#endregion

		#region OPERATORS
		public static bool operator == (MapTile left, MapTile right) {
			if (left.Type != right.Type)
				return false;
			return true;
		}

		public static bool operator != (MapTile left, MapTile right) {
			return !(left == right);
		}
		#endregion
	}
	#endregion

	#region MAP PATTERN
	public class MapPattern {

		#region PROPERTIES
		public Grid2D<PatternTile> Grid { get; private set; }
		public NodeType Type { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public MapPattern () {
			Grid = new Grid2D<PatternTile> ();
		}
		#endregion

		#region STATIC FUNCTIONS
		public static MapPattern Floor () {
			List<List<Vector>> vectorsList = new List<List<Vector>> () { new List<Vector> () { Vector.Zero } };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Room, TileType.Platform } };
			return Create (vectorsList, typesList, NodeType.Floor);
		}

		public static MapPattern Wall () {
			List<Vector> empty = new List<Vector> () { Vector.Up };
			List<Vector> filled = new List<Vector> () { Vector.Left, Vector.Zero, Vector.Right };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty, TileType.Platform }, new List<TileType> () { TileType.Room } };
			return Create (vectorsList, typesList, NodeType.Wall);
		}

		public static MapPattern Door () {
			List<Vector> empty = new List<Vector> () { Vector.Up + Vector.Left, Vector.Up + Vector.Right };
			List<Vector> platform = new List<Vector> () { Vector.Up };
			List<Vector> filled = new List<Vector> () { Vector.Left, Vector.Zero, Vector.Right };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, platform, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty, TileType.Platform }, new List<TileType> () { TileType.Platform }, new List<TileType> () { TileType.Room } };
			return Create (vectorsList, typesList, NodeType.Door);
		}

		public static MapPattern InsideCorner () {
			List<Vector> empty = new List<Vector> () { Vector.Up, Vector.Left };
			List<Vector> filled = new List<Vector> () { Vector.Zero, Vector.Down, Vector.Right };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty, TileType.Platform }, new List<TileType> () { TileType.Room } };
			return Create (vectorsList, typesList, NodeType.InsideCorner);
		}

		public static MapPattern OutsideCorner () {
			List<Vector> empty = new List<Vector> () { Vector.Left + Vector.Up };
			List<Vector> filled = new List<Vector> () { Vector.Left, Vector.Zero, Vector.Up };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty, TileType.Platform }, new List<TileType> () { TileType.Room } };
			return Create (vectorsList, typesList, NodeType.OutsideCorner);
		}

		public static MapPattern Railing () {
			List<Vector> empty = new List<Vector> () { Vector.Up };
			List<Vector> filled = new List<Vector> () { Vector.Zero };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty }, new List<TileType> () { TileType.Platform } };
			return Create (vectorsList, typesList, NodeType.Railing);
		}

		public static MapPattern RailingInsideCorner () {
			List<Vector> empty = new List<Vector> () { Vector.Up, Vector.Left };
			List<Vector> filled = new List<Vector> () { Vector.Zero, Vector.Down, Vector.Right };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty }, new List<TileType> () { TileType.Platform } };
			return Create (vectorsList, typesList, NodeType.RailingPillar);
		}

		public static MapPattern RailingOutsideCorner () {
			List<Vector> empty = new List<Vector> () { Vector.Left + Vector.Up };
			List<Vector> filled = new List<Vector> () { Vector.Left, Vector.Zero, Vector.Up };
			List<List<Vector>> vectorsList = new List<List<Vector>> () { empty, filled };
			List<List<TileType>> typesList = new List<List<TileType>> () { new List<TileType> () { TileType.Empty }, new List<TileType> () { TileType.Platform } };
			return Create (vectorsList, typesList, NodeType.RailingPillar);
		}

		public static MapPattern Create (List<List<Vector>> vectorsList, List<List<TileType>> typesList, NodeType type) {
			MapPattern pattern = new MapPattern ();
			pattern.Type = type;
			for (int i = 0; i < Math.Min (vectorsList.Count, typesList.Count); i++)
				pattern.Add (vectorsList[i], typesList[i]);
			return pattern;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public void Add (List<Vector> vectors, List<TileType> types) {
			foreach (Vector vector in vectors)
				Grid.Add (vector, new PatternTile (vector, types));
		}

		public bool Matches (Grid2D<MapTile> other, Vector pos, Rotation rot) {
			foreach (PatternTile tile in Grid.Elements.Values)
				if (!tile.Matches (other.Get (pos + rot * tile.Coordinates, d: new MapTile ())))
					return false;
			return true;
		}
		#endregion
	}
	#endregion

	#region PATTERN TILE
	public struct PatternTile {

		#region PROPERTIES
		public Vector Coordinates { get; private set; }
		public List<TileType> Types { get; private set; }
		#endregion

		#region CONSTRUCTOR
		public PatternTile (Vector coordinates, List<TileType> types) {
			Coordinates = coordinates;
			Types = types;
		}
		#endregion

		#region PUBLIC FUNCTIONS
		public bool Matches (MapTile tile) {
			foreach (TileType type in Types)
				if (type == tile.Type)
					return true;
			return false;
		}
		#endregion
	}
	#endregion
}
