using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MealPlanner.Models;
using MealPlanner.Repository;
using MealPlanner.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MealPlanner.Tests")]
namespace MealPlanner
{
    public static class SyncData
    {
        public static async Task<(int newVersion, List<MealPlannerData>)> Sync(DataSync[] data, int currentUserVersion, Guid userId)
        {
            var config = new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2,
            };
            var client = new AmazonDynamoDBClient();

            using var context = new DynamoDBContext(client, config);

            var user = await client.UserTable().GetItem(id: userId, projection: nameof(User.AccessorId));
            var accessorId = user.Item[nameof(User.AccessorId)].S;
            var lastUpdatedItems = await context.QueryAsync<MealPlannerData>(
                accessorId,
                QueryOperator.GreaterThan,
                new object[] { currentUserVersion },
                new DynamoDBOperationConfig
                {
                    IndexName = nameof(MealPlannerData.Version),
                }).GetRemainingAsync();

            // Assume current version is the latest
            var (maxVersion, filteredLastUpdatedItems) = FilterReturnSet(currentUserVersion, data, lastUpdatedItems);

            // Save latest data
            var batch = context.CreateBatchWrite<MealPlannerData>();
            var newVersion = maxVersion + 1;
            batch.AddPutItems(data.Select(x => new MealPlannerData
            {
                AccessorId = new Guid(accessorId),
                Data = x.Data,
                DataId = x.Key,
                Version = newVersion,
            }));

            await batch.ExecuteAsync();

            return (newVersion, filteredLastUpdatedItems);
        }

        internal static (int latestVersion, List<MealPlannerData> returnItems) FilterReturnSet(
            int userVersion,
            DataSync[] newItems,
            List<MealPlannerData> updatedItems)
        {
            var newItemIds = newItems.Select(x => x.Key).ToArray();

            // Assume current version is the latest
            var maxVersion = userVersion;
            var filteredData = FilterData().ToList();

            return (maxVersion, filteredData);

            IEnumerable<MealPlannerData> FilterData()
            {
                foreach (var item in updatedItems)
                {
                    if (item.Version > maxVersion) maxVersion = item.Version;

                    // If we are saving the data to the database then don't return an item which was
                    // saved in a previous sync
                    if (Array.IndexOf(newItemIds, item.DataId) == -1) yield return item;
                }
            }
        }
    }
}
