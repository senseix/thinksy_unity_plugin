using UnityEngine;
using System.Collections;

public class ThinksyEvents : MonoBehaviour 
{
	public delegate void CategoryAdvancement();
	/// <summary>
	/// Occurs when a problem from a new, higher category gets displayed.
	/// </summary>
	public static event CategoryAdvancement onAdvanceCategory;

	public delegate void EncouragementReceived (ProblemPart[] encouragementParts);
	/// <summary>
	/// Occurs when an encouragement is received from the server.
	/// encouragmentParts are the text/image pieces which make up
	/// the encouragement.
	/// </summary>
	public static event EncouragementReceived onEncouragementReceived;

	/// <summary>
	/// Invokes the OnAdvaceCategory event.  ThinksyQuestionDisplay calls this
	/// whenever it displays a problem from a new, higher category.
	/// </summary>
	public static void InvokeCategoryAdvancement()
	{
		if (onAdvanceCategory != null)
			onAdvanceCategory ();
	}

	/// <summary>
	/// Invokes the encouragement received event.
	/// </summary>
	/// <param name="encouragementParts">the text/image pieces which make up
	/// the encouragement.</param>
	public static void InvokeEncouragementReceived(ProblemPart[] encouragementParts)
	{
		onEncouragementReceived (encouragementParts);
	}
}