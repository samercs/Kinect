namespace Kinect.DB
{
    public enum Providers
    {
        SqlServer,
        OleDb,
        Oracle,
        ODBC,
        ConfigDefined
    }

    public enum ConnectionState
    {
        KeepOpen,
        CloseOnExit
    }

    public enum MovieType
    {
        Action=1,
        Comday =2,
        Horror =3,
        Drama=4
    }
}