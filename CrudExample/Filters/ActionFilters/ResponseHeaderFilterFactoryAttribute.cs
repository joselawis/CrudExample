using Microsoft.AspNetCore.Mvc.Filters;

namespace CrudExample.Filters.ActionFilters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;
    private string Key { get; }
    private string? Value { get; }
    private int Order { get; }

    public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
    {
        Key = key;
        Value = value;
        Order = order;
    }

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
        filter.Key = Key;
        filter.Value = Value;
        filter.Order = Order;
        return filter;
    }
}
