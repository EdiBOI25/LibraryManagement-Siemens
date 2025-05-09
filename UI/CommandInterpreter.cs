namespace UI;

public static class CommandInterpreter
    {
        public static async Task ExecuteAsync(string input, IServiceProvider serviceProvider)
        {
            var tokens = ParseInput(input);

            if (tokens.Count == 0)
            {
                Console.WriteLine("Invalid command.");
                return;
            }

            var mainCommand = tokens[0].ToLower();
            var subCommand = tokens.Count > 1 ? tokens[1].ToLower() : "";

            var arguments = tokens.Skip(2).ToList();

            switch (mainCommand)
            {
                case "book":
                    await BookCommandHandler.HandleAsync(subCommand, arguments, serviceProvider);
                    break;
                
                case "categories":
                    await CategoryCommandHandler.HandleAsync(subCommand, arguments, serviceProvider);
                    break;
                
                case "lending":
                    await LendingCommandHandler.HandleAsync(subCommand, arguments, serviceProvider);
                    break;

                case "help":
                    ShowHelp();
                    break;

                default:
                    Console.WriteLine($"Unknown command category: '{mainCommand}'");
                    break;
            }
        }

        private static List<string> ParseInput(string input)
        {
            var args = new List<string>();
            var current = new List<char>();
            bool inQuotes = false;

            foreach (char c in input)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Count > 0)
                    {
                        args.Add(new string(current.ToArray()));
                        current.Clear();
                    }
                }
                else
                {
                    current.Add(c);
                }
            }

            if (current.Count > 0)
                args.Add(new string(current.ToArray()));

            return args;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Available command categories:");
            Console.WriteLine("- book add --title \"...\" --author \"...\" --quantity n [--categories ...,...,]");
            Console.WriteLine("- book list [--title \"...\"] [--author \"...\"] [--categories ...,...,] [--sortby id|title|author|rating]");
            Console.WriteLine("- book delete --id n");
            Console.WriteLine("- categories list");
            Console.WriteLine("- lending borrow --bookid n --name \"...\"");
            Console.WriteLine("- lending return --bookid ... --name ... [--rating ...]");
            Console.WriteLine("- lending list [--bookid n] [--name \"...\"] [--active]");
            Console.WriteLine("- exit / quit");
        }
    }