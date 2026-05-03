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

            var receipt = receipts.First();
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
        }
    }

}
