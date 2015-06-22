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

	public delegate void SpecifiedProblemsReceived (Problem[] problems);
	/// <summary>
	/// Occurs when the server responds to a get specified problems request.
	/// The problems will meet the specifications given when you sent the
	/// specified problems request.
	/// </summary>
	public static event SpecifiedProblemsReceived onSpecifiedProblemsReceived;

	/// <summary>
	/// Invokes the specified problems received event.  This is invoked by a 
	/// Thinksy-internal specified problem response handler.
	/// </summary>
	/// <param name="problems">Problems.</param>
	public static void InvokeSpecifiedProblemsReceived(Problem[] problems)
	{
		if (onSpecifiedProblemsReceived != null)
			onSpecifiedProblemsReceived (problems);
	}

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
		if (onEncouragementReceived != null)
			onEncouragementReceived (encouragementParts);
	}
}