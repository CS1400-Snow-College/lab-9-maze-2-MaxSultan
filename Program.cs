// Max Sultan, Nov 6 2025, Lab 9: Maze 2

void ProcessNextTick(string[] mapRows, (int leftDelta, int topDelta) position) {
    Console.SetCursorPosition(0,0);
    foreach(string row in mapRows)
        Console.WriteLine(row);
    Console.SetCursorPosition(position.leftDelta, position.topDelta);
}

void Main()
{
    Game Game = new Game();
    Map Map = new Map();
    Game.PrintScore(Map.Rows.Length);
    Character BadGuy1 = new Character((14, 5));
    Character BadGuy2 = new Character((38, 15));
    List<Character> BadGuys = new List<Character> { BadGuy1, BadGuy2 };

    Console.SetCursorPosition(Map.Bounds["left"] + 1, Map.Bounds["top"] + 1);

    do {
        ConsoleKey inputKey = Console.ReadKey(true).Key;
        (int leftDelta, int topDelta) proposedPosition = (Console.CursorLeft, Console.CursorTop);
        (int leftDelta, int topDelta) currentPlayerPosition = (Console.CursorLeft, Console.CursorTop);
        if(inputKey == ConsoleKey.Escape) break;
        if (inputKey == ConsoleKey.UpArrow) proposedPosition.topDelta--;
        else if (inputKey == ConsoleKey.DownArrow) proposedPosition.topDelta++;
        else if(inputKey == ConsoleKey.LeftArrow) proposedPosition.leftDelta--;
        else if (inputKey == ConsoleKey.RightArrow)	proposedPosition.leftDelta++;
        
        bool withinMap = proposedPosition.leftDelta >= Map.Bounds["left"] && proposedPosition.leftDelta <= Map.Bounds["right"] && proposedPosition.topDelta >= Map.Bounds["top"] && proposedPosition.topDelta <= Map.Bounds["bottom"];
        if (!withinMap) continue;
        bool notProjectedBlockedSpace = !(new List<char> {Map.Legend["wall"], Map.Legend["gate"]}.Contains(Map.Rows[proposedPosition.topDelta][proposedPosition.leftDelta]));
        if (!notProjectedBlockedSpace) continue;
        
        foreach(Character BadGuy in BadGuys) {
            while(true) {
                int leftDeltaOffset = 1 * (BadGuy.Left ? -1 : 1);
                (int leftDelta, int topDelta) newPosition = (BadGuy.Position.leftDelta + leftDeltaOffset, BadGuy.Position.topDelta);
                if(BadGuy.CheckMove(newPosition, Map.Rows, Map.Legend, Map.Bounds)){
                    Map.UpdateCharacter(Map.Legend["badGuy"], BadGuy.Position, newPosition);
                    BadGuy.Move(newPosition);
                    break;
                } 
                BadGuy.Left = !BadGuy.Left;
            }
        }

        foreach(Character BadGuy in BadGuys)
            if (BadGuy.Position == currentPlayerPosition) Game.EndGame("You lost");
        
        bool playerWon = Map.Rows[proposedPosition.topDelta][proposedPosition.leftDelta] == Map.Legend["win"];
        if (playerWon) Game.EndGame("YOU WON!");

        bool gemColision = Map.Rows[proposedPosition.topDelta][proposedPosition.leftDelta] == Map.Legend["gem"];
        if(gemColision) {
            Game.Score += 200;
            Map.ClearMapSpace(proposedPosition);
            Game.PrintScore(Map.Rows.Length);
        }
        
        bool coinCollision = Map.Rows[proposedPosition.topDelta][proposedPosition.leftDelta] == Map.Legend["coin"];
        if(coinCollision) {
            Game.Score += 100;
            Map.ClearMapSpace(proposedPosition);
            Game.PrintScore(Map.Rows.Length);

            if(Game.Score == 1000) Map.OpenGate();
        }

        if (Game.Continue)
            ProcessNextTick(Map.Rows, proposedPosition);

    } while(Game.Continue);
}

Main();

public class Character {
    public (int leftDelta, int topDelta) Position { get; set; }
    public char Symbol { get; }
    public bool Left { get; set; }
    public Character((int leftDelta, int topDelta) position) 
    {
        Position = position;
        Symbol = '%';
        Left = true;
    }

    public bool CheckMove((int leftDelta, int topDelta) newPosition, string[] mapRows, Dictionary<string, char> mapLegend, Dictionary<string, int> mapBounds){
        bool withinMap = newPosition.leftDelta > mapBounds["left"] && newPosition.leftDelta <= mapBounds["right"] && newPosition.topDelta >= mapBounds["top"] && newPosition.topDelta <= mapBounds["bottom"];
        if (!withinMap) return false;
        bool notProjectedBlockedSpace = !(new List<char> {mapLegend["wall"], mapLegend["gate"]}.Contains(mapRows[newPosition.topDelta][newPosition.leftDelta]));
        if (!notProjectedBlockedSpace) return false;
        return true;
    }

    public void Move((int leftDelta, int topDelta) newPosition){
        Position = newPosition;
    }
}

public class Map {
    public string[] Rows { get; set; }
    public Dictionary<string, char> Legend { get; }
    public Dictionary<string, int> Bounds { get; }

    public Map(){
        Rows = File.ReadAllLines("./maze.txt");
        Legend = new Dictionary<string, char>
        {
            {"win", '#'},
            {"wall", '*'},
            {"badGuy", '%'},
            {"coin", '^'},
            {"gem", '$'},
            {"gate", '|'}
        };
        Bounds = new Dictionary<string, int>
        {
            {"left", 0},
            {"right", Rows[0].Length - 1},
            {"top", 0},
            {"bottom", Rows.Length - 1},
        };
            Console.SetCursorPosition(0,0);
            foreach(string row in Rows)
                Console.WriteLine(row);
    }

    public void ClearMapSpace((int leftDelta, int topDelta) position) {
        Rows = Rows.Select((row, idx) => idx == position.topDelta ? row.Remove(position.leftDelta, 1).Insert(position.leftDelta, " ") : row).ToArray();
    }

    public void OpenGate(){
        Rows = Rows.Select(row => row.Replace('|', ' ')).ToArray();
    }

    public void UpdateCharacter(char character, (int leftDelta, int topDelta) oldPosition, (int leftDelta, int topDelta) newPosition){
        Rows = Rows
        .Select((row, idx) => idx == oldPosition.topDelta ? row.Remove(oldPosition.leftDelta, 1).Insert(oldPosition.leftDelta, " ") : row)
        .ToArray()
        .Select((row, idx) => idx == newPosition.topDelta ? row.Remove(newPosition.leftDelta, 1).Insert(newPosition.leftDelta, $"{character}") : row)
        .ToArray();
    }
}

public class Game {
    public int Score { get; set; }
    public bool Continue { get; set; }
    private DateTime StartTime { get; set; }
    public Game(){
        Score = 0;
        Continue = true;
        StartTime = DateTime.Now;
        Console.Title = "Maze Game";
        Console.Clear();
    }

    public void PrintScore(int mapRowsEnd) {
        Console.SetCursorPosition(0, mapRowsEnd);
        Console.WriteLine($"Score: {Score}");
    }

    public void EndGame(string message){
        DateTime endTime = DateTime.Now;
        Continue = false;
        Console.Clear();
        Console.WriteLine(message);
        TimeSpan gameDuration = endTime - StartTime;
        Console.WriteLine($"You scored {Score} points in {gameDuration.Seconds} seconds");
    }
}