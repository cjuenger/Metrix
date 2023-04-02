namespace Io.Juenger.Scrum.GitLab.Configs;

internal class ProductRepositoryConfig : IProductRepositoryConfig
{
    // You can explore the RegEx here https://regexr.com/7bb8a
    private const string DefaultProductVisionPattern = @"(ProductVision:)\s*[^;]*;{1}";
    
    // You can explore the RegEx here https://regexr.com/7bc5l
    private const string DefaultProductTypePattern = @"(ProductType:)\s*[^;]*;{1}";
    
    // You can explore the RegEx here https://regexr.com/7bb8m
    private const string DefaultProductNamePattern = @"(Product:)\s*[^;]*;{1}";
    
    // You can explore the RegExe here https://regexr.com/7bb8p
    private const string DefaultProductKickoffPattern = @"(ProductKickoff:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?;{1}";
    
    // You can explore the RegExe here https://regexr.com/7bc5f
    private const string DefaultProductDueDatePattern = @"(ProductDueDate:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?;{1}";

    public string ProductVisionPattern { get; set; } = DefaultProductVisionPattern;
    public string ProductTypePattern { get; set; } = DefaultProductTypePattern;
    public string ProductNamePattern { get; set; } = DefaultProductNamePattern;
    public string ProductKickoffPattern { get; set; } = DefaultProductKickoffPattern;
    public string ProductDueDatePattern { get; set; } = DefaultProductDueDatePattern;
}