using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Favorites;
using Headstone.Models.Requests;
using System.Collections.Generic;

namespace Headstone.Service.Interfaces
{
    public interface IFavoriteProductService
    {
        #region [ Queries ]

        FavoriteProductServiceResponse<FavoriteProducts> GetFavoriteProducts(FavoriteProductRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ FavoriteProduct ]

        FavoriteProductServiceResponse<FavoriteProducts> CreateFavoriteProduct(FavoriteProductCreated ev, List<ServiceLogRecord> logRecords = null);

        FavoriteProductServiceResponse<FavoriteProducts> DeleteFavoriteProduct(FavoriteProductDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}