namespace Service;

using Application;
public class DateTimeService : IDateTimeService
{
    public DateTime AmericaLimaTimeZone
    {
        get
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            var nowInLima = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            return nowInLima;
        }
    }
}