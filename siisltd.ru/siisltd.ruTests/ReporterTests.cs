using ProgramNameSpace.Reports;
using ProgramNameSpace.Sessions;

namespace siisltd.ruTests
{
    [TestFixture]
    public class ReporterTests
    {
        private readonly string noExistsTestFilePath = Guid.NewGuid().ToString() + "test.csv";

        [Test]
        public async Task CalculateDailyMaxSessionsAsync_FileNotExists_ReturnsDictionary()
        {
            // Act
            var result = await Reporter.CalculateDailyMaxSessionsAsync(noExistsTestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf(typeof(Dictionary<DateTime, int>)));
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task CalculateOperatorStatesAsync_FileNotExists_ReturnsDictionary()
        {
            // Act
            var result = await Reporter.CalculateOperatorStatesAsync(noExistsTestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf(typeof(Dictionary<string, Dictionary<SessionState, int>>)));
            Assert.That(result.Count, Is.EqualTo(0));

            File.Delete(noExistsTestFilePath);
        }

        // Add more test cases for edge cases and error handling
    }
}