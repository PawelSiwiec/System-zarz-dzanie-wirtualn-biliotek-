using System.Text.RegularExpressions;

namespace LibraryManager;

public static class Validator
{
    private static readonly Regex EmailRx =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static bool EmailPoprawny(string e)
        => EmailRx.IsMatch(e ?? "");
}
