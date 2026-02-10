using System;
using System.Text.Json;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using Xunit;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    public class UnitTestsFtState
    {
        // Test values:
        // AT country code: 'A' = 0x41, 'T' = 0x54 -> 0x4154 << 48 = 0x4154_0000_0000_0000
        // DE country code: 'D' = 0x44, 'E' = 0x45 -> 0x4445 << 48 = 0x4445_0000_0000_0000
        // FR country code: 'F' = 0x46, 'R' = 0x52 -> 0x4652 << 48 = 0x4652_0000_0000_0000
        // IT country code: 'I' = 0x49, 'T' = 0x54 -> 0x4954 << 48 = 0x4954_0000_0000_0000

        #region Constructor and Value Tests

        [Fact]
        public void FtState_Constructor_StoresValue()
        {
            // Arrange
            ulong expected = 0x4154_1000_0000_0001;

            // Act
            var ftState = new FtState(expected);

            // Assert
            Assert.Equal(expected, ftState.Value);
        }

        #endregion

        #region Implicit Conversion Tests

        [Fact]
        public void FtState_ImplicitConversionFromUlong_Works()
        {
            // Arrange
            ulong value = 0x4154_0000_0000_0001;

            // Act
            FtState ftState = value;

            // Assert
            Assert.Equal(value, ftState.Value);
        }

        [Fact]
        public void FtState_ImplicitConversionToUlong_Works()
        {
            // Arrange
            var ftState = new FtState(0x4154_0000_0000_0001);

            // Act
            ulong value = ftState;

            // Assert
            Assert.Equal(0x4154_0000_0000_0001UL, value);
        }

        #endregion

        #region GetCountryCode Tests

        [Theory]
        [InlineData(0x4154_0000_0000_0000, "AT")] // Austria
        [InlineData(0x4445_0000_0000_0000, "DE")] // Germany
        [InlineData(0x4652_0000_0000_0000, "FR")] // France
        [InlineData(0x4954_0000_0000_0000, "IT")] // Italy
        public void FtState_GetCountryCode_DecodesCorrectly(ulong value, string expectedCountry)
        {
            // Arrange
            var ftState = new FtState(value);

            // Act
            var country = ftState.GetCountryCode();

            // Assert
            Assert.Equal(expectedCountry, country);
        }

        [Fact]
        public void FtState_GetCountryCode_IgnoresLowerBits()
        {
            // Arrange - AT with some flags set
            var ftState = new FtState(0x4154_FFFF_FFFF_FFFF);

            // Act
            var country = ftState.GetCountryCode();

            // Assert
            Assert.Equal("AT", country);
        }

        #endregion

        #region GetStatus Tests

        [Theory]
        [InlineData(0x0000_0000_0000_0000, 0x00000000)]  // No status flags
        [InlineData(0x0000_0000_0000_0001, 0x00000001)]  // Status flag 1
        [InlineData(0x0000_0000_0000_00FF, 0x000000FF)]  // Lower byte set
        [InlineData(0x0000_0000_FFFF_FFFF, unchecked((uint)0xFFFFFFFF))]  // All status bits set
        [InlineData(0x4154_F00F_1234_5678, 0x12345678)]  // AT country with version and SSCD, extract status
        public void FtState_GetStatus_ReturnsCorrectValue(ulong value, uint expectedStatus)
        {
            // Arrange
            var ftState = new FtState(value);

            // Act
            var result = ftState.GetGlobalFlags();

            // Assert
            Assert.Equal(expectedStatus, (uint) result);
        }

        [Fact]
        public void FtState_GetStatus_IgnoresUpperBits()
        {
            // Arrange - AT country code, version, SSCD status should all be ignored
            var ftState = new FtState(0x4154_F00F_0000_ABCD);

            // Act
            var result = ftState.GetGlobalFlags();

            // Assert
            Assert.Equal((uint) 0x0000ABCD, (uint) result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void FtState_Equals_ReturnsTrueForSameValue()
        {
            // Arrange
            var ftState1 = new FtState(0x4154_0000_0000_0001);
            var ftState2 = new FtState(0x4154_0000_0000_0001);

            // Act & Assert
            Assert.True(ftState1.Equals(ftState2));
            Assert.Equal(ftState1.GetHashCode(), ftState2.GetHashCode());
        }

        [Fact]
        public void FtState_Equals_ReturnsFalseForDifferentValue()
        {
            // Arrange
            var ftState1 = new FtState(0x4154_0000_0000_0001);
            var ftState2 = new FtState(0x4154_0000_0000_0002);

            // Act & Assert
            Assert.False(ftState1.Equals(ftState2));
        }

        [Fact]
        public void FtState_Equals_WorksWithUlong()
        {
            // Arrange
            var ftState = new FtState(0x4154_0000_0000_0001);

            // Act & Assert
            Assert.True(ftState.Equals(0x4154_0000_0000_0001UL));
            Assert.False(ftState.Equals(0x4154_0000_0000_0002UL));
        }

        #endregion

        #region JSON Serialization Tests

        [Fact]
        public void FtState_SerializesToJsonNumber()
        {
            // Arrange - use a simple value that's easy to verify
            var ftState = new FtState(12345);

            // Act
            var json = JsonSerializer.Serialize(ftState);

            // Assert - should be a number, not an object
            Assert.Equal("12345", json);
        }

        [Fact]
        public void FtState_DeserializesFromJsonNumber()
        {
            // Arrange
            var json = "12345";

            // Act
            var ftState = JsonSerializer.Deserialize<FtState>(json);

            // Assert
            Assert.NotNull(ftState);
            Assert.Equal(12345UL, ftState!.Value);
        }

        [Fact]
        public void FtState_RoundTrip_PreservesValue()
        {
            // Arrange
            var original = new FtState(0x4154_0000_0000_0001);

            // Act
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<FtState>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Value, deserialized!.Value);
        }

        [Fact]
        public void FtState_DeserializesAsPartOfObject()
        {
            // Arrange - use the actual hex value 0x4154_0000_0000_0001 for AT with flag 1
            // 0x4154_0000_0000_0001 in decimal = 4707387510509010945
            var json = """{"ftState":4707387510509010945}""";

            // Act
            var wrapper = JsonSerializer.Deserialize<FtStateWrapper>(json);

            // Assert
            Assert.NotNull(wrapper);
            Assert.NotNull(wrapper!.ftState);
            Assert.Equal(0x4154_0000_0000_0001UL, wrapper.ftState.Value);
            Assert.Equal("AT", wrapper.ftState.GetCountryCode());
        }

        private class FtStateWrapper
        {
            public FtState? ftState { get; set; }
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void FtState_ToString_IncludesCountryAndFlags()
        {
            // Arrange
            var ftState = new FtState(0x4154_0000_0000_0001);

            // Act
            var result = ftState.ToString();

            // Assert
            Assert.Contains("AT", result);
        }

        #endregion

        [Theory]
        [InlineData(0x4154_0000_0000_0000, 0)] // v0
        [InlineData(0x4154_1000_0000_0000, 1)] // v1
        [InlineData(0x4154_2000_0000_0000, 2)] // v2
        [InlineData(0x4154_2134_5678_9ABC, 2)] // v2
        public void FtState_GetVersion(ulong value, int expectedVersion)
        {
            // Arrange
            var ftState = new FtState(value);

            // Act
            var result = ftState.GetVersion();

            // Assert
            Assert.Equal(expectedVersion, result);
        }

        [Theory]
        [InlineData(0x4154_2000_0000_0000, 0)]  // No SSCD status
        [InlineData(0x4154_2001_0000_0000, 1)]  // SSCD status = 1
        [InlineData(0x4154_2002_0000_0000, 2)]  // SSCD status = 1
        [InlineData(0x4154_200F_0000_0000,  15)] // SSCD status = 15 (max); is currently an invalid valid value but checks the bit boundaries
        public void FtState_GetLocalSCUFlagsAT(ulong value, int expectedSSCDStatus)
        {
            // Arrange
            var ftState = new FtState(value);

            // Act
            var result = ftState.GetLocalSCUFlagsAT();

            // Assert
            Assert.Equal(expectedSSCDStatus, (int) result);
        }

        [Fact]
        public void FtState_GetLocalSCUFlags_AccessNotValidForCountry()
        {
            var ftState = new FtState(0x4154_2002_0000_0000UL); // AT country code
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsDE());
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsIT());  

            ftState = new FtState(0x4445_2002_0000_0000UL); // DE country code
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsAT());
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsIT());  

            ftState = new FtState(0x4954_2002_0000_0000UL); // IT country code
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsAT());
            Assert.Throws<InvalidOperationException>(() => ftState.GetLocalSCUFlagsDE());
        }
    }
}
