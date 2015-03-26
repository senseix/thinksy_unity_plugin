using UnityEngine;
using System.Collections;

public class ThinksyEvents : MonoBehaviour 
{
	public delegate void CategoryAdvancement();
	/// <summary>
	/// Occurs when a problem from a new, higher gets displayed.
	/// </summary>
	public static event CategoryAdvancement OnAdvanceCategory;

	/// <summary>
	/// Invokes the OnAdvaceCategory event.  ThinksyQuestionDisplay calls this
	/// whenever it displays a problem from a new, higher category.
	/// </summary>
	public static void InvokeCategoryAdvancement()
	{
		OnAdvanceCategory ();
	}
}
