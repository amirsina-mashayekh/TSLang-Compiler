string path = args.Length == 0 ? Console.ReadLine()! : args[0];
StreamReader codeFile = new(path);

var tokens = Tokenizer.TesLangTokenizer.Tokenize(codeFile);

foreach (var token in tokens)
{
    Console.WriteLine(token);
}