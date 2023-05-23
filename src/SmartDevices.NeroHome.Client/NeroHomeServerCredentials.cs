namespace SmartDevices.NeroHome.Client;

public class NeroHomeServerCredentials
{
    public NeroHomeServerCredentials(
        string login,
        string password)
    {
        this.Login = login;
        this.Password = password;
    }

    public string Login { get; }

    public string Password { get; }
}
