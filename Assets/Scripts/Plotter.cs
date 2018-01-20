using UnityEngine;
//using System.Collections;
using System;
using UnityEngine.UI;
//using UnityEditor;

public class Plotter : MonoBehaviour {

	public int base_iters;
	public int iterations;
	public double SPEED;
	public double ZOOM;
	public float MARGIN;
	public int height;
	public int width;
	float ASPECT_RATIO;
	double maxX = 2;
	double minX = -2;
	double maxY = 2;
	double minY = -2;
	double halfX { get {return (maxX - minX) / 2; } }
	double halfY { get { return (maxY - minY) / 2; } }
	double centerX
	{
		get
		{
			// returns mid point between min/max
			return (maxX - minX) / 2;
		}
		set
		{
			// changes min/max values
			double hfX = halfX;
			maxX += hfX * value;
			minX += hfX * value;
		}
	}
	double centerY
	{
		get
		{
			// returns mid point between min/max
			return (maxY - minY) / 2;
		}
		set
		{
			// changes min/max values
			double hfY = halfY;
			maxY += hfY * value;
			minY += hfY * value;
		}
	}
	double zoom
	{
		get { return ZOOM; }
		set
		{
			//calculate zoom level
			ZOOM =  2 / halfX;
			// set UI zoom text to zoom level after formatting nicely
			UIText.zoom_level_text.text = Convert.ToString(Mathf.Floor((float)ZOOM));
			
			// recalculate mins/maxs based on new zoom level
			double hfX = halfX;
			double hfY = halfY;
			maxX -= hfX * value;
			minX += hfX * value;
			maxY -= hfY * value;
			minY += hfY * value;
		}
	}

	public Camera Cam;
	public int numColors = 8;
	Color32 color1;
	Color32 color2;
	Material[] mats;
	public GameObject PixelCube;
	public GameObject[,] pixArr;
	public int[,] isInSet;
	public int[,] numIters;

    void Start() {

		//height = Screen.height / 8;
		//width = Screen.width / 8;
		ASPECT_RATIO = (float)Screen.width / (float)Screen.height;
		height = 100;
		width = Convert.ToInt16(height * ASPECT_RATIO);
		//MARGIN = (Screen.width - Screen.height) / (Screen.width * 2);
		//Cam.rect = new Rect(MARGIN, 0, 1 - MARGIN * 2, 1);
		// set plot size to largest side of screen
		//size = (Screen.width > Screen.height) ? Screen.width / 8 : Screen.height / 8;

		color1 = Color.blue;
		color2 = new Color32(222, 4, 3, 1);

		mats = new Material[numColors];
		for (int i = 0; i < numColors; i++)
		{
			mats[i] = new Material(Shader.Find("Diffuse"));
			//AssetDatabase.CreateAsset(mats[i], "Assets/Materials/Mat" + i + ".mat");
			if (i == 0)
				mats[i].color = new Color32(166, 165, 163, 1);
			else if (i == numColors - 1)
				mats[i].color = Color.black;
			else
				mats[i].color = Color32.Lerp(color1, color2, 1f / (float)i);
		}

		// instantiate and populate array of pixel objects the first time update is run
		pixArr = new GameObject[width, width];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < width; y++)
			{
				Vector3 pos = new Vector3(-width / 2 + x + .5f, -width / 2 + y + .5f, 0);
				pixArr[x, y] = GameObject.Instantiate(PixelCube, pos, Quaternion.identity) as GameObject;
			}
		}

		ColorPixels();
	}

    void Update() {

		// catch back button and escape key to close app
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
		// modify maths based on user input
		if (Input.GetKey(KeyCode.X) || ButtonLogic.buttZoomIn)
			zoom = SPEED;
		if (Input.GetKey(KeyCode.Z) || ButtonLogic.buttZoomOut)
			zoom = -SPEED;
		if (Input.GetKey(KeyCode.DownArrow) || ButtonLogic.buttDown)
			centerY = -SPEED;
		if (Input.GetKey(KeyCode.UpArrow) || ButtonLogic.buttUp)
			centerY = SPEED;
		if (Input.GetKey(KeyCode.LeftArrow) || ButtonLogic.buttLeft)
			centerX = -SPEED;
		if (Input.GetKey(KeyCode.RightArrow) || ButtonLogic.buttRight)
			centerX = SPEED;
		// only update plot if user input
		if (Input.anyKey)
			ColorPixels();

	}

	//void OnDrawGizmos()
	//{
	//	if (isInSet != null)
	//	{
	//		for (int x = 0; x < SIZE; x++)
	//		{
	//			for (int y = 0; y < SIZE; y++)
	//			{
	//				//Gizmos.color = (isInSet[x, y] == 1) ? Color.black : Color.white;

	//				// if number iterations is in middle third, color box grey
	//				int iters = numIters[x,y];
	//				int third = ITERATIONS / 3;
	//				if (iters <= third)
	//					Gizmos.color = Color.white;
	//				else if (iters > third && iters < third * 2)
	//					Gizmos.color = Color.grey;
	//				else
	//					Gizmos.color = Color.black;

	//				Vector3 pos = new Vector3(-SIZE / 2 + x + .5f, 0, -SIZE / 2 + y + .5f);
	//				Gizmos.DrawCube(pos, Vector3.one);
	//			}
	//		}
	//	}
	//}

	void ColorPixels() {

		// populate array grids with mandelbrot set and iterations data
        isInSet = new int[width, width];
		numIters = new int[width, width];
		// recalculate maximum number of iterations to increase resolution at higher zooms
		iterations = base_iters * ( 1 + (int) Mathf.Log(Convert.ToSingle(ZOOM)));
        FillArraysWithMaths();

		// assign material colors by linear interpolation of iterations
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < width; y++)
			{
				int iterInterval = (int) Mathf.Lerp(0, 7, ((float)numIters[x, y] / (float)(iterations)));
					pixArr[x, y].GetComponent<Renderer>().material = mats[iterInterval];
			}
		}

	}

	void FillArraysWithMaths() {

        for (int x = 0; x < width; x ++) {
            for (int y = 0; y < width; y ++) {
				isInSet[x,y] = (IsCoordInMandelbrotSet(x,y))? 1: 0;
            }
        }
    }

	bool IsCoordInMandelbrotSet(int x, int y) {
		
		// normalize plot coords to bounds
		double real = (minX + (x * (maxX - minX))/width);
		double imag = (minY + (y * (maxY - minY))/width);

		numIters[x, y] = HowManyIterations(real, imag);
		if (numIters[x, y] >= iterations)
		{
			return true; // The complex number is in the Mandelbrot set.
		}
		else
			return false; // The complex number is not in the Mandelbrot set.
	}

	// Returns number of iterations through mandelbrot equation.
	int HowManyIterations(double real, double imag)
	{
		double r = 0;
		double i = 0;
		int j = 0;

		while (r * r + i * i <= 4 && j < iterations)
		{
			double nextR = r * r - i * i + real;
			double nextI = 2 * r * i + imag;

			r = nextR;
			i = nextI;
			j++;
		}

		return j;
	}
}
