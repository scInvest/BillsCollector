using System;
using System.Collections.Generic;
using System.Text;

namespace BiedronkaParser.Tests.TDD
{
    [TestClass]
    public sealed class BiedronkaToBLConverterTests
    {
        [TestMethod]
        public void Ctor()
        {
            BiedronkaToBLConverter biedronkaToBLConverter = new BiedronkaToBLConverter();
        }

        [TestMethod]
        public async Task CreateIdAndData()
        {
            BiedronkaToBLConverter biedronkaToBLConverter = new BiedronkaToBLConverter();

            var datasource = new DataSrouce.API.Datasource();

            var files = datasource.ParagonTestData.GetParagonFiles();
            var receipts = await datasource.ParagonTestData.ReadFiles(files);


            foreach (var receipt in receipts)
            {
                var spendingCase = biedronkaToBLConverter.ConvertToStandardFromat(receipt);

                Assert.IsNotNull(spendingCase);

                Assert.AreNotEqual(default(DateTime), spendingCase.Date, "Date should not be default");
                Assert.IsTrue(spendingCase.Date.Year <= 2026, $"Date year should be 2026 or earlier, but was {spendingCase.Date.Year}");
                Assert.IsTrue(spendingCase.Date.Year >= 2020, $"Date year should be reasonable, but was {spendingCase.Date.Year}");

                Assert.IsNotNull(spendingCase.Id, "Id should not be null");
                Assert.IsFalse(string.IsNullOrEmpty(spendingCase.Id.ID), "ID should not be empty");

                Assert.IsNotNull(spendingCase.Id.AllIds, "AllIds should not be null");
                Assert.IsTrue(spendingCase.Id.AllIds.Count > 0, "AllIds should contain at least one ID");

                foreach (var kvp in spendingCase.Id.AllIds)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(kvp.Key), $"AllIds key should not be empty");
                    Assert.IsFalse(string.IsNullOrEmpty(kvp.Value), $"AllIds value for key '{kvp.Key}' should not be empty");
                }

                Assert.IsNotNull(spendingCase.Tags, "Tags should not be null");
                Assert.IsNotNull(spendingCase.Tags.Tags, "Tags.Tags should not be null");
                Assert.IsTrue(spendingCase.Tags.Tags.Count >= 2, $"Tags should contain at least 2 tags, but found {spendingCase.Tags.Tags.Count}");
                Assert.IsTrue(spendingCase.Tags.Tags.Any(t => t.Equals("biedronka", StringComparison.OrdinalIgnoreCase)), "Tags should contain 'biedronka' tag");

                Assert.IsNotNull(spendingCase.Summary, "Summary should not be null");
                Assert.IsTrue(spendingCase.Summary.Cost > 0, "Cost should be greater than 0");
                Assert.IsTrue(spendingCase.Summary.Total > 0, "Total should be greater than 0");
                Assert.AreEqual(spendingCase.Summary.Total + spendingCase.Summary.Discount, spendingCase.Summary.Cost, 0.01, "Cost should equal Total + Discount");
            }
        }

        [TestMethod]
        public async Task ChildrenAreProperlyInitialized()
        {
            BiedronkaToBLConverter biedronkaToBLConverter = new BiedronkaToBLConverter();

            var datasource = new DataSrouce.API.Datasource();

            var files = datasource.ParagonTestData.GetParagonFiles();
            var receipts = await datasource.ParagonTestData.ReadFiles(files);


            foreach (var receipt in receipts)
            {
                var spendingCase = biedronkaToBLConverter.ConvertToStandardFromat(receipt);

                Assert.IsNotNull(spendingCase.Node, "Node should not be null");
                Assert.IsNotNull(spendingCase.Node.Childs, "Children should not be null");
                Assert.IsTrue(spendingCase.Node.Childs.Count > 0, "Children collection should contain at least one item");

                foreach (var child in spendingCase.Node.Childs)
                {
                    Assert.IsNotNull(child, "Child should not be null");

                    Assert.IsNotNull(child.Id, "Child Id should not be null");
                    Assert.IsFalse(string.IsNullOrEmpty(child.Id.ID), "Child ID should not be empty");

                    Assert.IsFalse(string.IsNullOrEmpty(child.Name), "Child Name should not be empty");
                    Assert.IsFalse(string.IsNullOrEmpty(child.UserFriendlyName), "Child UserFriendlyName should not be empty");

                    Assert.IsNotNull(child.Summary, "Child Summary should not be null");
                    Assert.IsTrue(child.Summary.Total > 0, "Child Summary Total should be greater than 0");

                    Assert.IsNotNull(child.Tags, "Child Tags should not be null");
                    Assert.IsNotNull(child.Tags.Tags, "Child Tags.Tags should not be null");
                    Assert.IsTrue(child.Tags.Tags.Count > 0, "Child should have at least one tag");

                    Assert.IsNotNull(child.Node, "Child Node should not be null");
                    Assert.IsNotNull(child.Node.Childs, "Child Node Childs should not be null");

                    Assert.IsNotNull(child.Decorations, "Child Decorations should not be null");

                    Assert.AreEqual(spendingCase.Date, child.Date, "Child Date should equal parent Date");
                    Assert.IsTrue(child.Date.Year >= 2026, $"Child Date year should be 2026 or later, but was {child.Date.Year}");
                }
            }
        }
    }

}
