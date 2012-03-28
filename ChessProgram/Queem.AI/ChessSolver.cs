using System;
using Queem.Core;
using Queem.Core.ChessBoard;
using DebutMovesHolder;

namespace Queem.AI
{
	public class ChessSolver
	{
		protected DebutGraph debutGraph;
	
		public ChessSolver (DebutGraph graph)
		{
			this.debutGraph = graph;
		}
		
		protected bool TryFindDebutMove(out Move debutMove)
		{
			debutMove = null;
            return false;
		}
		
		
		public Move SolveProblem(GameProvider provider, Color color, int maxdepth)
		{
			Move bestMove;
			
			if (this.TryFindDebutMove(out bestMove))
				return bestMove;
				
			FixedArray moves;
				
			if (provider.PlayerBoard1.FigureColor == color)
				moves = provider.PlayerBoard1.GetMoves(provider.PlayerBoard2, 
					provider.History.GetLastMove(), 
					MovesMask.AllMoves);
			else
				moves = provider.PlayerBoard1.GetMoves(provider.PlayerBoard1, 
					provider.History.GetLastMove(), 
					MovesMask.AllMoves);
					
			Random rand = new Random(DateTime.Now.Millisecond);
			return moves.InnerArray[rand.Next(moves.InnerArray.Length)];
		}
	}
}

