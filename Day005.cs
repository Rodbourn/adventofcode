using System.Text;

namespace AdventOfCode2023
{
    internal class Day005
    {
        public static (string part1, string part2) Run(string input) 
        {
            //
            // parsing...
            //
            var inputQueue = new Queue<string>(input.Split("\r\n"));
            var seeds = inputQueue.Dequeue().Split(" ").SelectWhereOut(i => (long.TryParse(i, out long n), n), n => n).ToList();
            var mappings = new Dictionary<(string, string), RangeMapper>();
            RangeMapper currentMapping = null;
            while (inputQueue.TryDequeue(out string line))
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (char.IsDigit(line[0]))
                {
                    var numbers = line.Split(" ").SelectWhereOut(i => (long.TryParse(i, out long n), n), n => n).ToList();
                    currentMapping.AddMapping(numbers[1], numbers[0], numbers[2]);
                }
                else
                {
                    var mappingParts = line.Split(" ")[0].Split("-").ToList();
                    var key = (mappingParts[0], mappingParts[2]);
                    var inverseKey = (mappingParts[2], mappingParts[0]);
                    mappings.Add(key, new RangeMapper());
                    mappings.Add(inverseKey, new RangeMapper());
                    currentMapping = mappings[key];
                }
            }

            var part1 = seeds.Select(i => (seed: i, x: mappings[("seed", "soil")].MapValue(i)))
                .Select(i => (seed: i.seed, x: mappings[("soil", "fertilizer")].MapValue(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("fertilizer", "water")].MapValue(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("water", "light")].MapValue(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("light", "temperature")].MapValue(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("temperature", "humidity")].MapValue(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("humidity", "location")].MapValue(i.x))).Select(sl => sl.x).Min().ToString();
            

            Console.WriteLine("part 2");
            var seedRanges = new List<Range>();
            for (int seedIndex = 0; seedIndex < seeds.Count; seedIndex += 2)
            {
                seedRanges.Add(new Range(seeds[seedIndex], seeds[seedIndex + 1]));
            }

            var bestLocation = seedRanges.Select(i => (seed: i, x: mappings[("seed", "soil")].ApplyRangeMapping(i)))
                .Select(i => (seed: i.seed, x: mappings[("soil", "fertilizer")].ApplyRangeMapping(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("fertilizer", "water")].ApplyRangeMapping(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("water", "light")].ApplyRangeMapping(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("light", "temperature")].ApplyRangeMapping(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("temperature", "humidity")].ApplyRangeMapping(i.x)))
                .Select(i => (seed: i.seed, x: mappings[("humidity", "location")].ApplyRangeMapping(i.x)))
                .SelectMany(locationRange => locationRange.x.GetStartsWithIndexes().OrderBy(kvp => kvp.Key).Select(kvp => (seed: locationRange.seed.intervals[0].Start + kvp.Value, location: kvp.Key)))
                .OrderBy(i => i.location)
                .First();

            return (part1, bestLocation.location.ToString());
        }

        public class Interval
        {
            public long Start { get; set; }
            public long Length { get; set; }
        }
        public class Range
        {
            public List<Interval> intervals = new List<Interval>();
            public Range(long start, long length)
            {
                intervals.Add(new Interval { Start = start, Length = length });
            }
            public Range()
            {
            }
            public Dictionary<long, long> GetStartsWithIndexes()
            {
                Dictionary<long, long> startIndexes = new Dictionary<long, long>();
                long currentIndex = 0; // This will track the index in the original range

                foreach (var interval in intervals)
                {
                    // The start of each interval maps to the current index
                    startIndexes[interval.Start] = currentIndex;

                    // Update the current index for the next interval
                    currentIndex += interval.Length;
                }

                return startIndexes;
            }
        }
        public class RangeMapping
        {
            public long SourceStart { get; set; }
            public long DestinationStart { get; set; }
            public long Length { get; set; }

            public RangeMapping(long sourceStart, long destinationStart, long length)
            {
                SourceStart = sourceStart;
                DestinationStart = destinationStart;
                Length = length;
            }

            public bool ContainsSource(long sourceValue)
            {
                return sourceValue >= SourceStart && sourceValue < SourceStart + Length;
            }

            public long Map(long sourceValue)
            {
                return DestinationStart + (sourceValue - SourceStart);
            }
            /// <summary>
            /// produces a new range, where this mapping has been applied to input range
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
            public Range CutRange(Range range)
            {
                Range newRange = new Range();

                foreach (var interval in range.intervals)
                {
                    // Check if the interval overlaps with the mapping
                    if (interval.Start + interval.Length <= SourceStart || interval.Start >= SourceStart + Length)
                    {
                        // Interval does not overlap, add it as is
                        newRange.intervals.Add(interval);
                    }
                    else
                    {
                        // Handle the part of the interval before the mapping
                        if (interval.Start < SourceStart)
                        {
                            newRange.intervals.Add(new Interval
                            {
                                Start = interval.Start,
                                Length = SourceStart - interval.Start
                            });
                        }

                        // Handle the overlapping part
                        long overlapStart = Math.Max(interval.Start, SourceStart);
                        long overlapEnd = Math.Min(interval.Start + interval.Length, SourceStart + Length);
                        newRange.intervals.Add(new Interval
                        {
                            Start = overlapStart,
                            Length = overlapEnd - overlapStart
                        });

                        // Handle the part of the interval after the mapping
                        if (interval.Start + interval.Length > SourceStart + Length)
                        {
                            newRange.intervals.Add(new Interval
                            {
                                Start = SourceStart + Length,
                                Length = (interval.Start + interval.Length) - (SourceStart + Length)
                            });
                        }
                    }
                }

                return newRange;
            }
        }
        public class RangeMapper
        {
            public List<RangeMapping> mappings = new List<RangeMapping>();

            public void AddMapping(long sourceStart, long destinationStart, long length)
            {
                mappings.Add(new RangeMapping(sourceStart, destinationStart, length));
            }
            public Range ApplyRangeMapping(Range range)
            {
                Range currentRange = range;

                foreach (var mapping in mappings)
                {
                    currentRange = mapping.CutRange(currentRange);
                }
                foreach(var interval in currentRange.intervals)
                {
                    interval.Start = MapValue(interval.Start);
                }
                return currentRange;
            }


            public long MapValue(long sourceValue)
            {
                foreach (var mapping in mappings)
                {
                    if (mapping.ContainsSource(sourceValue))
                    {
                        return mapping.Map(sourceValue);
                    }
                }
                return sourceValue;
            }
        }

    }
    public static class EnumerableExtensions
    {
        // it bothers me how tryparse has the out parameter, and returns the bool of the attempt, put this together
        // to avoid doing a .where(long.tryparse(, out var _)).select(long.parse()), parsing twice
        public static IEnumerable<TResult> SelectWhereOut<TSource, TIntermediate, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, (bool, TIntermediate)> selectFunc,
            Func<TIntermediate, TResult> whereFunc)
        {
            foreach (var item in source)
            {
                var (isSuccessful, longmediate) = selectFunc(item);
                if (isSuccessful)
                {
                    yield return whereFunc(longmediate);
                }
            }
        }
    }

}
