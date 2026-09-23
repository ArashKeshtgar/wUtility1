namespace SchemaSyncApi;

// The 13 module names Connections.xaml.cs manages — see
// CommonClass.cs DatabaseModelName.ConnectedDatabases in the WPF app.
public static class WuModules
{
    public static readonly string[] All =
    [
        "Totalsystem", "Lab", "Radio", "Drug", "Blood", "OpRoom",
        "Phisio", "Food", "Den", "Tasisat", "Tajhizat", "MDoc", "Requests"
    ];
}

public record SiteConnectionSummary(int Id, string ModuleName, string DisplayName, DateTime CreatedAt);

public record AddSiteConnectionRequest(string ModuleName, string DisplayName, string ConnectionString);

public record TestConnectionResult(bool Success, string? Error);
