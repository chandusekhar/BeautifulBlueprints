using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BeautifulBlueprints.Layout.Svg
{
    public class PathLayout
    {
        private readonly string _svgPath;
        private readonly decimal _left;
        private readonly decimal _right;
        private readonly decimal _top;
        private readonly decimal _bottom;

        private PathPoint[] _points;
        public IEnumerable<PathPoint> Points
        {
            get
            {
                return _points;
            }
        }

        public PathLayout(string svgPath, decimal left, decimal right, decimal top, decimal bottom)
        {
            _svgPath = svgPath;
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
        }

        // All of the SVG commands
        private const string SEPARATORS = @"(?=[MZLHVCSQTAmzlhvcsqta])";

        public PathLayout Layout()
        {
            //Split path into tokens, one per command
            var commands = Regex.Split(_svgPath, SEPARATORS)
                                .Where(t => !string.IsNullOrEmpty(t))
                                .Select(SvgCommand.Parse)
                                .ToArray();

            //Evaluate all the SVG commands
            List<PathPoint> points = new List<PathPoint>();
            for (int i = 0; i < commands.Length; i++)
                commands[i].Execute(this, points, i == 0 ? null : (SvgCommand?)commands[i - 1]);

            //Points are in -1 to 1 range, conver to absolute position
            var width = _right - _left;
            var height = _top - _bottom;
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new PathPoint(
                    (points[i].X * 0.5m + 0.5m) * width + _left,
                    (points[i].Y * 0.5m + 0.5m) * height + _bottom,
                    points[i].StartOfLine
                );
            }

            _points = points.ToArray();
            return this;
        }

        private enum SvgCommandType
        {
// ReSharper disable InconsistentNaming
            M = 'M',
            Z = 'Z',
            L = 'L',
            H = 'H',
            V = 'V',
            C = 'C',
            S = 'S',
            Q = 'Q', 
            T = 'T',
            A = 'A',
            m = 'm',
            z = 'z',
            l = 'l',
            h = 'h',
            v = 'v',
            c = 'c',
            s = 's',
            q = 'q',
            t = 't',
            a = 'a'
// ReSharper restore InconsistentNaming
        }

        private struct SvgCommand
        {
            //Args in a command can be separated with space, comma or minus. If separated by minus we want to keep the symbol otherwise we want to discard it
            const string ARG_SEPARATORS = @"[\s,]|(?=-)";

            private SvgCommandType Command { get; set; }
            private decimal[] Arguments { get; set; }

            private SvgCommand(SvgCommandType command, params decimal[] arguments)
                : this()
            {
                Command = command;
                Arguments = arguments;
            }

            public static SvgCommand Parse(string pathString)
            {
                var cmd = (SvgCommandType)pathString.Take(1).Single();
                string remainingArgs = pathString.Substring(1);

                var splitArgs = Regex
                    .Split(remainingArgs, ARG_SEPARATORS)
                    .Where(t => !string.IsNullOrEmpty(t));

                decimal[] args = splitArgs.Select(decimal.Parse).ToArray();
                return new SvgCommand(cmd, args);
            }

            public void Execute(PathLayout layout, List<PathPoint> points, SvgCommand? previousCommand)
            {
                switch (Command)
                {
                    //Move To
                    case SvgCommandType.M:
                    case SvgCommandType.m: {
                        points.Add(new PathPoint(Arguments[0], Arguments[1], true));
                        return;
                    }

                    //Close Path
                    case SvgCommandType.Z:
                    case SvgCommandType.z: {
                        var start = Enumerable.Reverse(points).Cast<PathPoint?>().FirstOrDefault(a => a.HasValue && a.Value.StartOfLine) ?? points[0];
                        points.Add(new PathPoint(start.X, start.Y, false));
                        break;
                    }

                    //Line To
                    case SvgCommandType.L:
                    case SvgCommandType.l: {
                        points.Add(new PathPoint(Arguments[0], Arguments[1], false));
                        break;
                    }

                    //Horizontal Line To
                    case SvgCommandType.H:
                    case SvgCommandType.h: {
                        var last = points.Last();
                        points.Add(new PathPoint(Arguments[0], last.Y, false));
                        break;
                    }

                    //Vertical Line To
                    case SvgCommandType.V:
                    case SvgCommandType.v: {
                        var last = points.Last();
                        points.Add(new PathPoint(last.X, Arguments[0], false));
                        break;
                    }

                    case SvgCommandType.C:
                    case SvgCommandType.c:
                        throw new NotSupportedException("Cubic bezier curve not supported");


                    case SvgCommandType.S:
                    case SvgCommandType.s:
                        throw new NotSupportedException("Shortcut multiple cubic bezier not supported");

                    case SvgCommandType.Q:
                    case SvgCommandType.q:
                        throw new NotSupportedException("Quadratic Bezier curve not supported");

                    case SvgCommandType.T:
                    case SvgCommandType.t:
                        throw new NotSupportedException("Shortcut multiple quadratic bezier not supported");

                    case SvgCommandType.A:
                    case SvgCommandType.a:
                        throw new NotSupportedException("Arc not supported");

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public struct PathPoint
        {
            public decimal X { get; private set; }
            public decimal Y { get; private set; }
            public bool StartOfLine { get; private set; }

            public PathPoint(decimal x, decimal y, bool sol)
                : this()
            {
                X = x;
                Y = y;
                StartOfLine = sol;
            }
        }
    }
}

