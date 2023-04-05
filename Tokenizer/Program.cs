string path = Console.ReadLine()!;
StreamReader codeFile = new(path);

var tokens = Tokenizer.Tokenizer.Tokenize(codeFile);

foreach (var token in tokens)
{
    Console.WriteLine(token);
}