namespace Movie_management_system.Helper
{
    public static class TokenManager
    {
        public static string Token { get; set; }
        public static string Role { get; set; }
        public static string Email { get; set; }
        public static int UserId { get; set; }
        public static string Name { get; set; }
        public static bool IsAdmin => Role == "Admin";
    }

}
