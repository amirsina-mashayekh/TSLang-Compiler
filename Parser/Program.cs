using Parser;

string path = args.Length == 0 ? Console.ReadLine()! : args[0];

TSLangParser parser = new(new StreamReader(path), Console.Error);

parser.Parse();

if (!parser.HasError)
{
    Console.WriteLine("Parsing finished successfully.");
}