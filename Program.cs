// Max Sultan, Nov 6 2025, Lab 9: Maze 2

void ProcessNextTick(string[] mapRows, (int leftDelta, int topDelta) proposedPosition) {
    Console.SetCursorPosition(0,0);
    foreach(string row in mapRows)
        Console.WriteLine(row);
    Console.SetCursorPosition(proposedPosition.leftDelta, proposedPosition.topDelta);
}

void Main()
{
    Game Game = new Game();
    Map Map = new Map();

    Game.PrintScore(Map.Rows.Length);

    // Put the cursor (player) inside the map so arrow keys move the player on the map
    Console.SetCursorPosition(Map.Bounds["left"] + 1, Map.Bounds["top"] + 1);
    Character BadGuy1 = new Character((14, 5));
    Character BadGuy2 = new Character((38, 15));
    List<Character> BadGuys = new List<Character> { BadGuy1, BadGuy2 };

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
            if (BadGuy.Position == currentPlayerPosition) {
                Console.Clear();
                Console.WriteLine("You lost");
                return;
            }

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

            if(Game.Score == 1000)
                Map.OpenGate();
        }
        
        bool playerWon = Map.Rows[proposedPosition.topDelta][proposedPosition.leftDelta] == Map.Legend["win"];
        if (playerWon){
            Console.Clear();
            Console.WriteLine("YOU WON!");
            break;
        }

        ProcessNextTick(Map.Rows, proposedPosition);

    } while(true);
}

Main();

// (Detect the win) Break out of the loop, clear the screen, and print a congratulatory message if the current cell = "#" Be sure to tell the gamer their score and their time in the maze. (Make sure the program works and commit the changes to your repo.)

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
    public Game(){
        Score = 0;
        Console.Title = "Maze Game";
        Console.Clear();
    }

    public void PrintScore(int mapRowsEnd) {
        Console.SetCursorPosition(0, mapRowsEnd);
        Console.WriteLine($"Score: {Score}");
    }

}