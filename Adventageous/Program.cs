using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\13a");
	File.OpenRead(@"Data\Actual\13");
	// File.OpenRead(@"Data\Example\10b");

var day = new Day13(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");