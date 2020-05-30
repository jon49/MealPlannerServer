using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MealPlanner.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Repository
{
    public static class ClientUser
    {
        public static UserTable UserTable(this AmazonDynamoDBClient client)
            => new UserTable(client);
    }

    public struct UserTable
    {
        private readonly AmazonDynamoDBClient client;

        public UserTable(AmazonDynamoDBClient client)
        {
            this.client = client;
        }

        public Dictionary<string, AttributeValue> GetKey(Guid id)
            => new Dictionary<string, AttributeValue> { { nameof(User.Id), new AttributeValue { S = id.ToString() } } };

        /// <summary>
        /// Get an item from the 'users' table.
        /// </summary>
        /// <param name="id">The id of the table (User.Id)</param>
        /// <param name="projection">The projection to be used, e.g., "AccessorId,UserInfo.LastName"</param>
        /// <returns></returns>
        public Task<GetItemResponse> GetItem(Guid id, string projection = null)
        {
            var request = new GetItemRequest
            {
                TableName = Meta.TableName.User,
                Key = GetKey(id),
                ProjectionExpression = projection,
            };

            return client.GetItemAsync(request);
        }

    }
}
