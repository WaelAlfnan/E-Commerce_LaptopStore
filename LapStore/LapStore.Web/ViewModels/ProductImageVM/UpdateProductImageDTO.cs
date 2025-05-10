namespace LapStore.Web.ViewModels.ProductImageVM
{
    public class UpdateProductImageVM
    {
        #region Properties
        public int Id { get; set; }

        public string URL { get; set; }
        public int ProductId { get; set; }

        #endregion

        #region Methods
        public static UpdateProductImageVM FromGetProductImageVM(UpdateProductImageVM productImageVM)
        {
            return new UpdateProductImageVM
            {
                Id = productImageVM.Id,
                URL = productImageVM.URL,
                ProductId = productImageVM.ProductId
            };
        }
        #endregion
    }
}
