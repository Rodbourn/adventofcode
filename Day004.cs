namespace AdventOfCode2023
{
    internal class Day004
    {
        public static (string part1, string part2) Run(string input)
        {
            var lines = input.Split("\r\n");
            var cardCounts = new Dictionary<int, int>();
            var totalPoints = 0;
            var cardNumber = 0;
            foreach (var line in lines)
            {
                cardNumber++;
                if (cardCounts.ContainsKey(cardNumber))
                    cardCounts[cardNumber]++;
                else
                    cardCounts.Add(cardNumber, 1);

                var cleanLine = line.Replace("  ", " ");
                var winningNumbers = cleanLine.Split(" | ")[0].Split(":")[1].Trim().Split(" ").Select(i => Int32.Parse(i)).ToHashSet();
                var myNumbers = cleanLine.Split(" | ")[1].Trim().Split(" ").Select(i => Int32.Parse(i)).ToHashSet();
                var matches = winningNumbers.Intersect(myNumbers);
                if (matches.Any())
                {
                    for (var i = 0; i < matches.Count(); i++)
                    {
                        if (cardCounts.ContainsKey(cardNumber + i + 1))
                            cardCounts[cardNumber + i + 1] += cardCounts[cardNumber];
                        else
                            cardCounts.Add(cardNumber + i + 1, cardCounts[cardNumber]);
                    }
                    totalPoints += Convert.ToInt32(Math.Pow(2, matches.Count() - 1));
                }
            }
            return (totalPoints.ToString(), cardCounts.Values.Sum().ToString());
        }
    }
}
