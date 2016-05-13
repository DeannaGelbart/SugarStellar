using UnityEngine;
using System.Collections;

class MathUtilities
{

	/* Get the coordinates of a point on a circle at given angle. 
	  
	 From http://answers.unity3d.com/questions/33193/randomonunitcircle-.html 
	 
     Since this code was not written for SugarStellar, the SugarStellar open
     source license in README.md doesn't apply to this code.
	*/
	public static Vector2 getPointOnCircle (float angleDegrees, float radius)
	{
		float _x = 0;
		float _y = 0;
		float angleRadians = 0;
		Vector2 _returnVector;

		// convert degrees to radians
		angleRadians = angleDegrees * Mathf.PI / 180.0f;

		// get the 2D dimensional coordinates
		_x = radius * Mathf.Cos (angleRadians);
		_y = radius * Mathf.Sin (angleRadians);

		// derive the 2D vector
		_returnVector = new Vector2 (_x, _y);

		return _returnVector;
	}


}
