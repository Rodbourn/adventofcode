
namespace AdventOfCode2023
{
    internal class Day002
    {
        public (string possibleGames, string powers) Run(string input)
        {


            var cubeCounts = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
            var powers = new List<long>();
            var possibleGames = new List<int>();
            input.Split("\n")
                .ToList()
                .ForEach(line =>
                {
                    var parts = line.Trim().Split(":");
                    var name = parts[0];
                    var gameNumber = Int32.Parse(name.Split(' ')[1]);
                    var possible = true;

                    var neededCubes = new Dictionary<string, int> { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
                    foreach (var roll in parts[1].Split(";").Select(i => i.Trim()))
                    {
                        var value = roll.Split(",").Select(i => i.Trim().Split(" ")).ToDictionary(p => p[1], p => Int32.Parse(p[0]));

                        foreach (var (color, count) in value)
                        {
                            neededCubes[color] = Math.Max(count, neededCubes[color]);
                        }
                        possible = possible && value.All(kvp => cubeCounts.ContainsKey(kvp.Key) && cubeCounts[kvp.Key] >= kvp.Value);


                    }
                    if (possible) possibleGames.Add(gameNumber);
                    powers.Add(neededCubes["red"] * neededCubes["green"] * neededCubes["blue"]);
                });

            return (possibleGames.Sum().ToString(), powers.Sum().ToString());
        }
    }
}
