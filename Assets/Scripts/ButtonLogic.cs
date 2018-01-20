using UnityEngine;
using System.Collections;

public class ButtonLogic : MonoBehaviour
{

	public static bool buttZoomIn = false;
	public static bool buttZoomOut = false;
	public static bool buttUp = false;
	public static bool buttDown = false;
	public static bool buttLeft = false;
	public static bool buttRight = false;


	public void ZoomInButtDown()
	{
		buttZoomIn = true;
	}

	public void ZoomInButtUp()
	{
		buttZoomIn = false;
	}

	public void ZoomOutButtDown()
	{
		buttZoomOut = true;
	}

	public void ZoomOutButtUp()
	{
		buttZoomOut = false;
	}

	public void UpButtDown()
	{
		buttUp = true;
	}

	public void UpButtUp()
	{
		buttUp = false;
	}

	public void DownButtDown()
	{
		buttDown = true;
	}

	public void DownButtUp()
	{
		buttDown = false;
	}

	public void LeftButtDown()
	{
		buttLeft = true;
	}

	public void LeftButtUp()
	{
		buttLeft = false;
	}

	public void RightButtDown()
	{
		buttRight = true;
	}

	public void RightButtUp()
	{
		buttRight = false;
	}

}
