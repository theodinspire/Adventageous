using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\03a");
	File.OpenRead(@"Data\Actual\03");

var day = new Day03(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");