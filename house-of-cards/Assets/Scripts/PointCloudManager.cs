﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PointCloudManager : MonoBehaviour {

	// File
	public string dataPath;
	private string filename = "/100";
	public Material matVertex;

	// GUI
	private float progress = 0;
	private string guiText;
	public bool loaded = false;

	// PointCloud
	public GameObject pointCloud;

	public float scale = 1;
	public bool invertYZ = false;
	public bool forceReload = false;

	public int numPoints;
	public int numPointGroups;
	private int limitPoints = 65000;

	private Vector3[] points;
	private Color[] colors;
	private Vector3 minValue;
	
	void Start () {
		// Create Resources folder
		createFolders ();
	}

	public void load() {
		loaded = false;

		// Get Filename
		filename = Path.GetFileName(dataPath);

		loadScene ();
	}

	void loadScene(){
#if UNITY_EDITOR
        // Check if the PointCloud was loaded previously
        if(!Directory.Exists (Application.dataPath + "/Resources/PointCloudMeshes/" + filename)){
			UnityEditor.AssetDatabase.CreateFolder ("Assets/Resources/PointCloudMeshes", filename);
			loadPointCloud ();
		} else if (forceReload){
			UnityEditor.FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/PointCloudMeshes/" + filename);
			UnityEditor.AssetDatabase.Refresh();
			UnityEditor.AssetDatabase.CreateFolder ("Assets/Resources/PointCloudMeshes", filename);
			loadPointCloud ();
		} else
#endif
            // Load stored PointCloud
            loadStoredMeshes();
	}
	
	
	void loadPointCloud(){
		// Check what file exists
		if (File.Exists (Application.dataPath + dataPath + ".off")) 
			// load off
			StartCoroutine ("loadOFF", dataPath + ".off");
		else 
			Debug.Log ("File '" + dataPath + "' could not be found"); 
		
	}
	
	// Load stored PointCloud
	void loadStoredMeshes(){

		Debug.Log ("Using previously loaded PointCloud: " + filename);

        Destroy(pointCloud);

        pointCloud = Instantiate(Resources.Load ("PointCloudMeshes/" + filename)) as GameObject;
        pointCloud.name = pointCloud.name.Substring (0, pointCloud.name.IndexOf ("("));
        pointCloud.transform.position = Vector3.zero;
        if (filename == "city" || filename == "culdesac")
        {
            pointCloud.transform.eulerAngles = new Vector3(270, 0, 90);
        }
        else
        {
            pointCloud.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        pointCloud.transform.localScale = new Vector3(0.0002f, 0.0002f, 0.0002f);

		loaded = true;
	}
	
	// Start Coroutine of reading the points from the OFF file and creating the meshes
	IEnumerator loadOFF(string dPath){

		// Read file
		StreamReader sr = new StreamReader (Application.dataPath + dPath);
		sr.ReadLine (); // OFF
		string[] buffer = sr.ReadLine ().Split(); // nPoints, nFaces
		
		numPoints = int.Parse (buffer[0]);
		points = new Vector3[numPoints];
		colors = new Color[numPoints];
		minValue = new Vector3();
		
		for (int i = 0; i< numPoints; i++){
			buffer = sr.ReadLine ().Split (',');

			if (!invertYZ)
				points[i] = new Vector3 (float.Parse (buffer[0])*scale, float.Parse (buffer[1])*scale,float.Parse (buffer[2])*scale) ;
			else
				points[i] = new Vector3 (float.Parse (buffer[0])*scale, float.Parse (buffer[2])*scale,float.Parse (buffer[1])*scale) ;
			
			if (buffer.Length > 5)
				colors [i] = new Color (int.Parse (buffer [3]) / 255.0f, int.Parse (buffer [4]) / 255.0f, int.Parse (buffer [5]) / 255.0f);
			else if (buffer.Length == 4) {
				var intensity = int.Parse(buffer [3]);
				colors [i] = new Color (intensity * 1.1f / 255.0f, intensity * 1.6f / 255.0f, 200 / 255.0f);
			} else
				colors[i] = Color.cyan;

			// GUI
			progress = i *1.0f/(numPoints-1)*1.0f;
			if (i%Mathf.FloorToInt(numPoints/20) == 0){
				guiText=i.ToString() + " out of " + numPoints.ToString() + " loaded";
				yield return null;
			}
		}

		
		// Instantiate Point Groups
		numPointGroups = Mathf.CeilToInt (numPoints*1.0f / limitPoints*1.0f);

		pointCloud = new GameObject (filename);

		for (int i = 0; i < numPointGroups-1; i ++) {
			InstantiateMesh (i, limitPoints);
			if (i%10==0){
				guiText = i.ToString() + " out of " + numPointGroups.ToString() + " PointGroups loaded";
				yield return null;
			}
		}
		InstantiateMesh (numPointGroups-1, numPoints- (numPointGroups-1) * limitPoints);

#if UNITY_EDITOR
        //Store PointCloud
        UnityEditor.PrefabUtility.CreatePrefab ("Assets/Resources/PointCloudMeshes/" + filename + ".prefab", pointCloud);
#endif

        afterLoad (dPath);

		loaded = true;
	}

	
	void InstantiateMesh(int meshInd, int nPoints){
		// Create Mesh
		GameObject pointGroup = new GameObject (filename + meshInd);
		pointGroup.AddComponent<MeshFilter> ();
		pointGroup.AddComponent<MeshRenderer> ();
		pointGroup.GetComponent<Renderer>().material = matVertex;

		pointGroup.GetComponent<MeshFilter> ().mesh = CreateMesh (meshInd, nPoints, limitPoints);
		pointGroup.transform.parent = pointCloud.transform;

#if UNITY_EDITOR
        // Store Mesh
        UnityEditor.AssetDatabase.CreateAsset(pointGroup.GetComponent<MeshFilter> ().mesh, "Assets/Resources/PointCloudMeshes/" + filename + @"/" + filename + meshInd + ".asset");
		UnityEditor.AssetDatabase.SaveAssets ();
		UnityEditor.AssetDatabase.Refresh();
#endif
    }

	void afterLoad(string dPath) {
		// remove old object
		string[] buffer = dPath.Split ('/');
		buffer = buffer [buffer.Length - 1].Split ('.');

        if (buffer[0] == "1")
        {
            try
            {
                var past = GameObject.Find("city");
                if (past != null)
                {
                    Destroy(past);
                }

                past = GameObject.Find("culdesac");
                if (past != null)
                {
                    Destroy(past);
                }
            }
            catch (Exception) { }
        }

        if (buffer[0] == "city")
        {
            buffer[0] = "2102";
        }

        try
        {
            var index = int.Parse(buffer[0]);
            if (index == 1)
            {
                index = 2102;
            }
            var past = GameObject.Find((index - 1).ToString());
            if (past != null)
            {
                print("Deleting " + (index - 1).ToString());
                Destroy(past);
            }
        }
        catch (Exception) { }
	}

	Mesh CreateMesh(int id, int nPoints, int limitPoints){
		
		Mesh mesh = new Mesh ();
		
		Vector3[] myPoints = new Vector3[nPoints]; 
		int[] indecies = new int[nPoints];
		Color[] myColors = new Color[nPoints];

		for(int i=0;i<nPoints;++i) {
			myPoints[i] = points[id*limitPoints + i] - minValue;
			indecies[i] = i;
			myColors[i] = colors[id*limitPoints + i];
		}


		mesh.vertices = myPoints;
		mesh.colors = myColors;
		mesh.SetIndices(indecies, MeshTopology.Points,0);
		mesh.uv = new Vector2[nPoints];
		mesh.normals = new Vector3[nPoints];


		return mesh;
	}

	void calculateMin(Vector3 point){
		if (minValue.magnitude == 0)
			minValue = point;


		if (point.x < minValue.x)
			minValue.x = point.x;
		if (point.y < minValue.y)
			minValue.y = point.y;
		if (point.z < minValue.z)
			minValue.z = point.z;
	}

	void createFolders(){
#if UNITY_EDITOR
        if(!Directory.Exists (Application.dataPath + "/Resources/"))
			UnityEditor.AssetDatabase.CreateFolder ("Assets", "Resources");

		if (!Directory.Exists (Application.dataPath + "/Resources/PointCloudMeshes/"))
			UnityEditor.AssetDatabase.CreateFolder ("Assets/Resources", "PointCloudMeshes");
#endif
    }


	void OnGUI(){


		if (!loaded){
			GUI.BeginGroup (new Rect(Screen.width/2-100, Screen.height/2, 400.0f, 20));
			GUI.Box (new Rect (0, 0, 200.0f, 20.0f), guiText);
			GUI.Box (new Rect (0, 0, progress*200.0f, 20), "");
			GUI.EndGroup ();
		}
	}

}
