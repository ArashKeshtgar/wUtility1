namespace SchemaSyncApi;

// Mirrors the real Doctors / Works / DoctorsPriceGroups tables that
// DoctorsForm.xaml.cs and DoctorsPriceGroupsForm.xaml.cs read from the
// Totalsystem database — see those files for the original WPF behavior
// this is porting to the web.
public record Doctor(int Code, string FName, string LName, string Name, string? PostNo);

public record Work(int Code, string Name);

public record DoctorPriceGroup(
    int RowGd,
    int DrCode,
    int? WCode,
    decimal? CenterSharePrice,
    string? Title,
    string? DrName,
    string? WName
);

public record UpsertDoctorPriceGroupRequest(int DrCode, int? WCode, decimal? CenterSharePrice, string? Title);
