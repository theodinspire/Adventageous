using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day07
{
	private static Regex ChangeDirectory = new(@"\$ cd (?<directory>\w+)");
	
	private readonly Directory root;

	private readonly Stack<Directory> stack = new();

	private readonly List<Directory> directories = new();

	private Directory current
	{
		get => this.stack.Peek();
		set => this.stack.Push(value);
	}

	public Day07(Stream input)
	{
		this.root = new Directory("/");
		this.current = this.root;
		this.directories.Add(this.root);

		using var reader = new StreamReader(input);

		do
		{
			var line = reader.ReadLine();
			// Console.WriteLine(line);
			
			if (line is null) break;
			
			if (line.Equals("$ cd /"))
				continue;

			if (line.Equals("$ cd .."))
			{
				
				this.stack.Pop();

				if (this.stack.Count == 0)
					this.current = this.root;
				continue;
			}

			if (ChangeDirectory.TryMatch(line, out var match))
			{
				var name = match.Groups["directory"].Value;
				this.current = this.current.GetDirectory(name);
				continue;
			}

			if (Directory.Matcher.TryMatch(line, out match))
			{
				var directory = new Directory(match);
				this.current.Add(directory);
				this.directories.Add(directory);
				continue;
			}

			if (File.Matcher.TryMatch(line, out match))
			{
				var file = new File(match);
				this.current.Add(file);
				continue;
			}
		}
		while (!reader.EndOfStream);
	}

	public int First()
	{
		/*Console.WriteLine("Directory Sizes:");
		foreach (var directory in directories)
		{
			Console.WriteLine("{0,11} {1,16:N0}", directory.Name, directory.Size);
		}*/
		
		return this.directories.Select(x => x.Size)
			.Where(x => x <= 100000)
			.Aggregate((a, b) => a + b);
	}

	public int Second()
	{
		const int total = 70000000;
		const int needed = 30000000;

		var available = total - this.root.Size;
		var toFree = needed - available;

		return this.directories
			.Select(x => x.Size)
			.Where(x => x >= toFree)
			.Order()
			.FirstOrDefault();
	}

	private interface INode
	{
		string Name { get; }
		
		int Size { get; }
	}

	private class Directory : INode
	{
		public static Regex Matcher = new(@"dir (?<name>\w+)");
		
		private IEnumerable<INode> Children => this.Directories.Values.Cast<INode>().Union(this.files);

		public readonly Dictionary<string, Directory> Directories = new();
		private readonly List<File> files = new ();

		public Directory(Match match)
		{
			this.Name = match.Groups["name"].Value;
		}

		public Directory(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public int Size => this.Children.Select(x => x.Size).Aggregate(0, (a, b) => a + b);

		public void Add(INode child)
		{
			switch (child)
			{
				case Directory directory:
					this.Directories.Add(directory.Name, directory);
					break;
				case File file:
					this.files.Add(file);
					break;
				default:
					throw new InvalidOperationException($"Cannot consume child of type {child.GetType().Name}");
			}
		}

		public Directory GetDirectory(string name)
		{
			return this.Directories[name];
		}
	}

	private class File : INode
	{
		public static Regex Matcher = new(@"(?<size>\d+) (?<name>\w+(?:\.\w+)?)");

		public File(Match match)
		{
			this.Name = match.Groups["name"].Value;

			if (int.TryParse(match.Groups["size"].Value, out var size))
				this.Size = size;
		}
		
		public File(string name, int size)
		{
			this.Name = name;
			this.Size = size;
		}
		
		public string Name { get; }

		public int Size { get; }
	}
}