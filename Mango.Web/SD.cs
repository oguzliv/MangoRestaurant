﻿namespace Mango.Web
{
    public static class SD
    {
        public static string ProdsuctAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            GET,POST,PUT,DELETE
        }
    }
}
