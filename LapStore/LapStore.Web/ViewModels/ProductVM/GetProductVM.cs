using System.ComponentModel.DataAnnotations;
using LapStore.Web.ViewModels.ProductImageVM;

namespace LapStore.Web.ViewModels.ProductVM
{
    public class GetProductVM
    {
        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<GetProductImageVM> Images { get; set; } = new List<GetProductImageVM>();

        public string MainImageUrl { get; set; }
        #endregion
    }
}