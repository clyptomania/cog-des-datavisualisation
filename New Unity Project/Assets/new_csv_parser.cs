using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class new_csv_parser : MonoBehaviour
{
    // The prefab for the data points that will be instantiated
    public GameObject PointPrefab;

	public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // CSV-Datei einlesen -> records[]
		int counter = 0;
		string line;
  
		// Read the file and display it line by line.  
		System.IO.StreamReader file =
		    new System.IO.StreamReader(@"/Users/kenrodenwaldt/Documents/HfG OF_am Main/Cog-Des/Jakob/SubjectData/Version 2_0_14_ET.csv");  
		// Skip header line
		file.ReadLine();
		List<Vector3> points = new List<Vector3>();
		Vector3 positionOld = new Vector3(0.0f, 0.0f, 0.0f);
		Color red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		Color blue = new Color(0.0f, 0.0f, 1.0f, 1.0f);
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(0.0f, 0.1f);
		curve.AddKey(1.0f, 0.1f);		

		while((line = file.ReadLine()) != null)
		{  
		    System.Console.WriteLine(line); 
		    Debug.Log(line); 
		    string[] linesplit = line.Split(',');
		    // Parse i.e. process an array of numbers of each parameter into more easily processed components
		    Vector3 position = new Vector3(
		    	float.Parse(linesplit[2]), 
		    	float.Parse(linesplit[3]), 
		    	float.Parse(linesplit[4]));
		    Quaternion rotation = new Quaternion(
		    	float.Parse(linesplit[5]), 
		    	float.Parse(linesplit[6]), 
		    	float.Parse(linesplit[7]), 
		    	float.Parse(linesplit[7]));
		    Vector3 gazeVecRelCam = new Vector3(
		    	float.Parse(linesplit[12]), 
		    	float.Parse(linesplit[13]), 
		    	float.Parse(linesplit[14]));
			
			// Error: Rotation Data (4D Vector/Quaternion) cannot be added in conjunction with Gaze Data (3D Vector)
			Vector3 gazeVec = rotation * gazeVecRelCam;
			// Just for fun (and testing):
			GameObject pointClone = Instantiate(PointPrefab, position, rotation);
			points.Add(position);
			if (counter != 0) {
				LineRenderer lineRendererClone = pointClone.AddComponent<LineRenderer>();
				lineRendererClone.positionCount = 2;
				lineRendererClone.SetPosition(0, positionOld);
				lineRendererClone.SetPosition(1, position);
				lineRendererClone.material = new Material(Shader.Find("Sprites/Default"));
				lineRendererClone.widthCurve = curve;
				float distance = Vector3.Distance(positionOld, position);
				float distanceNormed = distance / 5.0f;
				Color color = new Color(distanceNormed, 0.0f, 1-distanceNormed, 1.0f);
				lineRendererClone.SetColors(color, color);
				// if (distance > 1) {
				// 	lineRendererClone.SetColors(red, red);
				// } else {
				// 	lineRendererClone.SetColors(blue, blue);
				// }
			}
			positionOld = position;
		    counter++;
		}  
		  
		file.Close();  
		System.Console.WriteLine("There were {0} lines.", counter);  
		// Suspend the screen.  
		System.Console.ReadLine(); 
        // records[] -> vector3
        // vector3 -> indie3dweltwerfen
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
