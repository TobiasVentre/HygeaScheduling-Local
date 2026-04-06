namespace SchedulingMS.Api.Security;

public static class SecurityConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string ProviderAdmin = "ProviderAdmin";
        public const string Technician = "Technician";
        public const string Client = "Client";
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string ProviderAdminOrAdmin = "ProviderAdminOrAdmin";
        public const string TechnicianOnly = "TechnicianOnly";
        public const string ClientOnly = "ClientOnly";
    }
}
