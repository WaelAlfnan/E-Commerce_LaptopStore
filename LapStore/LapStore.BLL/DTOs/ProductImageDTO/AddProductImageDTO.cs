using LapStore.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.DTOs.ProductImageDTO
{
    public class AddProductImageDTO
    {
        #region Properties
        public int Id { get; set; }

        public string URL { get; set; }
        public int ProductId { get; set; }

        #endregion

        #region Methods
        public static ProductImage FromProductImageDTO(AddProductImageDTO productImageDTO)
        {
            return new ProductImage
            {
                Id = productImageDTO.Id,
                URL = productImageDTO.URL,
                ProductId = productImageDTO.ProductId
            };
        }
        #endregion
    }
}
