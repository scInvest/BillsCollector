namespace BiedronkaParser.Tests.TDD
{
    [TestClass]
    public sealed class DatasourceTests
    {
        [TestMethod]
        public void ParagonTestData_ShouldNotBeNull()
        {
            var datasource = new DataSrouce.API.Datasource();

            Assert.IsNotNull(datasource.ParagonTestData);
        }

        [TestMethod]
        public void ParagonTestData_ShouldFindFiles()
        {
            var datasource = new DataSrouce.API.Datasource();

            var files = datasource.ParagonTestData.GetParagonFiles();

            Assert.IsNotNull(files);
            Assert.IsTrue(files.Count > 0, "Should find at least one paragon file");
            Assert.IsTrue(files.Count == 48, "Expected count");
        }

        [TestMethod]
        public async Task ReadFiles_ShouldReadAllFilesInParallel()
        {
            var datasource = new DataSrouce.API.Datasource();

            var files = datasource.ParagonTestData.GetParagonFiles();
            var receipts = await datasource.ParagonTestData.ReadFiles(files);

            Assert.IsNotNull(receipts);
            var receiptList = receipts.ToList();
            lkAssert.IsTrue(receiptList.Count == 48, $"Expected 48 receipts, got {receiptList.Count}");
        }
    }
}
