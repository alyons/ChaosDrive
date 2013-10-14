using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ChaosDrive.Extensions;

namespace ChaosDrive.Utility
{
    public class ChaosDriveMath
    {
        public static Vector2 CalculateBezierCurveLocation(Vector2[] points, float time)
        {
            if (time > 1.0f || time < 0.0f) throw new ArgumentException("Time must be greater than or equal to 0.0f or less than or equal to 1.0f");
            int n = points.Count() - 1;
            float T = 1- time;

            Vector2 sum = new Vector2(0, 0);

            for (int i = 0; i <= n; i++)
            {
                sum += points[i].Multiply(BinomialCoefficient(n, i) * Math.Pow(T, (n - i)) * Math.Pow(time, i));
            }

            return sum;
        }

        protected static float BinomialCoefficient(int n, int i)
        {
            return Factorial(n) / (Factorial(i) * Factorial(n - i));
        }

        protected static int Factorial(int n)
        {
            int result = 1;
            for (int i = n; i > 1; i--)
                result *= i;

            return result;
        }
    }
}
