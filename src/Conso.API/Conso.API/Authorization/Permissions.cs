namespace Conso.API.Authorization
{
    public class ConsoPermissions
    {
        public const string Read = "Conso-read";
        public const string Write = "Conso-write";

        public static IEnumerable<string> Permissions 
        {
            get 
            { 
                return new[] { Read, Write };
            }
        }
    }
}
