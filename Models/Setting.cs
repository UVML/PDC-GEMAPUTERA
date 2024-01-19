using System.ComponentModel.DataAnnotations;

public class Setting
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
}


public class Setting_SMTP
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool UseSSL { get; set; }
    public string From { get; set; }
}

public class Setting_SMS
{
    public string Url { get; set; }
    public string API { get; set; }
}
