using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\08a");
	File.OpenRead(@"Data\Actual\08");

var day = new Day08(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");