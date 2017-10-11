using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBox
{
	public class SlidingTile : MonoBehaviour
	{
		[SerializeField]
		SpriteRenderer tileImage;
		[HideInInspector]
		public SlidingTilePuzzle.Position position;

		public void SetImage(Sprite newImage)
		{
			tileImage.sprite = newImage;
		}
	}
}
