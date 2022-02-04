namespace LittleToolCollection;

public class TimeMeasurer {
    private DateTime _timeStart;

    public TimeMeasurer() {
        SetTimeToNow();
    }
    public void SetTimeToNow() {
        _timeStart = DateTime.Now;
    }

    public TimeSpan PrintTimeUsedSinceLastTask(string taskMsg = "perform previous task.") {
        var diff = DateTime.Now - _timeStart;
        Console.WriteLine($"Used {diff.Milliseconds} milliseconds to {taskMsg}");
        _timeStart = DateTime.Now;

        return diff;
    }
}
