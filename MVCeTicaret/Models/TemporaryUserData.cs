using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCeTicaret.Models
{
    public static class TemporaryUserData//online kullanici id sini tutuyor
    {
        public static int UserID { get; set; }
        public static string ReturnUrl { get; set; }

    }
}