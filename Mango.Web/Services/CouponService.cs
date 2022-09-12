using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory _clientFactory;
        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _clientFactory = httpClient;
        }
        public async Task<T> GetCoupon<T>(string couponCode, string token)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                //Data = couponCode,
                Url = SD.CouponAPIBase + "/api/coupon/GetCouponCode/" + couponCode,
                AccessToken = token
            }); ;
        }
    }
}
