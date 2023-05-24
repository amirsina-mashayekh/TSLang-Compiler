using Tokenizer;

string path = args.Length == 0 ? Console.ReadLine()! : args[0];

TSLangTokenizer tokenizer = new(new StreamReader(path));

while (!tokenizer.EndOfStream)
{
    Console.WriteLine(tokenizer.NextToken());
}