using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleBox
{
	public class SlidingTile : MonoBehaviour
	{
		[SerializeField]
		SpriteRenderer tileImage;

		public void SetImage(Sprite newImage)
		{
			tileImage.sprite = newImage;
		}
	}
}
