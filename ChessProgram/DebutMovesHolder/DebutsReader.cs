using System;
using System.IO;
using Queem.Core;

namespace DebutMovesHolder
{
	public static class DebutsReader
	{
		public static DebutGraph ReadDebuts (string fileName, PlayerPosition whitePos)
		{
			FileStream fs = new FileStream (fileName, FileMode.Open, FileAccess.Read);			
			StreamReader sr = new StreamReader (fs);
			
			DebutGraph dg = new DebutGraph ();
			
			string str;
			
			while ((str = sr.ReadLine ()) != null)
			{
				if (string.IsNullOrEmpty (str))
					continue;
				
				dg.AddMoves (whitePos, str.Split (','));
			}
			
			sr.Close ();
			
			return dg;
		}
	}
}

