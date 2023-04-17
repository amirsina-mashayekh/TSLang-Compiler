using Tokenizer;

string path = args.Length == 0 ? Console.ReadLine()! : args[0];

TesLangTokenizer tokenizer = new(new StreamReader(path));

List<Token> tokens = new();
while (!tokenizer.EndOfStream)
{
    tokens.Add(tokenizer.NextToken());
}

foreach (var token in tokens)
{
    Console.WriteLine(token);
}