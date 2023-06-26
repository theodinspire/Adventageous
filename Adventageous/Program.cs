using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\07a");
	File.OpenRead(@"Data\Actual\07");

var day = new Day07(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");