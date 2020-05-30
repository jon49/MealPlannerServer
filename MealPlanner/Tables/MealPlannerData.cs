using Amazon.DynamoDBv2.DataModel;
using System;

namespace MealPlanner.Tables
{
    [DynamoDBTable(Meta.TableName.MealPlanner)]
    public class MealPlannerData
    {
        /// <summary>
        /// See Users.AccessorId
        /// </summary>
        [DynamoDBHashKey]
        public Guid AccessorId { get; set; }

        /// <summary>
        /// A secondary index to get the actual data or save to the actual data directly.
        /// </summary>
        [DynamoDBRangeKey]
        public string DataId { get; set; }

        /// <summary>
        /// See UserVersion.Version
        /// This is the sorted key.
        /// </summary>
        [DynamoDBLocalSecondaryIndexRangeKey]
        public int Version { get; set; }

        /// <summary>
        /// The actual data :-)
        /// </summary>
        public string Data { get; set; }
    }
}
