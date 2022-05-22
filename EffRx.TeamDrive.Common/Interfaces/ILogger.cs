namespace EffRx.TeamDrive.Common.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Trace(string message);
        void Fatal(string message);
    }
}
