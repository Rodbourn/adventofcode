using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
    internal class Day006
    {
        public static string Run(string input)
        {
            var numbers = Regex.Matches(input, "\\d+").OfType<Match>().Select(m => long.Parse(m.Value)).ToList();
            var games = new List<(long time, long distance)>();
            for(var i = 0; i < numbers.Count/2; i ++)
                games.Add((numbers[i], numbers[i + numbers.Count / 2]));

            long power = 1;
            foreach(var (T, D) in games)
            {
                double discriminant = Math.Sqrt(T * T - 4 * D);
                var count = (long)Math.Floor((T + discriminant) / 2) - (long)Math.Ceiling((T - discriminant) / 2) + 1;
                power *= count;
            }
            return power.ToString();
        }
    }
}
