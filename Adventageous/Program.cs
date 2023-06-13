using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\04a");
	File.OpenRead(@"Data\Actual\04");

var day = new Day04(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");