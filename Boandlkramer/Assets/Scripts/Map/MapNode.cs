using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoG.Math2Int;
using Random = UnityEngine.Random;

namespace LoG.Map {

	#region MAP NODE
	#region ENUMS
	public enum NodeType { None = 0, Floor, Wall, Door, InsideCorner, OutsideCorner, Railing, RailingPillar }
	#endregion

	public class MapNode {

		#region PROPERTIES
		public NodeType Type { get; private set; }
		public Vector Position { get; private set; }
		public Rotation Rotation { get; private set; }
		public Map Map { get; private set; }
		public MapObject Object { get; set; }
		#endregion

		#region CONSTRUCTOR
		public MapNode (NodeType type, Vector pos, Rotation rot, Map map) {
			Type = type;
			Position = pos;
			Rotation = rot;
			Map = map;
		}
		#endregion

		#region STATIC FUNCTIONS
		public static MapNode Floor (Vector pos, Rotation rot, Map map) {
			return new MapNode (NodeType.Floor, pos, rot, map);
		}

		public static MapNode Wall (Vector pos, Rotation rot, Map map) {
			return new MapNode (NodeType.Wall, pos, rot, map);
		}

		public static MapNode Door (Vector pos, Rotation rot, Map map) {
			return new MapNode (NodeType.Door, pos, rot, map);
		}

		public static MapNode InsideCorner (Vector pos, Rotation rot, Map map) {
			return new MapNode (NodeType.InsideCorner, pos, rot, map);
		}

		public static MapNode OutsideCorner (Vector pos, Rotation rot, Map map) {
			return new MapNode (NodeType.OutsideCorner, pos, rot, map);
		}
		#endregion
	}
	#endregion
}
