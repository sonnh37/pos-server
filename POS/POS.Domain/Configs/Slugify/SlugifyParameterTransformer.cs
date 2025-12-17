using Humanizer;
using Microsoft.AspNetCore.Routing;

namespace POS.Domain.Configs.Slugify;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;
        return value.ToString().Pluralize()?.ToLower();
    }
}