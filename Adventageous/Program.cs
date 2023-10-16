using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\12a");
	File.OpenRead(@"Data\Actual\12");
	// File.OpenRead(@"Data\Example\10b");

var day = new Day12(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");