using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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

            var width = _right - _left;
            var height = _top - _bottom;

            //Evaluate all the SVG commands
            List<PathPoint> points = new List<PathPoint>();
            for (int i = 0; i < commands.Length; i++)
                commands[i].Execute(this, points, i == 0 ? null : (SvgCommand?)commands[i - 1], width, height);

            //Points are in -1 to 1 range, convert to absolute position
            for (int i = 0; i < points.Count; i++)
            {
                var x = Math.Max(Math.Min(1, points[i].X), -1);
                var y = Math.Max(Math.Min(1, points[i].Y), -1);

                points[i] = new PathPoint(
                    (x * 0.5m + 0.5m) * width + _left,
                    (y * 0.5m + 0.5m) * height + _bottom,
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

            public void Execute(PathLayout layout, List<PathPoint> points, SvgCommand? previousCommand, decimal width, decimal height)
            {
                const decimal CURVE_ERR = 5;
                decimal curveError = (CURVE_ERR / width) * (CURVE_ERR / height);

                var previousPoint = points.Cast<PathPoint?>().LastOrDefault() ?? new PathPoint(0, 0, true);

                switch (Command)
                {
                    //Move To
                    case SvgCommandType.M: {
                        points.Add(new PathPoint(Arguments[0], Arguments[1], true));
                        return;
                    }
                    case SvgCommandType.m: {
                        points.Add(new PathPoint(previousPoint.X + Arguments[0], previousPoint.Y + Arguments[1], true));
                        return;
                    }

                    //Close Path
                    case SvgCommandType.Z:
                    case SvgCommandType.z: {
                        var start = Enumerable.Reverse(points).Cast<PathPoint?>().FirstOrDefault(a => a.HasValue && a.Value.StartOfLine) ?? points[0];
                        points.Add(new PathPoint(start.X, start.Y, false));
                        return;
                    }

                    //Line To
                    case SvgCommandType.L: {
                        points.Add(new PathPoint(Arguments[0], Arguments[1], false));
                        return;
                    }
                    case SvgCommandType.l: {
                        points.Add(new PathPoint(previousPoint.X + Arguments[0], previousPoint.Y + Arguments[1], false));
                        return;
                    }

                    //Horizontal Line To
                    case SvgCommandType.H: {
                        points.Add(new PathPoint(Arguments[0], previousPoint.Y, false));
                        return;
                    }
                    case SvgCommandType.h: {
                        points.Add(new PathPoint(previousPoint.X + Arguments[0], previousPoint.Y, false));
                        return;
                    }

                    //Vertical Line To
                    case SvgCommandType.V: {
                        points.Add(new PathPoint(previousPoint.X, Arguments[0], false));
                        return;
                    }
                    case SvgCommandType.v: {
                        points.Add(new PathPoint(previousPoint.X, previousPoint.Y + Arguments[0], false));
                        return;
                    }

                    //Bezier curve
                    case SvgCommandType.C: {
                        EvaluateCubicBezier(points, curveError);
                        return;
                    }
                    case SvgCommandType.c:
                        throw new NotSupportedException("Relative positioned cubic bezier curve not supported");

                    //Shortcut cubic bezier (read control points from previous command if it was a cubic bezier)
                    case SvgCommandType.S:
                    case SvgCommandType.s:
                        throw new NotSupportedException("Shortcut multiple cubic bezier not supported");

                    //Quadratic bezier
                    case SvgCommandType.Q: {
                        EvaluateQuadraticBezier(points, curveError);
                        return;
                    }
                    case SvgCommandType.q:
                        throw new NotSupportedException("Relative positioned quadratic Bezier curve not supported");

                    //Shortcut quadratic bezier (read control points from previous command if it was a quadratic bezier)
                    case SvgCommandType.T:
                    case SvgCommandType.t:
                        throw new NotSupportedException("Shortcut multiple quadratic bezier not supported");

                    //Arc
                    case SvgCommandType.A:
                    case SvgCommandType.a:
                        throw new NotSupportedException("Arc not supported");

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void EvaluateQuadraticBezier(List<PathPoint> points, decimal error)
            {
                //Line start
                var start = points.Last();

                //Control point
                var cx = Arguments[0];
                var cy = Arguments[1];

                //Line end
                var ex = Arguments[2];
                var ey = Arguments[3];

                //Evaluator for 1 dimension of a curve
                Func<decimal, decimal, decimal, decimal, decimal> evaluator = (s, c, e, t) => (decimal)Math.Pow(1 - (double) t, 2) * s + 2 * (1 - t) * t * c + t * t * e;

                //Evaluators for specific dimensions
                Func<decimal, decimal> evalX = t => evaluator(start.X, cx, ex, t);
                Func<decimal, decimal> evalY = t => evaluator(start.Y, cy, ey, t);

                //Now evaluate the curve
                EvaluateCurve(points, evalX, evalY, error);
            }

            private void EvaluateCubicBezier(List<PathPoint> points, decimal error)
            {
                //Line start
                var start = points.Last();

                //Start control point
                var x1 = Arguments[0];
                var y1 = Arguments[1];

                //end control points
                var x2 = Arguments[2];
                var y2 = Arguments[3];

                //Line end
                var x = Arguments[4];
                var y = Arguments[5];

                //Evaluator function for 1 dimension of a curve
                Func<decimal, decimal, decimal, decimal, decimal, decimal> evaluator = (a, b, c, d, t) => (decimal)Math.Pow(1 - (double)t, 3) * a + 3 * (decimal)Math.Pow(1 - (double)t, 2) * t * b + 3 * (1 - t) * t * t * c + t * t * t * d;

                //Evaluators for specific dimensions
                Func<decimal, decimal> evalX = t => evaluator(start.X, x1, x2, x, t);
                Func<decimal, decimal> evalY = t => evaluator(start.Y, y1, y2, y, t);

                //Now evaluate the curve
                EvaluateCurve(points, evalX, evalY, maxError: error);
            }

            private void EvaluateCurve(List<PathPoint> points, Func<decimal, decimal> evalX, Func<decimal, decimal> evalY, decimal maxError = 10)
            {
                List<KeyValuePair<decimal, PathPoint>> wip = new List<KeyValuePair<decimal, PathPoint>>();

                //Evaluate the start and end points of the curve
                var startX = evalX(0);
                var startY = evalY(0);
                var endX = evalX(1);
                var endY = evalY(1);

                //Add start and end points to list
                wip.Add(new KeyValuePair<decimal, PathPoint>(0, new PathPoint(startX, startY, false)));
                wip.Add(new KeyValuePair<decimal, PathPoint>(1, new PathPoint(endX, endY, false)));

                RecursiveEvaluateCurveSegment(wip, evalX, evalY, maxError, 0, 1);

                points.AddRange(wip.OrderBy(a => a.Key).Select(a => a.Value));
            }

            private void RecursiveEvaluateCurveSegment(List<KeyValuePair<decimal, PathPoint>> points, Func<decimal, decimal> evalX, Func<decimal, decimal> evalY, decimal maxError, decimal start, decimal end)
            {
                //Evaluate the start and end points of this curve segment
                var startX = evalX(start);
                var startY = evalY(start);
                var endX = evalX(end);
                var endY = evalY(end);

                //Evaluate the middle of this curve segment
                var midT = start * 0.5m + end * 0.5m;
                var midX = evalX(midT);
                var midY = evalY(midT);

                //What's the area of the curve (estimated by area of the triangle formed with mid point)
                var trueArea = Math.Abs(startX * (midY - endY) + midX * (endY - startY) + endX * (startY - midY)) / 2;

                //What's the area of our approximation (currently always a line)
                var estArea = 0;

                //If the area is small we've reached a good enough approximation, terminate recursion
                if (trueArea - estArea < maxError)
                    return;

                //Insert a point in the middle of the endpoints, making the approximation more accurate
                points.Add(new KeyValuePair<decimal, PathPoint>(midT, new PathPoint(midX, midY, false)));

                //Evaluate the two halves of the curve
                RecursiveEvaluateCurveSegment(points, evalX, evalY, maxError, midT, end);
                RecursiveEvaluateCurveSegment(points, evalX, evalY, maxError, start, midT);
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

