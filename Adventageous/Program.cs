using Adventageous.Days;
var exampleFile =
	File.OpenRead(Path.Combine("Data", "Example", "20a"));

var actualFile =
	File.OpenRead(Path.Combine("Data", "Actual", "20"));

var example = new Day20(exampleFile);
exampleFile.Dispose();
var actual = new Day20(actualFile);
actualFile.Dispose();


Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First()}");
Console.WriteLine($"2nd: {example.Second()}");

Console.WriteLine();
Console.WriteLine("Actual:");
Console.WriteLine($"1st: {actual.First()}");
Console.WriteLine($"2nd: {actual.Second()}");