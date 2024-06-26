public static class Helper
{
    public static async Task Log(ApplicationDbContext _Db, string message, string location = "")
    {
        _Db.Add(new Logger
        {
            Message   = message,
            CreatedAt = DateTime.Now,
            Location  = location
        });

        await _Db.SaveChangesAsync();
    }
}