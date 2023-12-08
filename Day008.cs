
namespace AdventOfCode2023
{
    internal class Day008
    {
        public static (string part1, string part2) Run(string input)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var directions = lines[0].Trim().Select(i => i == 'R'? 1: 0).ToArray();
            var dictionary = new Dictionary<string, (string left, string right)>();
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split('=');
                var key = parts[0].Trim();
                var value = parts[1].Trim().TrimStart('(').TrimEnd(')').Split(',').Select(i => i.Trim()).ToArray();
                dictionary[key] = (value[0], value[1]);
            }

            // part1
            var part1Answer = (GetPathToEnd(dictionary, directions, "AAA", "ZZZ").Count-1).ToString();

            // part 2
            var part2Starts = dictionary.Keys.Where(k => k.EndsWith("A")).ToList();
            var paths = new List<List<string>>();
            foreach (var start in part2Starts)
                paths.Add(GetPathToEnd(dictionary, directions, start, "Z"));
            // the  least common multiple will be when the various paths align
            var part2Answer = FindLcm(paths.Select(p => p.Count -1).ToArray()).ToString();
            return (part1Answer, part2Answer);
        }

        private static List<string> GetPathToEnd(Dictionary<string, (string left, string right)> map, int[] directions, string start, string end)
        {
            var current = start;
            var path = new List<string>() { start };
            var index = 0;
            do
            {
                var localIndex = index % directions.Length;
                if (directions[localIndex] == 0)
                {
                    current = map[current].left;
                }
                else
                    current = map[current].right;
                path.Add(current);

                index++;

                if (current.EndsWith(end))
                    break;
            } while (true);

            return path;
        }

        static long Gcd(long a, long b)
        {
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        static long Lcm(long a, long b)
        {
            return (a / Gcd(a, b)) * b;
        }

        static long FindLcm(int[] numbers)
        {
            long result = numbers[0];
            for (long i = 1; i < numbers.Length; i++)
            {
                result = Lcm(result, numbers[i]);
            }
            return result;
        }
    }
}
