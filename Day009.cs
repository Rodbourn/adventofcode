namespace AdventOfCode2023
{
    internal class Day009
    {
        public static (string part1, string part2) Run(string input)
        {
            // Finite difference weights for lengths 6, and 21, extrapolated to the next interval
            // where it's noted that to extrapolate left, you just reverse the weights due to symmetry
            // To calculate them, see:
            // % Calculation of Weights in Finite Difference Formulas
            // %   Bengt Fornberg
            // %   SIAM Review
            // %   Vol. 40, No. 3 (Sep., 1998), pp. 685-691
            // %   Published by: Society for Industrial and Applied Mathematics
            // %   Stable URL: http://www.jstor.org/stable/2653239
            var weights = new Dictionary<int, int[]>();
            weights.Add(6, [-1, 6, -15, 20, -15, 6]);
            weights.Add(21, [1, -21, 210, -1330, 5985, -20349, 54264, -116280, 203490, -293930, 352716, -352716, 293930, -203490, 116280, -54264, 20349, -5985, 1330, -210, 21]);
            string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            long sumRight = 0;
            long sumLeft = 0;
            foreach (string line in lines)
            {
                long extrapolatedValueRight = 0;
                long extrapolatedValueLeft = 0;
                string[] numbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                long[] nums = Array.ConvertAll(numbers, long.Parse);
                for (var i = 0; i < nums.Length; i++)
                {
                    extrapolatedValueRight += nums[i] * weights[nums.Length][i];
                    extrapolatedValueLeft += nums[i] * weights[nums.Length][nums.Length -1 - i];
                }

                sumRight += extrapolatedValueRight;
                sumLeft += extrapolatedValueLeft;
            }

            return (part1: sumRight.ToString(), part2: sumLeft.ToString());
        }
    }
}
