using Adventageous.Days;
var exampleFile =
	File.OpenRead(@"Data\Example\17a");

var actualFile =
	File.OpenRead(@"Data\Actual\17");

var example = new Day17(exampleFile);
exampleFile.Dispose();
var actual = new Day17(actualFile);
actualFile.Dispose();


Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First()}");
Console.WriteLine($"2nd: {example.Second()}");

Console.WriteLine();
Console.WriteLine($"1st: {actual.First()}");
Console.WriteLine($"2nd: {actual.Second()}");