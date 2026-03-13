using System.Text.RegularExpressions;
using NewRentalCarManagerAPI.Domain.Interfaces;

namespace NewRentalCarManagerAPI.Infrastructure.Services;

public class SlugService : ISlugService
{
    public string GenerateSlug(string input)
    {
        var slug = input.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
