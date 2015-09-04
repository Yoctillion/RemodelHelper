using System;

namespace RemodelHelper.Models
{
    [Flags]
    public enum Week
    {
        None = 0,
        Sunday = 1,
        Monday = 1 << 1,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        Saturday = 1 << 6,
    }

    public static class WeekConverter
    {
        public static Week Convert(this DayOfWeek day) => (Week)(1 << (int)day);

        public static DayOfWeek Convert(this Week day)
        {
            var dayCount = 0;
            var res = default(DayOfWeek);
            for (var i = 0; i < 7; i++)
            {
                if (day.HasFlag((Week)(1 << i)))
                {
                    dayCount++;
                    res = (DayOfWeek)i;
                }
            }

            if (dayCount != 1) throw new InvalidCastException("错误的星期格式");
            return res;
        }
    }
}
