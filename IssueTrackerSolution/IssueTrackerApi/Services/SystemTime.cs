namespace IssueTrackerApi.Services;

public interface ISystemTime
{
    DateTime GetCurrent();
}

public class SystemTime : ISystemTime
{
    public DateTime GetCurrent() => DateTime.Now;
}
