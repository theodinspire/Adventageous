using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\11a");
	File.OpenRead(@"Data\Actual\11");
	// File.OpenRead(@"Data\Example\10b");

var day = new Day11(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");