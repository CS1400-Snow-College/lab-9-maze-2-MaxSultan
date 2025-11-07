// Max Sultan, Nov 6 2025, Lab 9: Maze 2

// (Load from file) Within your Main method, create a string array variable called mapRows and use File.ReadAllLines to load the contents of map.txt into your variable. Clear the screen, and then write a loop to print out the rows of the map to the screen. (Make sure the program works and commit the changes to your repo.)

// IF YOUR TERMINAL ISN'T TALL ENOUGH TO PRINT THE MAP YOUR PROGRAM WILL CRASH

// (Basic User controls)

// Move the cursor position to the top left corner of the screen (this would be a great time to read chapter 4)
// Create a "do-while" loop that accepts user input (one key at a time without echoing --- hint: Console.ReadKey(true).Key will evaluate to a ConsoleKey value.
// On each iteration through the loop, process a pressed key as follows:
// Key	Action
// ConsoleKey.Escape	return from Main
// ConsoleKey.UpArrow	Console.CursorTop--;
// ConsoleKey.DownArrow	Console.CursorTop++;
// ConsoleKey.LeftArrow	Console.CursorLeft--;
// ConsoleKey.RightArrow	Console.CursorLeft++;
// Notice what happens when you move the cursor off of the page (that is fine for now). (Make sure the program works and commit the changes to your repo.)

// (Stay on the map) Update your handling of user input to ensure the following:
// Never allow Console.CursorTop to be less than 0 or greater than Console.BufferHeight or mazeRows.Length
// Never allow Console.CursorLeft to be less than 0 or greater than Console.BufferWidth or the length of the mazeRow.
// hint: consider writing a method static void TryMove(int proposedTop, int proposedLeft, string[] mazeRows) that sets CursorTop=proposedTop and CursorLeft=proposedTop only if the proposed location is valid.
// (Move the Bad Guys) You will notice on the map there are two '%' symbols. These are bad guys. Each time you move, they move. How they move is entirely up to you. You can make it random, you can make it up and down like you move, you can make it diagonals. They can mirror each other or both take their own path. Have some fun with this. . If the bad guy collides with the good guy, break out of the loop, clear the screen and print a 'You Lose' message.
// (Collect Coins) You will notice on the map there are 10 '^' symbols. These are coins. Each time you move over the top of one of these, the 'coin' should disappear and your score will go up by 100 points. You should display your score somewhere on the game.
// (Open the gate) You will notice you cannot get to the treasure in the middle. When you have collected all 10 coins (or 1000 points whichever you want to monitor) you will make an opening appear in the center square by 'opening' the door (erasing the characters), thus allowing a path to the treasure in the middle. Just for fun we'll say '$' are gems worth 200 points each.
// (Detect the win) Break out of the loop, clear the screen, and print a congratulatory message if the current cell = "#" Be sure to tell the gamer their score and their time in the maze. (Make sure the program works and commit the changes to your repo.)

// (Enforce walls) Update your TryMove code to additionally enforce that no move is taken if it would land the player on a '*' cell. (Make sure the program works and commit the changes to your repo.)

// (optional) (More Features) (5 bonus points each)
// Add something else interesting to the game.
