namespace WinForm2048.Models
{
  class Board
  {
    public int[,] TheBoard { get; set; } = new int[9, 9];

    public Board()
    {
      // Initialization of the board
      for (int i = 1; i < 9; i++)
      {
        for (int j = 1; j < 9; j++)
        {
          TheBoard[i, j] = 0;
        }
      }
    }

    public int NumberOfTilePerColumn { get; set; }
    public int NumberOfTilePerLine { get; set; }

    public int PositionOfAColumn { get; set; }
    public int PositionOfALine { get; set; }
  }
}
