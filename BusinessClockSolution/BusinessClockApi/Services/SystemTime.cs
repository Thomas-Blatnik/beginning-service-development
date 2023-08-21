namespace BusinessClockApi.Services;

public class SystemTime : ISystemTime
{
    public DateTime GetCurrent()
    {
        return DateTime.Now;
    }
}

public interface ISystemTime
{
    public DateTime GetCurrent();
}