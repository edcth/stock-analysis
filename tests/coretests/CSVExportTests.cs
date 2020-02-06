using System;
using core;
using core.Notes;
using core.Options;
using core.Stocks;
using Xunit;

namespace coretests
{
    public class CSVExportTests
    {
        [Fact]
        public void ExportStocksHeader()
        {
            var stock = new OwnedStock("ticker", Guid.NewGuid());
            stock.Purchase(1, 100, DateTime.UtcNow);

            var report = CSVExport.Generate(new[] {stock});

            Assert.Contains(CSVExport.STOCK_HEADER, report);
            Assert.Contains("ticker", report);
            Assert.Contains("ticker", report);
        }

        [Fact]
        public void ExportOptionsHeader()
        {
            var option = new OwnedOption(
                "ticker",
                2.5,
                OptionType.CALL,
                DateTimeOffset.UtcNow.AddDays(1),
                Guid.NewGuid());
            
            option.Sell(1, 20, DateTimeOffset.UtcNow);

            var report = CSVExport.Generate(new[] {option});

            Assert.Contains(CSVExport.OPTION_HEADER, report);
            
            Assert.Contains("ticker", report);
            Assert.Contains("CALL", report);
            Assert.Contains("2.5", report);
        }

        [Fact]
        public void ExportNotes()
        {
            var note = new Note(Guid.NewGuid(), "my note", "stockticker", DateTimeOffset.UtcNow);

            var report = CSVExport.Generate(new[] {note});

            Assert.Contains(CSVExport.NOTE_HEADER, report);
            Assert.Contains("stockticker", report);
            Assert.Contains("my note", report);
        }

        [Fact]
        public void FilenameEndsWithCSV()
        {
            var filename = CSVExport.GenerateFilename("option");

            Assert.Contains("option", filename);
            Assert.EndsWith("csv", filename);
        }
    }
}