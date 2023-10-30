using Adventageous.Days;
var exampleFile =
	File.OpenRead(@"Data\Example\15a");

var actualFile =
	File.OpenRead(@"Data\Actual\15");

var example = new Day15(exampleFile);
exampleFile.Dispose();
var actual = new Day15(actualFile);
actualFile.Dispose();


Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First(10)}");
Console.WriteLine($"2nd: {example.Second(20)}");

Console.WriteLine();
Console.WriteLine($"1st: {actual.First(2_000_000)}");
Console.WriteLine($"2nd: {actual.Second(4_000_000)}");