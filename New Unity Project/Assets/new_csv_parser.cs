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
			Instantiate(PointPrefab, position, rotation);
			points.Add(position);
		    counter++;
		}  
		lineRenderer = this.lineRenderer;
		lineRenderer.positionCount = counter - 1;

		int counter2 = 0;
		foreach (Vector3 point in points) {
			lineRenderer.SetPosition(counter2, point);
			counter2++;
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
