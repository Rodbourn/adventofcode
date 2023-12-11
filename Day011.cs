namespace AdventOfCode2023
{
    using Coordinate = (long x, long y);
    internal class Day011
    {
        public static (string part1, string part2) Run(string input)
        {
            return (GetExpandedDistances(input, 2), GetExpandedDistances(input, 1000000));
        }
        public static string GetExpandedDistances(string input, long expansionFactor)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.TrimEntries);
            var galaxies = new List<Coordinate>();
            var galaxiesXIndex = new Dictionary<long, List<int>>();
            var galaxiesYIndex = new Dictionary<long, List<int>>();
            for (var j = 0; j < lines.Length; j++)
            {
                galaxiesYIndex[j] = [];
                for (var i = 0; i < lines[j].Length; i++)
                {
                    if (j == 0)
                        galaxiesXIndex[i] = [];
                    
                    switch (lines[j][i])
                    {
                        case '#':
                            galaxies.Add((i, j));
                            galaxiesXIndex[i].Add(galaxies.Count - 1);
                            galaxiesYIndex[j].Add(galaxies.Count - 1);
                            break;
                    }
                }
            }

            ExpandGalaxy(galaxiesXIndex, expansionFactor - 1, true);
            ExpandGalaxy(galaxiesYIndex, expansionFactor - 1, false);

            var totalDistance = 0L;
            var galaxyArray = galaxies.ToArray();
            for (int i = 0; i < galaxyArray.Length; i++)
            {
                var (x1, y1) = galaxyArray[i];
                for (int j = i + 1; j < galaxyArray.Length; j++)
                {
                    var (x2, y2) = galaxyArray[j];
                    var dx = x1 - x2;
                    var dy = y1 - y2;
                    totalDistance += (dx >= 0 ? dx : -dx) + (dy >= 0 ? dy : -dy);
                }
            }
            
            return totalDistance.ToString();

            void ExpandGalaxy(Dictionary<long, List<int>> galaxyDimensionIndexes, long expansionScale, bool inX)
            {
                var expansionAmount = 0L;
                foreach (var (_, galaxyIndexes) in galaxyDimensionIndexes)
                {
                    if (galaxyIndexes.Count == 0)
                    {
                        expansionAmount += expansionScale;
                        continue;
                    }

                    foreach (var galaxyIndex in galaxyIndexes)
                    {
                        var c = (galaxies[galaxyIndex].x, galaxies[galaxyIndex].y);
                        if (inX)
                            c.x += expansionAmount;
                        else
                            c.y += expansionAmount;
                        galaxies[galaxyIndex] = c;
                    }
                }
            }
        }
    }
}
