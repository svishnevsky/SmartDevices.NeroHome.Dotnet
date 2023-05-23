namespace SmartDevices.NeroHome.Client.Commands;

public class AuthCommand : DeviceCommand<DeviceCommandReply>
{
    public AuthCommand(
        string login,
        string password)
        : base(CommandClasses.Auth, "register")
    {
        this.Login = login;
        this.Password = password;
    }

    public string Login { get; }

    public string Password { get; }
}
