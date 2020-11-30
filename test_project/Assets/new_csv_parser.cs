using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class new_csv_parser : MonoBehaviour
{
    // The prefab for the data points that will be instantiated
    public string inputfile;
	public GameObject PointPrefab;

	public GameObject HitpointPrefab;

	public int linesToBeSkipped = 10;

	public int metersToBeRaycasted = 10;

	private List<Vector3> positionList = new List<Vector3>();
	private List<Quaternion> rotationList = new List<Quaternion>();
	private List<Vector3> gazeDirectionList = new List<Vector3>();

	private int animationIndex = 0;
	private AnimationCurve curve = new AnimationCurve();
	Color redOpaque = new Color(1.0f, 0.0f, 0.0f, 0.4f);
	Color blueOpaque = new Color(0.0f, 0.0f, 1.0f, 0.1f);
	Color blueTransparent = new Color(0.0f, 0.0f, 1.0f, 0.0f);

	long millisecondsOld;
	// At least animationDurationInMs milliseconds have to pass until the next update is painted
	public long animationDurationInMs = 100;
	private List<float> distances;
	private float distanceMax;
	private Color colorOld = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	private Vector3 positionOld;



    // Start is called before the first frame update
    void Start()
    {
        // CSV-Datei einlesen -> records[]
		int counter = 0;
		string line;
  
		// Read the file and display it line by line.  
		System.IO.StreamReader file =
		    new System.IO.StreamReader(inputfile);  
		// Skip lines
		for (int i = 0; i < linesToBeSkipped; i++)
		{
			file.ReadLine();
		}
		Vector3 positionOld = new Vector3(0.0f, 0.0f, 0.0f);
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
		    	float.Parse(linesplit[8]));
		    Vector3 gazeDirection = new Vector3(
		    	float.Parse(linesplit[12]), 
		    	float.Parse(linesplit[13]), 
		    	float.Parse(linesplit[14]));
			positionList.Add(position);
			rotationList.Add(rotation);
			gazeDirectionList.Add(gazeDirection);

			positionOld = position;
		    counter++;
		}  

		distanceMax = 0;
		counter = 0;
		distances = new List<float>();
		foreach (Vector3 position in positionList)
		{
			if (counter != 0) {
				float distance = Vector3.Distance(positionOld, position);
				if (distance > distanceMax) {
					distanceMax = distance;
				}
				distances.Add(distance);
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
		millisecondsOld = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

	Color calcColor(float weightNormed) {
		if (weightNormed < 0.5f) {
			// weightNormed = 0 => r = 1, g = 0, 	b = 0
			// weigthNormed = 0.5 => r = 0, g = 1,	b = 0
			return new Color(1-weightNormed*2, weightNormed*2, 0.0f, 1.0f);
		} else {
			// weightNormed = 0.5 => r = 0,		g = 1, 	b = 0
			// weigthNormed = 1 => r = 0, 		g = 0,	b = 1
			return new Color(0.0f, 1-((weightNormed-0.5f)*2), (weightNormed-0.5f)*2, 1.0f);
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (animationIndex > positionList.Count) {
			return;
		}
		long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		long millisecondsSinceLastUpdate = milliseconds - millisecondsOld;
		if (millisecondsSinceLastUpdate < animationDurationInMs) {
			return;
		}

		Vector3 position = positionList[animationIndex];
		Quaternion rotation = rotationList[animationIndex];
		GameObject pointClone = Instantiate(PointPrefab, position, rotation);

		Vector3 gazeDirection = gazeDirectionList[animationIndex];
		Ray ray = new Ray(position, pointClone.transform.TransformDirection(gazeDirection));
		RaycastHit hitInfo;

		GameObject pointClone2 = Instantiate(PointPrefab, position, rotation);
		LineRenderer lineRendererGaze = pointClone2.AddComponent<LineRenderer>();
		lineRendererGaze.positionCount = 2;
		lineRendererGaze.SetPosition(0, position);
		lineRendererGaze.material = new Material(Shader.Find("Sprites/Default"));
		lineRendererGaze.widthCurve = curve;
		if (Physics.Raycast(ray, out hitInfo, metersToBeRaycasted)) {
			lineRendererGaze.SetPosition(1, hitInfo.point);
			lineRendererGaze.SetColors(redOpaque, redOpaque);
			GameObject hitPoint = Instantiate(HitpointPrefab, hitInfo.point, rotation);
		} else {
			lineRendererGaze.SetPosition(1, ray.origin + ray.direction * metersToBeRaycasted);
			lineRendererGaze.SetColors(blueOpaque, blueTransparent);
		}

		if (animationIndex > 0) {
			LineRenderer lineRendererPath = pointClone.AddComponent<LineRenderer>();
			lineRendererPath.positionCount = 2;
			lineRendererPath.SetPosition(0, positionOld);
			lineRendererPath.SetPosition(1, position);
			lineRendererPath.material = new Material(Shader.Find("Sprites/Default"));
			lineRendererPath.widthCurve = curve;

			float weight = (distances[animationIndex] + distances[animationIndex-1]) / 2;
			float weightNormed = weight / distanceMax;
			Color color = calcColor(weightNormed);
			lineRendererPath.SetColors(colorOld, color);
			colorOld = color;
		}
		positionOld = position;

		animationIndex++;
		millisecondsOld = milliseconds;
    }
}
