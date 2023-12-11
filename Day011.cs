namespace AdventOfCode2023
{
    using Coordinate = (long x, long y);
    using Pair = ((long x, long y) c1, (long x, long y) c2);
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
            var pairDistances = new HashSet<Pair>();
            foreach (var galaxy in galaxies)
            {
                foreach (var otherGalaxy in galaxies)
                {
                    if (otherGalaxy == galaxy) continue;
                    var pair = GetNormalizedPair(galaxy, otherGalaxy);
                    if (pairDistances.Contains(pair)) continue;
                    var distance = Math.Abs(pair.c1.x - pair.c2.x) + Math.Abs(pair.c1.y - pair.c2.y);
                    totalDistance += distance;
                    pairDistances.Add(pair);
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
            static Pair GetNormalizedPair(Coordinate point1, Coordinate point2)
            {
                return point1.CompareTo(point2) < 0 ? (point1, point2) : (point2, point1);
            }
        }
    }
}
