using System;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Helpers
{
    public static class SessionManager
    {
        public static Kullanici? CurrentUser { get; private set; }

        public static void Login(Kullanici user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn => CurrentUser != null;

        public static bool IsAdmin => CurrentUser?.Rol == "Admin";

        public static bool IsTekniker => CurrentUser?.Rol == "Tekniker" || IsAdmin;

        public static bool IsKasiyer => CurrentUser?.Rol == "Kasiyer" || IsAdmin;
    }
}
