using LapStore.DAL.Data.Entities;

public class GetProductImageDTO
{
    public int Id { get; set; }
    public string URL { get; set; }
    public int ProductId { get; set; }
    public bool IsMain { get; set; }

    public static GetProductImageDTO FromProductImage(ProductImage image)
    {
        if (image == null)
            return null;

        return new GetProductImageDTO
        {
            Id = image.Id,
            URL = image.URL,
            ProductId = image.ProductId,
            IsMain = image.IsMain
        };
    }
}