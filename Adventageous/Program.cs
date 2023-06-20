using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\06a");
	File.OpenRead(@"Data\Actual\06");

var day = new Day06(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");