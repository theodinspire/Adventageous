﻿using Adventageous.Days;

// Example
var exampleFile =
	File.OpenRead(Path.Combine("Data", "Example", "23a"));

var example = new Day23(exampleFile);
exampleFile.Dispose();

Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First()}");
Console.WriteLine($"2nd: {example.Second()}");


// Actual
var actualFile =
	File.OpenRead(Path.Combine("Data", "Actual", "23"));
var actual = new Day23(actualFile);
actualFile.Dispose();

Console.WriteLine();
Console.WriteLine("Actual:");
Console.WriteLine($"1st: {actual.First()}");
Console.WriteLine($"2nd: {actual.Second()}");