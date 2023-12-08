using System.Text;

namespace AdventOfCode2023
{
    internal class Day007
    {
        public static Func<Rank, int> RankEvaluatorPartA = rank => (int)rank;
        // Jacks are the weakest, followed by Two, Three, ..., Ace
        public static Func<Rank, int> RankEvaluatorPartB = rank => rank == Rank.Jack ? 0 : (int)rank + 1;

        public static string Run(string input, Func<Rank, int> rankingToUse, HashSet<Rank> wildCards)
        {
            // could just sort on the packed integer due to bit packing
            var hands = new SortedDictionary<Hand, int>(new HandComparer(rankingToUse));
            foreach (var line in input.Split(Environment.NewLine))
            {
                var cards = line.Trim().Split(" ");
                var hand = Hand.Parse(cards[0], wildCards);
                var value = int.Parse(cards[1]);
                hands[hand] = value;
            }

            var winnings = 0;
            var rank = 0;
            
            foreach (var (hand, bid) in hands)
            {
                rank++;
                winnings += bid * rank;
            }
            
            return winnings.ToString();
        }

        public class HandComparer : IComparer<Hand>
        {
            protected Func<Rank, int> rankFunc { get; set; }

            public HandComparer(Func<Rank, int> rankFunc)
            {
                this.rankFunc = rankFunc;
            }

            public int Compare(Hand x, Hand y)
            {
                var result = x.HandType.CompareTo(y.HandType);
                if (result != 0)
                    return result;
                
                for (var i = 0; i < 5; i++)
                {
                    result = rankFunc(x.GetCard(i).Rank).CompareTo(rankFunc(y.GetCard(i).Rank));
                    if (result != 0)
                        return result;
                }
                return 0;
            }
        }


        [Flags]
        public enum Rank { Two = 0,  Three = 1,  Four = 2,  Five = 3,  Six = 4,  Seven = 5,  Eight = 6,  Nine = 7,  Ten = 8,  Jack = 9,  Queen = 10,  King = 11,  Ace = 12 }


        public struct Card
        {
            public Rank Rank { get; }

            public Card(Rank rank)
            {
                Rank = rank;
            }
            public int ToInt()
            {
                return (int)Rank;
            }

            public static Card FromInt(int value)
            {
                return new Card((Rank)value);
            }
            
            public Card(char c)
            {
                Rank = c switch
                {
                    'T' => Rank.Ten,
                    'J' => Rank.Jack,
                    'Q' => Rank.Queen,
                    'K' => Rank.King,
                    'A' => Rank.Ace,
                    '2' => Rank.Two,
                    '3' => Rank.Three,
                    '4' => Rank.Four,
                    '5' => Rank.Five,
                    '6' => Rank.Six,
                    '7' => Rank.Seven,
                    '8' => Rank.Eight,
                    '9' => Rank.Nine,
                    _ => throw new ArgumentException("Invalid card character", nameof(c)),
                };
            }
        }

        public enum HandType
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind
        }

        public readonly struct Hand
        {
            public int PackedValue { get; }

            public HandType HandType { get; }

            public Hand(Card card1, Card card2, Card card3, Card card4, Card card5, IReadOnlySet<Rank> wildCards)
            {
                // Determine hand type
                int maxRank = 0;
                int pairs = 0;

                // pack the cards into _value
                PackedValue = card1.ToInt() | (card2.ToInt() << 4) | (card3.ToInt() << 8) | (card4.ToInt() << 12) | (card5.ToInt() << 16);
                
                // Determine hand type
                var ranks = new int[13];
                var wildCount = 0;
                var cards = new[] { card1, card2, card3, card4, card5 };
                foreach (var card in cards)
                {
                    if (wildCards.Contains(card.Rank))
                    {
                        wildCount++;
                        continue;
                    }
                    var rank = (int)card.Rank;
                    var count = ++ranks[rank];
                    maxRank = Math.Max(count, maxRank);
                    if (count == 2) pairs++;
                    else if (count == 3) { pairs--;  }
                }
                HandType = maxRank switch
                {
                    5 => HandType.FiveOfAKind,
                    4 => wildCount > 0 ? HandType.FiveOfAKind : HandType.FourOfAKind,
                    3 => wildCount switch
                    {
                        2 => HandType.FiveOfAKind,
                        1 => HandType.FourOfAKind,
                        _ => pairs > 0 ? HandType.FullHouse : HandType.ThreeOfAKind
                    },
                    2 => pairs switch
                    {
                        // If there are two pairs and at least one wild, it's a Full House
                        > 1 => wildCount > 0 ? HandType.FullHouse : HandType.TwoPair,
                        // If there's only one pair
                        1 => wildCount switch
                        {
                            1 => HandType.ThreeOfAKind, // One wild card makes it Three of a Kind
                            2 => HandType.FourOfAKind,  // Two wild cards make it Four of a Kind
                            _ => wildCount > 2 ? HandType.FiveOfAKind : HandType.OnePair // Three wild cards make it Five of a Kind, otherwise it's One Pair
                        },
                        // No pairs
                        _ => HandType.HighCard
                    },
                    _ => wildCount switch
                    {
                        1 => HandType.OnePair,
                        2 => HandType.ThreeOfAKind,
                        3 => HandType.FourOfAKind,
                        4 => HandType.FiveOfAKind,
                        5 => HandType.FiveOfAKind,
                        _ => HandType.HighCard
                    }
                };
            }

            public Card GetCard(int index)
            {
                return Card.FromInt((PackedValue >> (index * 4)) & 0xF);
            }

            public static Hand Parse(string hand, HashSet<Rank> wildCards)
            {
                if (hand.Length != 5)
                    throw new ArgumentException("Invalid hand string.", nameof(hand));
                var cards = new Card[5];
                for (int i = 0; i < 5; i++)
                {
                    cards[i] = new Card(hand[i]);
                }

                return new Hand(cards[0], cards[1], cards[2], cards[3], cards[4], wildCards);
            }
        }
    }
}
