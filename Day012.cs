namespace AdventOfCode2023
{
    internal class Day012
    {
        private const bool Memoize = true;

        public static (string result1, string result2) Run(string input)
        {
            return (Run(input, 1), Run(input, 5));
        }

        // not the best solution... finding good short circuits and a good way to get cache hits was the hard part
        public static string Run(string input, int unfoldCount)
        {
            string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var count = 0L;
            foreach (string line in lines)
            {
                var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var part0 = parts[0];
                var part1 = parts[1];
                for (var i = 0; i < unfoldCount - 1; i++)
                {
                    part0 = part0 + "?" + parts[0];
                    part1 = part1 + "," + parts[1];
                }

                var springs = part0;

                var sizes = Array.ConvertAll(part1.Split(','), int.Parse);
                
                // clear our cache between lines
                memo = new Dictionary<string, long>();

                var permutations = Process(springs, sizes);
                count += permutations;
            }

            return count.ToString();
        }

        private static Dictionary<string, long> memo = new Dictionary<string, long>();

        private static long Process(string springs, int[] sizes)
        {
            var sizeKey = string.Join(",", sizes);
            var key = springs + sizeKey;
            if (Memoize && memo.TryGetValue(key, out var process))
            {
                return process;
            }

            var springCount = springs.Count(c => c == '#');
            var wildCount = springs.Count(c => c == '?');

            if (springCount > sizes.Sum() || springCount + wildCount < sizes.Sum())
            {
                if (Memoize)
                    memo[key] = 0;
                return 0;
            }

            if (sizes.Length == 0)
            {
                if (Memoize)
                    memo[key] = springCount == 0 ? 1 : 0;
                return springCount == 0 ? 1 : 0;
            }

            var chunks = springs.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (chunks.Count(c => c.IndexOf('?') < 0) > sizes.Length)
            {
                memo[key] = 0;
                return 0;
            }

            // check that our completed groups thus far are valid
            for (var i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].IndexOf('?') >= 0)
                    break;

                if (i >= sizes.Length)
                {
                    if (Memoize)
                        memo[key] = 0;
                    return 0;
                }

                // this check is key to not getting stuck on particular problems
                if (chunks[i].Length != sizes[i])
                {
                    if (Memoize)
                        memo[key] = 0;
                    return 0;
                }
            }

            var nextUnknown = springs.IndexOf('?');

            // ending condition
            if (chunks.Length == sizes.Length && nextUnknown < 0)
            {
                bool match = true;
                for (int i = 0; i < chunks.Length; i++)
                {
                    if (chunks[i].Length != sizes[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    if (Memoize)
                        memo[key] = 1;
                    return 1;
                }

                if (Memoize)
                    memo[key] = 0;
                return 0;
            }

            if (chunks.Length != sizes.Length && nextUnknown < 0)
            {
                if (Memoize)
                    memo[key] = 0;
                return 0;
            }


            var perm1 = springs.ToCharArray();
            perm1[nextUnknown] = '.';
            
            var perm2 = springs.ToCharArray();
            perm2[nextUnknown] = '#';
            var result = ProcessRemainingGroups(new string(perm1), sizes) + ProcessRemainingGroups(new string(perm2), sizes);

            if (Memoize)
                memo[key] = result;
            return result;
        }

        // this was needed to have any cache/memoization hits
        private static long ProcessRemainingGroups(string springs, int[] sizes)
        {
            var chunks = springs.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            for (var i = 0; i < chunks.Length && i < sizes.Length; i++)
            {
                if (!string.IsNullOrEmpty(chunks[i].Trim('#')) || chunks[i].Length != sizes[i])
                    break;
                count++;
            }

            springs = string.Join('.', chunks.Skip(count));
            sizes = sizes.Skip(count).ToArray();


            return Process(springs, sizes);
        }
    }
}
