namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogRenamedData
{
    public TrackingLogRenamedData(string renamedFrom, 
                                  string renamedTo)
    {
        RenamedFrom = renamedFrom;
        RenamedTo = renamedTo;
    }

    public string RenamedFrom { get; }
    public string RenamedTo { get; }
}