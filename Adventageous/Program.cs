using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\14a");
	File.OpenRead(@"Data\Actual\14");
	// File.OpenRead(@"Data\Example\10b");

var day = new Day14(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");