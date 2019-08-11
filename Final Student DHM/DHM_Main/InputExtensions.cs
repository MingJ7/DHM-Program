using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DHM_Main
{
    static class InputExtensions
    {
        public static int LimitToRange(this int value, int inclusiveMinimum, int inclusiveMaximum) {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }


        public static Point Minus(this Point thisP, Point otherP) {
            return new Point {
                X = thisP.X - otherP.X,
                Y = thisP.Y - otherP.Y
            };
        }
    }
}
