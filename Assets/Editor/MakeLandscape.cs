﻿//C# Example

using UnityEngine;
using UnityEditor;
using System.Collections;

class MakeLandscape : EditorWindow {

	string minLong = "-79.65";
	string minLat = "37.6";
	string maxLong = "-79.35";
	string maxLat = "37.9";
	bool groupEnabled;
	float resolution = 90.0f;
	string modelName = "dem.obj";
	string mapName = "map.tiff";

	[MenuItem ("Tools/My Window")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(MakeLandscape));
	}

	void OnGUI () {
	
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
		minLong = EditorGUILayout.TextField ("Min Longitude", minLong);
		minLat = EditorGUILayout.TextField ("Min Latitude", minLat);
		maxLong = EditorGUILayout.TextField ("Max Longitude", maxLong);
		maxLat = EditorGUILayout.TextField ("Max Longitude", maxLat);

		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		resolution = EditorGUILayout.Slider ("Resolution (m)", resolution, 30, 90);
		modelName = EditorGUILayout.TextField ("Model Filename", modelName);
		mapName = EditorGUILayout.TextField ("Map Filename", mapName);
		EditorGUILayout.EndToggleGroup ();
	
	}
}