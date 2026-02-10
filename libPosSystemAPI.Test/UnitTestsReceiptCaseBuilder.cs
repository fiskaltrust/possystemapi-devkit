using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using Xunit;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    public class UnitTests
    {
        #region ReceiptCaseBuilder Tests

        [Fact]
        public void ReceiptCaseBuilder_SetCountry_AT_EncodesCorrectly()
        {
            // Arrange & Act
            var result = new ReceiptCaseBuilder()
                .SetCountry("AT")
                .SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001)
                .Build();

            ulong expectedCountryPartAT = 0x4154_0000_0000_0000;
            ulong expected = expectedCountryPartAT + (ulong)ReceiptCase.PointOfSaleReceipt0x0001;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ReceiptCaseBuilder_SetCountry_LowercaseConverted()
        {
            // Arrange & Act
            var resultLower = new ReceiptCaseBuilder()
                .SetCountry("at")
                .SetReceiptCase(ReceiptCase.ZeroReceipt0x2000)
                .Build();

            var resultUpper = new ReceiptCaseBuilder()
                .SetCountry("AT")
                .SetReceiptCase(ReceiptCase.ZeroReceipt0x2000)
                .Build();

            // Assert - lowercase should be converted to uppercase
            Assert.Equal(resultUpper, resultLower);
        }

        [Fact]
        public void ReceiptCaseBuilder_SetCountry_InvalidLength_ThrowsException()
        {
            // Arrange
            var builder = new ReceiptCaseBuilder();

            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => builder.SetCountry("A"));
            Assert.Throws<System.ArgumentException>(() => builder.SetCountry("AUT"));
            Assert.Throws<System.ArgumentException>(() => builder.SetCountry(""));
        }

        [Fact]
        public void ReceiptCaseBuilder_DifferentReceiptCases_EncodedCorrectly()
        {
            // Act - each builder must have country set before building
            var posReceipt = new ReceiptCaseBuilder().SetCountry("AT").SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001).Build();
            var zeroReceipt = new ReceiptCaseBuilder().SetCountry("AT").SetReceiptCase(ReceiptCase.ZeroReceipt0x2000).Build();
            var dailyClosing = new ReceiptCaseBuilder().SetCountry("AT").SetReceiptCase(ReceiptCase.DailyClosing0x2011).Build();

            // The country part should be the same for all
            ulong expectedCountryPartAT = 0x4154_0000_0000_0000;
            ulong countryMask = 0xFFFF_0000_0000_0000;
            Assert.Equal(expectedCountryPartAT, posReceipt & countryMask);
            Assert.Equal(expectedCountryPartAT, zeroReceipt & countryMask);
            Assert.Equal(expectedCountryPartAT, dailyClosing & countryMask);

            // The receipt case part should differ
            ulong caseMask = 0x0000_FFFF_FFFF_FFFF;
            Assert.Equal((ulong)ReceiptCase.PointOfSaleReceipt0x0001, posReceipt & caseMask);
            Assert.Equal((ulong)ReceiptCase.ZeroReceipt0x2000, zeroReceipt & caseMask);
            Assert.Equal((ulong)ReceiptCase.DailyClosing0x2011, dailyClosing & caseMask);
        }

        [Fact]
        public void ReceiptCaseBuilder_FluentInterface_ReturnsSameInstance()
        {
            // Arrange
            var builder = new ReceiptCaseBuilder();

            // Act
            var result1 = builder.SetCountry("AT");
            var result2 = result1.SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001);

            // Assert
            Assert.Same(builder, result1);
            Assert.Same(builder, result2);
        }

        #endregion
    }
}