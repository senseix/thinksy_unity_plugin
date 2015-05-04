using System;
using UnityEngine;

public class ImageProblemPart : ProblemPart
{
	public ImageProblemPart (Senseix.Message_v2.Atom.Atom atom) : base(atom)
	{
		
	}
	
	public override Sprite GetSprite()
	{
		string filepath = GetImageFilepath ();
		//Debug.Log (filepath);
		Sprite sprite = Resources.Load<Sprite> (filepath);
		return sprite;
	}

	public Texture2D GetImage()
	{
		return GetSprite ().texture;
	}

	public override bool HasSprite()
	{
		return true;
	}

	private string GetImageFilename()
	{
		Senseix.Message_v2.Atom.Atom atom = GetAtom ();
		if (atom.image_atom.filename == "")
			return "dog";
		return atom.image_atom.filename;
	}


	private string GetImageFilepath()
	{
		string filename = GetImageFilename();
		string filepath = System.IO.Path.Combine("ProblemParts/", filename);
		return filepath;
	}
}