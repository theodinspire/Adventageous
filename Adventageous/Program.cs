using Adventageous.Days;
var exampleFile =
	File.OpenRead(Path.Combine("Data", "Example", "18a"));

var actualFile =
	File.OpenRead(Path.Combine("Data", "Actual", "18"));

var example = new Day18(exampleFile);
exampleFile.Dispose();
var actual = new Day18(actualFile);
actualFile.Dispose();


Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First()}");
Console.WriteLine($"2nd: {example.Second()}");

Console.WriteLine();
Console.WriteLine($"1st: {actual.First()}");
Console.WriteLine($"2nd: {actual.Second()}");