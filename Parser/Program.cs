using Parser;

string path = args.Length == 0 ? Console.ReadLine()! : args[0];

TSLangParser parser = new(new StreamReader(path));

parser.Parse();