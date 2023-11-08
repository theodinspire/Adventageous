﻿using Adventageous.Days;

// Example
var exampleFile =
	File.OpenRead(Path.Combine("Data", "Example", "22a"));

var example = new Day22(exampleFile);
exampleFile.Dispose();

Console.WriteLine("Example:");
Console.WriteLine($"1st: {example.First()}");
Console.WriteLine($"2nd: {example.Second(4)}");


// Actual
var actualFile =
	File.OpenRead(Path.Combine("Data", "Actual", "22"));
var actual = new Day22(actualFile);
actualFile.Dispose();

Console.WriteLine();
Console.WriteLine("Actual:");
Console.WriteLine($"1st: {actual.First()}");
Console.WriteLine($"2nd: {actual.Second(50)}");