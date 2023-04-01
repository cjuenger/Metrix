namespace Io.Juenger.Scrum.GitLab.Configs;

internal class ProductRepositoryConfig : IProductRepositoryConfig
{
    // You can explore the RegEx here https://regexr.com/7bb8a
    private const string DefaultProductVisionPattern = @"(ProductVision:)\s*[^;]*;{1}";
    
    // You can explore the RegEx here https://regexr.com/7bb8j
    private const string DefaultRepositoryTypePattern = @"(RepositoryType:)\s*[^;]*;{1}";
    
    // You can explore the RegEx here https://regexr.com/7bb8m
    private const string DefaultProductPattern = @"(Product:)\s*[^;]*;{1}";
    
    // You can explore the RegExe here https://regexr.com/7bb8p
    private const string DefaultProductKickoffPattern = @"(ProductKickoff:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?;{1}";
    
    // You can explore the RegExe here https://regexr.com/7bb92
    private const string DefaultProductEndPattern = @"(ProductEnd:)\s*(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?;{1}";

    public string ProductVisionPattern { get; set; } = DefaultProductVisionPattern;
    public string RepositoryTypePattern { get; set; } = DefaultRepositoryTypePattern;
    public string ProductPattern { get; set; } = DefaultProductPattern;
    public string ProductKickoffPattern { get; set; } = DefaultProductKickoffPattern;
    public string ProductEndPattern { get; set; } = DefaultProductEndPattern;
}