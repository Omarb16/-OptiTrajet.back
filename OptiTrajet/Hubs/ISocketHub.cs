namespace OptiTrajet.Hubs
{
    public interface ISocketHub
    {
        public Task SendProgress(string connectionId, int progress);
    }
}