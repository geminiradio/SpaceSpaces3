using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class CodeTools {
//
//	// TODO - generalize this to take any array as input
//	public static void Shuffle (ref CardinalDirection[] deck)
//	{
//		int rand;
//		CardinalDirection tmp;
//
//		for (int i=0; i<deck.Length; i++)
//		{
//			rand = Random.Range(0,deck.Length);
//			tmp = deck[rand];
//			deck[rand]=deck[i];
//			deck[i]=tmp;
//		}
//
//	}
//
//
//	// TODO - generalize this to take any list as input
//	public static void Shuffle (ref List<InteractableObject> deck)
//	{
//		int rand;
//		InteractableObject tmp;
//		
//		for (int i=0; i<deck.Count; i++)
//		{
//			rand = Random.Range(0,deck.Count);
//			tmp = deck[rand];
//			deck[rand]=deck[i];
//			deck[i]=tmp;
//		}
//		
//	}
//
//
//}

public class Interpolator2D {

	public Vector2 start, end;
	public float duration;

	private float interpStart;
	public float startTime
	{
		get { return interpStart; }
	}

	public bool complete
	{
		get { 
			return ( (Time.time - interpStart)>=duration );  
		}
	}

	public bool active
	{
		get {
			return ( ((Time.time - interpStart) < duration) && ((Time.time - interpStart) >= 0f) );
		}
	}

	private float sizeX, sizeY;
	private Vector2 returnValue;

	// this isn't a constructor - it's intended to be reused
	public void Initialize (Vector2 start_loc, Vector2 end_loc, float duration_loc)
	{
		start = new Vector2(start_loc.x, start_loc.y);
		end = new Vector2(end_loc.x, end_loc.y);
		duration = duration_loc;

		interpStart = Time.time;

		sizeX = end.x - start.x;
		sizeY = end.y - start.y;

		// resuse this vector structure
		returnValue = new Vector2();

	}

	public Vector2 Update ()
	{
		return Update (Time.time - startTime);
	}

	public Vector2 Update(float deltaTime)
	{
		float percent_complete = ( deltaTime / duration );

		if (percent_complete < 0f)
		{
			Debug.Log ("Warning: Interpolator called with negative time value.");
			return Vector2.zero;
		}
		else if (percent_complete < 1f)
		{
			returnValue.x = start.x + (sizeX * percent_complete);
			returnValue.y = start.y + (sizeY * percent_complete);
		}
		else   // percent_complete >= 1f, ie 100% complete
		{
			returnValue = end;
		}

		return returnValue;

	}
	
}
