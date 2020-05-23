using Bussiness;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Models.DTO;
using Models.Models;
using System.Net.Http;

namespace Services
{
    public class ReviewDataService : RestDataService<Review, ReviewDTO>, IReviewDataService
    {
        public ReviewDataService(HttpClient httpClient, IConfiguration configuration, IJSRuntime JSRuntime, ILocalStorageManagerService localStorageManagerService) : base(httpClient, configuration, JSRuntime, localStorageManagerService, "Review")
        {
            GetByFilterSelector = x => new Review
            {
                Id = x.Id,
                ReviewComment = x.ReviewComment,
                Stars = x.Stars
            };
        }
    }
}
