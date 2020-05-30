using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace MealPlanner.Tests
{
    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        public void Should_work()
        {
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest();
            var function = new Functions();
            var response = function.SyncData(request, context);
            Debug.WriteLine(response.Body);
        }

        [TestMethod]
        public void Should_save_new_item()
        {
        }
    }
}
