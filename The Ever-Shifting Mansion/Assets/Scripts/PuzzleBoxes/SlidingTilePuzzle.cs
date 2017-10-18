﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PuzzleBox
{
	public class SlidingTilePuzzle : MonoBehaviour
	{
		public enum Position { TopLeft = 1, TopCenter = 2, TopRight = 3, MidLeft = 4, MidCenter = 5, MidRight = 6, BotLeft = 7, BotCenter = 8, BotRight = 9 }
		public enum Direction { UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3 }
		public enum Difficulty { EASY = 0, MEDIUM = 1, HARD = 2}

		Dictionary<Position, Vector2> offset = new Dictionary<Position, Vector2>()
		{
			{ Position.TopLeft, new Vector2(-1, 1) },
			{ Position.TopCenter, new Vector2(0, 1) },
			{ Position.TopRight, new Vector2(1, 1) },
			{ Position.MidLeft, new Vector2(-1, 0) },
			{ Position.MidCenter, new Vector2(0, 0) },
			{ Position.MidRight, new Vector2(1, 0) },
			{ Position.BotLeft, new Vector2(-1, -1) },
			{ Position.BotCenter, new Vector2(0, -1) },
			{ Position.BotRight, new Vector2(1, -1) }
		};
		Dictionary<Difficulty, SlideDifficulty> difficulties = new Dictionary<Difficulty, SlideDifficulty>()
		{
			{ Difficulty.EASY, new SlideDifficulty(12, 4) },
			{ Difficulty.MEDIUM, new SlideDifficulty(24, 5) },
			{ Difficulty.HARD, new SlideDifficulty(50, 8) }
		};

		public GameObject tileObject;
		public float tileSize;
		public Texture2D puzzleImage;
		public Difficulty difficulty;

		[HideInInspector]
		public bool isFocused = false;

		private List<TileSlot> tiles = new List<TileSlot>();
		private TileSlot emptySlot = null;

		void Start()
		{
			GenerateTiles();
			RandomiseTiles();
		}

		void GenerateTiles()
		{
			List<Sprite> tileImages = new List<Sprite>(Resources.LoadAll<Sprite>(puzzleImage.name));


			#region GenerateTiles
			for (int i = 0; i < 9; i += 1)
			{
				Position tilePos = (Position)i + 1;
				Vector3 tileCoord = new Vector3(offset[tilePos].x * tileSize, 0, offset[tilePos].y * tileSize);

				SlidingTile newTile = Instantiate(tileObject, transform).GetComponent<SlidingTile>();
				newTile.SetImage(tileImages[i]);
				newTile.position = tilePos;
				newTile.transform.localPosition = tileCoord;

				tiles.Add(new TileSlot(newTile, tilePos));
			}
			#endregion GenerateTiles
			Debug.Log(tiles.Count);
			#region AssignNeighbours
			for (int i = 0; i < tiles.Count; i += 1)
			{
				switch (tiles[i].position)
				{
					case Position.TopLeft:
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.TopCenter));
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.MidLeft));
						break;
					case Position.TopCenter:
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.TopRight));
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.MidCenter));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.TopLeft));
						break;
					case Position.TopRight:
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.MidRight));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.TopCenter));
						break;
					case Position.MidLeft:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.TopLeft));
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.MidCenter));
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.BotLeft));
						break;
					case Position.MidCenter:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.TopCenter));
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.MidRight));
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.BotCenter));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.MidLeft));
						break;
					case Position.MidRight:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.TopRight));
						tiles[i].neighbours.Add(Direction.DOWN, TileAtPos(Position.MidCenter));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.BotRight));
						break;
					case Position.BotLeft:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.MidLeft));
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.BotCenter));
						break;
					case Position.BotCenter:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.MidCenter));
						tiles[i].neighbours.Add(Direction.RIGHT, TileAtPos(Position.BotLeft));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.BotRight));
						break;
					case Position.BotRight:
						tiles[i].neighbours.Add(Direction.UP, TileAtPos(Position.MidRight));
						tiles[i].neighbours.Add(Direction.LEFT, TileAtPos(Position.BotCenter));
						break;
				}
			}
			#endregion AssignNeighbours
		}

		void RandomiseTiles()
		{
			if (emptySlot == null)
			{
				emptySlot = tiles[UnityEngine.Random.Range(0, 9)];
				emptySlot.tile.gameObject.SetActive(false);
			}

			int blockedDir = -1;
			do
			{
				for (int i = 0; i < difficulties[difficulty].movements; i += 1)
				{
					int direction;
					do { direction = UnityEngine.Random.Range(0, 4); } while (!emptySlot.neighbours.ContainsKey((Direction)direction) || direction == blockedDir);

					SlideTile(emptySlot, (Direction)direction);
					emptySlot = emptySlot.neighbours[(Direction)direction];
					blockedDir = (direction + 2) % 4;
				}
			}
			while (GetTileDisplacements() < difficulties[difficulty].posChanges);
		}

		void ResetTiles()
		{
			List<SlidingTile> tileObjs = new List<SlidingTile>();
			foreach (TileSlot tile in tiles)
			{
				tileObjs.Add(tile.tile);
			}

			foreach (SlidingTile tile in tileObjs)
			{
				TileAtPos(tile.position).tile = tile;
				SetTileCoords(tile, tile.position);
			}
		}

		void SlideTile(TileSlot movedTile, Direction direction)
		{
			if (!movedTile.neighbours.ContainsKey(direction)) { return; }

			SlidingTile tileA = movedTile.tile;
			SlidingTile tileB = movedTile.neighbours[direction].tile;

			movedTile.tile = tileB;
			movedTile.neighbours[direction].tile = tileA;

			SetTileCoords(tileA, movedTile.neighbours[direction].position);
			SetTileCoords(tileB, movedTile.position);
		}

		void SetTileCoords(SlidingTile tile, Position pos)
		{
			Vector3 tileCoord = new Vector3(offset[pos].x * tileSize, 0, offset[pos].y * tileSize);
			tile.transform.localPosition = tileCoord;
		}

		TileSlot TileAtPos(Position pos)
		{
			return tiles[((int)pos) - 1];
		}

		public int GetTileDisplacements()
		{
			int displaced = 0;
			for (int i = 0; i < tiles.Count; i += 1)
			{
				if (tiles[i].position != tiles[i].tile.position)
				{
					displaced += 1;
				}
			}
			return displaced;
		}

		public void RandomTest()
		{
			if (emptySlot != null)
			{
				emptySlot.tile.gameObject.SetActive(true);
				emptySlot = null;
			}

			ResetTiles();
			RandomiseTiles();
		}

		public class TileSlot
		{
			public Position position;
			public SlidingTile tile;
			public Dictionary<Direction, TileSlot> neighbours = new Dictionary<Direction, TileSlot>();

			public TileSlot(SlidingTile _tile, Position _pos)
			{
				tile = _tile;
				position = _pos;
				Debug.Log(position.ToString());
			}
		}

		public class SlideDifficulty
		{
			public int movements;
			public int posChanges;

			public SlideDifficulty(int move, int change)
			{
				movements = move;
				posChanges = change;
			}
		}
	}
}


