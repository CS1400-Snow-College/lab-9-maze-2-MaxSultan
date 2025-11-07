// Max Sultan, Nov 6 2025, Lab 9: Maze 2
void Main()
{
    Console.Title = "Maze Game";
    Console.Clear();
    string[] mapRows = File.ReadAllLines("./maze.txt");
    foreach(string row in mapRows)
        Console.WriteLine(row);

    Dictionary<string, int> mapBounds = new Dictionary<string, int>
    {
        {"left", 0},
        {"right", mapRows[0].Length - 1},
        {"top", 0},
        {"bottom", mapRows.Length - 1},
    };

    Dictionary<string, char> mapLegend = new Dictionary<string, char>
    {
        {"win", '#'},
        {"wall", '*'},
        {"badGuy", '%'},
        {"coin", '^'},
        {"gems", '$'},
        {"gate", '|'}
    };

    // Put the cursor (player) inside the map so arrow keys move the player on the map
    Console.SetCursorPosition(mapBounds["left"] + 1, mapBounds["top"] + 1);
    Character BadGuy1 = new Character((14, 5), mapLegend["badGuy"]);
    Character BadGuy2 = new Character((38, 15), mapLegend["badGuy"]);
    List<Character> BadGuys = new List<Character> { BadGuy1, BadGuy2 };

    do {
        ConsoleKey inputKey = Console.ReadKey(true).Key;
        (int leftDelta, int topDelta) proposedPosition = (Console.CursorLeft, Console.CursorTop);
        if(inputKey == ConsoleKey.Escape) break;
        if (inputKey == ConsoleKey.UpArrow) proposedPosition.topDelta--;
        else if (inputKey == ConsoleKey.DownArrow) proposedPosition.topDelta++;
        else if(inputKey == ConsoleKey.LeftArrow) proposedPosition.leftDelta--;
        else if (inputKey == ConsoleKey.RightArrow)	proposedPosition.leftDelta++;
        
        bool withinMap = proposedPosition.leftDelta >= mapBounds["left"] && proposedPosition.leftDelta <= mapBounds["right"] && proposedPosition.topDelta >= mapBounds["top"] && proposedPosition.topDelta <= mapBounds["bottom"];
        if (!withinMap) continue;
        bool notProjectedBlockedSpace = !(new List<char> {mapLegend["wall"], mapLegend["gate"]}.Contains(mapRows[proposedPosition.topDelta][proposedPosition.leftDelta]));
        if (!notProjectedBlockedSpace) continue;
        
    bool badGuyCollision = false;

    // record player's current cursor position before bad guys move
    (int leftDelta, int topDelta) currentPlayerPosition = (Console.CursorLeft, Console.CursorTop);

    foreach(Character BadGuy in BadGuys) {
            while(true) {
                int topDeltaOffset = 1 * (BadGuy.Up ? -1 : 1);
                if(BadGuy.CheckMove(
                    (BadGuy.Position.leftDelta,BadGuy.Position.topDelta + topDeltaOffset), 
                    mapRows, 
                    mapLegend, 
                    mapBounds)
                ){
                    BadGuy.Move((BadGuy.Position.leftDelta, BadGuy.Position.topDelta + topDeltaOffset));
                    break;
                } 
                
                BadGuy.Up = !BadGuy.Up;
            }
        }

        Console.SetCursorPosition(proposedPosition.leftDelta, proposedPosition.topDelta);

        foreach(Character BadGuy in BadGuys) {
            if (BadGuy.Position == proposedPosition) badGuyCollision = true;
            if (BadGuy.Position == currentPlayerPosition) badGuyCollision = true;
        }
        
        if (badGuyCollision) {
            Console.Clear();
            Console.WriteLine("You lost");
            break;
        }
        
        bool playerWon = mapRows[proposedPosition.topDelta][proposedPosition.leftDelta] == mapLegend["win"];
        if (playerWon){
            Console.Clear();
            Console.WriteLine("YOU WON!");
            break;
        }

    } while(true);
}

Main();

// (Collect Coins) You will notice on the map there are 10 '^' symbols. These are coins. Each time you move over the top of one of these, the 'coin' should disappear and your score will go up by 100 points. You should display your score somewhere on the game.
// (Open the gate) You will notice you cannot get to the treasure in the middle. When you have collected all 10 coins (or 1000 points whichever you want to monitor) you will make an opening appear in the center square by 'opening' the door (erasing the characters), thus allowing a path to the treasure in the middle. Just for fun we'll say '$' are gems worth 200 points each.
// (Detect the win) Break out of the loop, clear the screen, and print a congratulatory message if the current cell = "#" Be sure to tell the gamer their score and their time in the maze. (Make sure the program works and commit the changes to your repo.)

// (Enforce walls) Update your TryMove code to additionally enforce that no move is taken if it would land the player on a '*' cell. (Make sure the program works and commit the changes to your repo.)

// (optional) (More Features) (5 bonus points each)
// Add something else interesting to the game.

public class Character {
    public (int leftDelta, int topDelta) Position { get; set; }
    public char Symbol { get; }
    public bool Up { get; set; }
    public Character((int leftDelta, int topDelta) position, char symbol) 
    {
        Position = position;
        Symbol = symbol;
        Up = true;
    }

    public bool CheckMove((int leftDelta, int topDelta) newPosition, string[] mapRows, Dictionary<string, char> mapLegend, Dictionary<string, int> mapBounds){
        bool withinMap = newPosition.leftDelta > mapBounds["left"] && newPosition.leftDelta <= mapBounds["right"] && newPosition.topDelta >= mapBounds["top"] && newPosition.topDelta <= mapBounds["bottom"];
        if (!withinMap) return false;
        bool notProjectedBlockedSpace = !(new List<char> {mapLegend["wall"], mapLegend["gate"]}.Contains(mapRows[newPosition.topDelta][newPosition.leftDelta]));
        if (!notProjectedBlockedSpace) return false;
        return true;
    }

    public void Move((int leftDelta, int topDelta) newPosition){
        Console.SetCursorPosition(Position.leftDelta, Position.topDelta);
        if (Symbol == '%') {
            Console.Write(' ');
        }
        Position = newPosition;
        Console.SetCursorPosition(newPosition.leftDelta, newPosition.topDelta);
        if (Symbol == '%') {
            Console.Write('%');
        }
    }
}