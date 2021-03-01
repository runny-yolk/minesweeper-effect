using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
	public static void ForEach2D<T>(this T[,] arr, Action<Vector2Int, T> cb){
		for(int x = 0; x < arr.GetLength(0); x++) for(int y = 0; y < arr.GetLength(1); y++) cb(new Vector2Int(x,y), arr[x,y]);
	}
	public static void ForEachNeighbours<T>(this T[,] arr, int xs, int ys, Action<Vector2Int, T> cb){
		for(int xo = -1; xo < 2; xo++) for(int yo = -1; yo < 2; yo++) {
			if(xo == 0 && yo == 0) continue;
			var x = xs + xo;
			var y = ys + yo;
			if(
				x < 0 || y < 0 ||
				x >= arr.GetLength(0) || y >= arr.GetLength(1)
			) continue;
			cb(new Vector2Int(x,y), arr[x,y]);
		}
	}

	private static System.Random rng = new System.Random();  
	public static void Shuffle<T>(this IList<T> list) {  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}