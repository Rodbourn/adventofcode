using System.Text;

namespace AdventOfCode2023
{
    internal class Day003
    {
        public static string RunPart2(string input)
        {
            var lines = input.Split("\r\n");
            int width = lines[0].Length;
            int height = lines.Length;

            var mask = new bool[width, height];
            var maskGears = new Dictionary<(int, int), HashSet<(int, int)>>();
            var gears = new Dictionary<(int, int), List<int>>();

            var cleanLines = new List<string>();

            for (int j = 0; j < height; j++)
            {
                var line = lines[j];
                var cleanLine = new StringBuilder();
                for (int i = 0; i < width; i++)
                {
                    var c = line[i];
                    // use this for part 1
                    //if (!char.IsDigit(c) && c != '.')
                    if (c == '*')
                    {
                        gears.Add((i, j), new List<int>());
                        punch(i, j);
                    }

                    if (char.IsDigit(c))
                    {
                        cleanLine.Append(c);
                    }
                    else
                    {
                        cleanLine.Append(".");
                    }
                }
                cleanLines.Add(cleanLine.ToString());
            }


            var numbers = new List<int>();

            for (int j = 0; j < height; j++)
            {
                var line = cleanLines[j];

                var start = 0;
                foreach (var chunk in line.Split("."))
                {
                    if (chunk == "")
                    {
                        start += 1;
                        continue;
                    }

                    bool hit = false;
                    var touchedGears = new HashSet<(int, int)>();
                    for (var index = start; index < start + chunk.Length; index++)
                    {
                        if (mask[index, j])
                        {
                            hit = true;
                            foreach (var gear in maskGears[(index, j)])
                            {
                                touchedGears.Add(gear);
                            }
                        }
                    }
                    foreach (var gear in touchedGears)
                    {
                        gears[gear].Add(int.Parse(chunk));
                    }
                    if (hit)
                    {
                        numbers.Add(int.Parse(chunk));
                    }

                    start += chunk.Length + 1;
                }
            }


            var gearPower = 0;
            foreach (var (coordinate, values) in gears)
            {
                if (values.Count == 2)
                {
                    gearPower += (values[0] * values[1]);
                }
            }

            return gearPower.ToString();

            void punch(int x, int y)
            {
                for (int i = Math.Max(0, x - 1); i <= Math.Min(x + 1, width - 1); i++)
                {
                    for (int j = Math.Max(0, y - 1); j <= Math.Min(y + 1, height - 1); j++)
                    {
                        mask[i, j] = true;
                        if (!maskGears.ContainsKey((i, j)))
                        {
                            maskGears.Add((i, j), new HashSet<(int, int)>());
                        }
                        maskGears[(i, j)].Add((x, y));
                    }
                }
            }
        }
    }
}