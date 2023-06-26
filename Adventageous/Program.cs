using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\09a");
	File.OpenRead(@"Data\Actual\09");
	// File.OpenRead(@"Data\Example\09b");


var day = new Day09(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");