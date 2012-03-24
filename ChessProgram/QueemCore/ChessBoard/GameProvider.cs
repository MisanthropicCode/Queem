using System;
using QueemCore.History;

namespace QueemCore.ChessBoard
{
	public class GameProvider
	{
		protected PlayerBoard PlayerBoard1 { get; private set; }
		protected PlayerBoard PlayerBoard2 { get; private set; }
				
		protected MovesHistory History { get; private set; }
		
		protected PlayerBoard[] playerBoards;
	
		public GameProvider ()
		{
			this.PlayerBoard1 = new PlayerBoard(PlayerPosition.Down, Color.White);
			this.PlayerBoard2 = new PlayerBoard(PlayerPosition.Up, Color.Black);
			
			this.History = new MovesHistory();
			
			this.playerBoards = new PlayerBoard[2];
			this.playerBoards[(int)Color.White] = this.PlayerBoard1;
			this.playerBoards[(int)Color.Black] = this.PlayerBoard2;
		}
		
		
		/*
         * Possible problematic situations
         * 
         * ----------Pawn-----------
         *  1. move to in-passing state
         *  2. move from in-passing state
         *  3. kill other pawn that is in passing state
         *  4. just move
         *  5. kill a rook in state with possible castling
         * 
         * 
         * ----------Rook----------
         *  1. kill a pawn in a passing state
         *  2. move from castling state
         *  3. be a part of a castling
         *  4. kill other rook in a castling state
         *  5. just move
         * 
         * 
         * ----------King----------
         *  1. move from a castling state
         *  2. kill a pawn in a passing state
         *  3. kill a rook in a castling state
        */
		public void ProcessMove(Move move, Color color)
		{
			var oppositeColor = (Color)(1 - (int)color);
			
			var playerBoard1 = this.playerBoards[(int)color];
			var playerBoard2 = this.playerBoards[(int)oppositeColor];
			
			var figureMoving = playerBoard1.Figures[(int)move.From];
			var destinationFigure = playerBoard2.Figures[(int)move.To];
			
			var lastMove = this.History.GetLastMove();
			
			this.History.AddItem(move);

			var deltaChange = this.History.GetCurrentDeltaChange();
			var moveChange = deltaChange.GetNext(MoveAction.Move);
			
			moveChange.Square = move.From;
			moveChange.AdditionalSquare = move.To;
			moveChange.FigureColor = color;
			moveChange.FigureType = figureMoving;
			moveChange.Data = playerBoard1.GetBoardProperty(figureMoving);
			
			playerBoard1.ProcessMove(move, figureMoving);
			
			if (destinationFigure != Figure.Nobody)
			{
				var killChange = deltaChange.GetNext(MoveAction.Deletion);
			
				killChange.Square = move.To;
				killChange.Data = playerBoard2.GetBoardProperty(destinationFigure);
				killChange.FigureType = destinationFigure;
				killChange.FigureColor = oppositeColor;
				
				playerBoard2.RemoveFigure(move.To, destinationFigure);
				
				return;
			}
			
			if (move.Type == MoveType.EpCapture)
			{
				var passingKillChange = deltaChange.GetNext(MoveAction.Deletion);
				
				passingKillChange.Square = lastMove.To;
				passingKillChange.FigureType = Figure.Pawn;
				passingKillChange.FigureColor = oppositeColor;
				
				playerBoard2.RemoveFigure(passingKillChange.Square, Figure.Pawn);
				
				return;
			}
			
			if (move.Type == MoveType.KingCastle)
			{
				int moveTo = (int)move.To;
				int moveFrom = (int)move.From;				
				// will be +2 or -2
				int difference = moveTo - moveFrom;
				// same as sign of difference
				difference = difference / 2;
				var rookMoveChange = deltaChange.GetNext(MoveAction.Move);
				
				// rook target
				rookMoveChange.AdditionalSquare = moveTo - difference;
				
				// rook source
				// diff -> [1 -> 1, -1 -> 0]
				difference = (difference + 1) / 2;
				rookMoveChange.Square = (Square)((moveFrom / 8) * 8 + difference*7);
				
				rookMoveChange.FigureType = Figure.Rook;
				rookMoveChange.FigureColor = color;
				rookMoveChange.Data = playerBoard1.GetBoardProperty(Figure.Rook);
				
				playerBoard1.ProcessMove(
					new Move(rookMoveChange.Square, rookMoveChange.AdditionalSquare),
					Figure.Rook);
					
				return;
			}
		}		
	}
}
