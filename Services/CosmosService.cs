using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public abstract class CosmosService<T> : ICosmosService<T> where T : BaseModel
{
    private readonly CosmosClient _client;
    private readonly string _containerName;

    public CosmosService(IConfiguration config, string containerName)
    {
        _client = new CosmosClient(connectionString: config.GetConnectionString("Cosmos"));
        _containerName = containerName;
    }

    private Container container => _client.GetDatabase("Attendance").GetContainer(_containerName);

    public async Task BulkInsert(IEnumerable<T> models)
    {
        await Task.WhenAll(models.ToList().Select(x => Insert(x)));
    }

    public async Task BulkUpsert(IEnumerable<T> models)
    {
        await Task.WhenAll(models.ToList().Select(x => Upsert(x)));
    }

    public async Task Insert(T model)
    {
        await container.CreateItemAsync(model);
    }

    public async Task<IEnumerable<T>> RetrieveAllAsync()
    {
        var queryable = container.GetItemLinqQueryable<T>();

        using FeedIterator<T> feed = queryable.ToFeedIterator();

        List<T> results = new();

        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            foreach (var item in response)
            {
                results.Add(item);
            }
        }

        return results;
    }

    //public async Task<IEnumerable<T>> RetrieveAsync(string sql)
    //{
    //    string sql = """
    //    SELECT
    //        p.id,
    //        p.categoryId,
    //        p.categoryName,
    //        p.sku,
    //        p.name,
    //        p.description,
    //        p.price,
    //        p.tags
    //    FROM products p
    //    JOIN t IN p.tags
    //    WHERE t.name = @tagFilter
    //    """;

    //    var query = new QueryDefinition(
    //        query: sql
    //    )
    //        .WithParameter("@tagFilter", "Tag-75");

    //    using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
    //        queryDefinition: query
    //    );

    //    List<Product> results = new();

    //    while (feed.HasMoreResults)
    //    {
    //        var response = await feed.ReadNextAsync();
    //        foreach (Product item in response)
    //        {
    //            results.Add(item);
    //        }
    //    }

    //    return results;
    //}

    public async Task Upsert(T model)
    {
        await container.UpsertItemAsync(model);
    }
}
