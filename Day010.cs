namespace AdventOfCode2023
{
    internal class Day010
    {
        private static ((int dx1, int dy1), (int dx2, int dy2)) GetPipeConnections(char pipe, int x, int y)
        {
            return pipe switch
            {
                '|' => ((x, y - 1), (x, y + 1)),
                '-' => ((x - 1, y), (x + 1, y)),
                'L' => ((x, y - 1), (x + 1, y)),
                'J' => ((x, y - 1), (x - 1, y)),
                '7' => ((x - 1, y), (x, y + 1)),
                'F' => ((x + 1, y), (x, y + 1)),
                _ => throw new Exception($"Bad Pipe: {pipe}"),
            };
        }

        public static (string part1, string part2) Run(string input)
        {
            // get our pipes
            var pipes = new Dictionary<(int x0, int y0), ((int x1, int y1) connection1, (int x2, int y2) connection2)>();
            var grid = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            int height = grid.Length;
            int width = grid[0].Length;
            var start = (x: -1, y: -1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (grid[y][x])
                    {
                        case '.':
                            continue;
                        case 'S':
                            start = (x, y);
                            break;
                        default:
                            pipes.Add((x,y), GetPipeConnections(grid[y][x], x, y));
                            break;
                    }
                }
            }
            
            // build our graph
            var graph = new Dictionary<(int x0, int y0), List<(int x1, int y1)>>();
            foreach (var pipe in pipes)
            {
                var (x, y) = pipe.Key;
                var ((dx1, dy1), (dx2, dy2)) = pipe.Value;
                if (!graph.ContainsKey((x, y)))
                {
                    graph[(x, y)] = new List<(int dx, int dy)>();
                }
                AddConnectionToGraph(pipe.Key, (dx1, dy1));
                AddConnectionToGraph(pipe.Key, (dx2, dy2));
            }
            void AddConnectionToGraph((int x, int y) current, (int dx, int dy) connection)
            {
                if (pipes.TryGetValue(connection, out var connections) &&
                    (connections.connection1 == current || connections.connection2 == current))
                {
                    graph[current].Add(connection);
                }
            }

            // manually add the start, it's the only one with more than two possible connections
            graph[start] = new List<(int x, int y)>();
            foreach (var pipe in pipes)
            {
                if (pipe.Key != start && (pipe.Value.connection1 == start || pipe.Value.connection2 == start))
                {
                    graph[start].Add(pipe.Key);
                }
            }

            var visited = new bool[width, height];
            var maskedLoop = new bool[width, height];
            var enclosedByLoop = new bool[width, height];
            var distances = new int[width, height];
            var maxDistance = -1;
            var queue = new Queue<((int x, int y) p, int n, List<(int x, int y)> path)>();
            queue.Enqueue((start, 0, new List<(int x, int y)>()));
            visited[start.x, start.y] = true;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var hasConnection = false;
                foreach (var connection in graph[current.p])
                {
                    hasConnection |= ProcessPipe(connection, current.n, current.path);
                }
                // end of path
                if (!hasConnection)
                {
                    foreach (var p in current.path)
                    {
                        maskedLoop[p.x, p.y] = true;
                    }
                }
            }
            bool ProcessPipe((int x, int y) point, int n, List<(int x, int y)> path)
            {
                if (graph.ContainsKey(point) && !visited[point.x, point.y])
                {
                    var pathNew = new List<(int x, int y)>(path);
                    n += 1;
                    pathNew.Add(point);
                    visited[point.x, point.y] = true;
                    distances[point.x, point.y] = n;
                    maxDistance = Math.Max(maxDistance, n);
                    queue.Enqueue((point, n, pathNew));
                    return true;
                }

                return false;
            }
            
            var loopPoints = new List<(int x, int y)>();
            var currentPoint = start;
            var previousPoint = (-1, -1); // Invalid initial point
            do
            {
                loopPoints.Add(currentPoint);
                // Find the next point in the loop that is not the previous point
                var nextPoint = graph[currentPoint].FirstOrDefault(p => p != previousPoint);
                previousPoint = currentPoint;
                currentPoint = nextPoint;
            } while (currentPoint != start && currentPoint != (0, 0)); // Assuming (0, 0) won't be part of the loop

            var enclosedCount = 0;
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var enclosed = IsPointInsideLoop(loopPoints, (i, j));
                    enclosedByLoop[i, j] = enclosed;
                    if (enclosed)
                    {
                        enclosedCount++;
                    }
                }
            }
            return (maxDistance.ToString(), enclosedCount.ToString());
        }
        public static bool IsPointInsideLoop(List<(int x, int y)> loopPoints, (int x, int y) point)
        {
            if (loopPoints.Contains(point))
            {
                return false;
            }
            
            int intersections = 0;
            int n = loopPoints.Count;

            for (int i = 0; i < n; i++)
            {
                (int x1, int y1) = loopPoints[i];
                (int x2, int y2) = loopPoints[(i + 1) % n];
                if ((y1 > point.y) != (y2 > point.y))
                {
                    int intersectX = x1 + (point.y - y1) * (x2 - x1) / (y2 - y1);

                    if (intersectX > point.x)
                    {
                        intersections++;
                    }
                }
            }

            // When odd, we are starting from inside the loop (crossed to the other side and not back in)
            return intersections % 2 != 0;
        }

    }
}
