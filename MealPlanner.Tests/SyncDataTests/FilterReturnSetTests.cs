using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MealPlanner.Tables;
using System;
using MealPlanner.Models;
using System.Linq;

namespace MealPlanner.Tests.SyncDataTests
{
    [TestClass]
    public class FilterReturnSetTests
    {
        private readonly Guid ACCESSOR_ID = Guid.NewGuid();

        [TestMethod]
        public void Should_only_return_items_which_are_not_being_saved()
        {
            var data1 = GetMealPlannerData();
            data1.DataId = "filtered";
            var data2 = GetMealPlannerData();
            data2.DataId = "correct";
            var updatedItems = new List<MealPlannerData> { data1, data2 };

            var sync = GetDataSync();
            sync.Key = "filtered";
            var newItems = new[] { sync };

            var (_, returnItems) = SyncData.FilterReturnSet(1, newItems, updatedItems);

            Assert.IsNotNull(returnItems);
            Assert.AreEqual(1, returnItems.Count(), "Should contain only one item.");
            Assert.AreEqual("correct", returnItems.FirstOrDefault().DataId);
        }

        [TestMethod]
        public void Should_return_the_max_version_when_updated_items_has_max()
        {
            var data1 = GetMealPlannerData();
            data1.Version = 5;
            var data2 = GetMealPlannerData();
            data2.Version = 6;
            var data = new List<MealPlannerData> { data1, data2 };
            var newItem = GetDataSync();

            var (maxVersion, _) = SyncData.FilterReturnSet(3, new[] { newItem }, data);

            Assert.AreEqual(6, maxVersion);
        }

        [TestMethod]
        public void Should_return_the_max_version_when_new_item_is_newest()
        {
            var data1 = GetMealPlannerData();
            data1.Version = 5;
            var data2 = GetMealPlannerData();
            data2.Version = 6;
            var data = new List<MealPlannerData> { data1, data2 };
            var newItem = GetDataSync();

            var (maxVersion, _) = SyncData.FilterReturnSet(7, new[] { newItem }, data);

            Assert.AreEqual(7, maxVersion);
        }

        private MealPlannerData GetMealPlannerData()
            => new MealPlannerData
            {
                AccessorId = ACCESSOR_ID,
                Data = "Empty",
                DataId = "Id",
                Version = 1,
            };

        private DataSync GetDataSync()
            => new DataSync
            {
                Data = "EmptySync",
                Key = "IdSync"
            };
    }
}
