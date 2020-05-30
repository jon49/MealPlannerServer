using Amazon.DynamoDBv2.DataModel;
using MealPlanner.Repository;
using System;

namespace MealPlanner.Tables
{
    [DynamoDBTable(Meta.TableName.User)]
    public class User
    {
        /// <summary>
        /// Guid assigned to the user. This will most likely go away once I get proper
        /// authentication.
        /// </summary>
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        /// <summary>
        /// Used to access the actual data of the user.
        /// </summary>
        public Guid AccessorId { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class UserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
