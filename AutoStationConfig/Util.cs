namespace Pasukaru.DSP.AutoStationConfig
{
    public static class Util
    {
        public static double AU(double num)
        {
            return num * 40000;
        }

        public static double LY(double num)
        {
            return num * 2400000D;
        }

        public static double PERCENT(double num)
        {
            return num / 100;
        }

        public static double MINMAXCHECK(double min, double max, double num)
        {
            if (num < min)
            {
                return min;
            }

            else if (num > max)
            {
                return max;
            }

            return num;
        }
    }
}