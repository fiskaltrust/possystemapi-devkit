using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// Decoder for the ftState field returned in receipt responses.
    /// The ftState field contains encoded information about the state of the fiskaltrust.Middleware.
    /// </summary>
    /// <remarks>
    /// Format: _CCCC_vlll_gggg_gggg_
    /// - CCCC: Country code (2 characters, upper 16 bits)
    /// - v: Version (4 bits)
    /// - lll: local flags (country specific, 12 bits
    /// - gggggggg: global flags (32 bits)
    /// </remarks>
    [JsonConverter(typeof(FtStateJsonConverter))]
    public class FtState
    {
        /// <summary>
        /// The raw ulong value of the ftState field.
        /// </summary>
        public ulong Value { get; }

        /// <summary>
        /// Creates a new FtStateDecoder from a raw ulong value.
        /// </summary>
        public FtState(ulong value)
        {
            Value = value;
        }

        /// <summary>
        /// Implicit conversion from ulong to FtState.
        /// </summary>
        public static implicit operator FtState(ulong value) => new FtState(value);

        /// <summary>
        /// Implicit conversion from FtState to ulong.
        /// </summary>
        public static implicit operator ulong(FtState decoder) => decoder.Value;

        /// <summary>
        /// Gets the country code from the upper 16 bits (same encoding as ReceiptCase).
        /// </summary>
        public string GetCountryCode()
        {
            ulong countryPart = (Value >> 48) & 0xFFFF;
            char first = (char)((countryPart >> 8) & 0xFF);
            char second = (char)(countryPart & 0xFF);
            return $"{first}{second}";
        }

        /// <summary>
        /// Returns the used version. Should be 2 for the POS System API.
        /// </summary>
        public int GetVersion()
        {
            return (int)((Value & 0x0000_F000_0000_0000) >> 44);
        }

#region Local Flags
        public ulong GetLocalFlags()
        {
            return ((Value & 0x0000_0FFF_0000_0000) >> 32);
        }

        //////////////////////////////////////////////////////////////////////////
        /// Local SCU Flags

        public uint GetLocalSCUFlags()
        {
            return (uint)(GetLocalFlags() & 0x00F);
        }
        public FtStateLocalSCUFlagsAT GetLocalSCUFlagsAT()
        {
            if (GetCountryCode() != "AT")
                throw new InvalidOperationException("FtStateLocalSCUFlagsAT is only valid for country code 'AT'");
            return (FtStateLocalSCUFlagsAT)GetLocalSCUFlags();
        }
        public FtStateLocalSCUFlagsDE GetLocalSCUFlagsDE()
        {
            if (GetCountryCode() != "DE")
                throw new InvalidOperationException("FtStateLocalSCUFlagsDE is only valid for country code 'DE'");
            return (FtStateLocalSCUFlagsDE)GetLocalSCUFlags();
        }
        public FtStateLocalSCUFlagsIT GetLocalSCUFlagsIT()
        {
            if (GetCountryCode() != "IT")
                throw new InvalidOperationException("FtStateLocalSCUFlagsIT is only valid for country code 'IT'");
            return (FtStateLocalSCUFlagsIT)(GetLocalSCUFlags());
        }
        //////////////////////////////////////////////////////////////////////////

#endregion

        public FtStateStatus GetGlobalFlags()
        {
            return (FtStateStatus)(Value & 0x0000_0000_FFFF_FFFF);
        }

        public override string ToString() => $"{GetCountryCode()} - Version: {GetVersion()} - LocalFlags: {GetLocalFlags()} - GlobalFlags: {GetGlobalFlags()}";

        public override bool Equals(object? obj)
        {
            if (obj is FtState other)
                return Value == other.Value;
            if (obj is ulong ulongValue)
                return Value == ulongValue;
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }

    public enum FtStateStatus : uint
    {
        Ready = 0x0000_0000,
        /// <summary>
        /// Security Mechanism is out Out of Operation. Queue is not started or already stopped.
        /// </summary>
        QueOutOfOperation = 0x0000_0001,
        /// <summary>
        /// SCU (Signature Creation Unit) temporary out of service.
        /// For at least one receipt, it was not possible to receive the sig-nature from an allocated SCU,
        /// therefore the security mecha-nism has been put into "signature creation device out of ser-vice" mode.
        /// Regardless of whether an allocated SCU is availa-ble again or not,
        /// the mode remains in place until a ZeroReceipt cleans up the state and takes the market specific action re-quired.
        /// </summary>
        SCUTemporaryOutOfService = 0x0000_0002,
        /// <summary>
        /// Deferred Queue Mode / Late Signing Mode is active.
        /// When the cash register doesn’t reach the queue, it queues up the receipt requests while continuing to do business.
        /// Also, with a major failure of the cash register or a power outage, handwritten paper receipts are queued up while continuing to do business.
        /// After getting back to a full functional state, these queued-up ReceiptRequests are sent to the queue,
        /// having the original cbReceiptMoment of the business case and Re-ceiptCase tagged/flagged with 0001 (Deferred Queue / Late Signing) or 0008 (Handwritten).
        /// A result of this is a marker within the ftState, which can be re-solved via ZeroReceipt.
        /// The reason for the marker is a mis-match between processed time along the receipt chain and a manual event to clean up the state and maybe notify 3rd par-ties of an outage.
        /// </summary>
        LateSigningModeActive = 0x0000_0008,
        /// <summary>
        /// Message Pending Middleware/Queue is a headless background service, but there are situations where communication with the cash-ier/operator or the cash register is necessary.
        /// For example, if the last daily closing was missed or if a special condition re-lated to the signature creation unit or service happened.
        /// This is the moment when the message pending flag is set by the mid-dleware, which should be signalled to the cashier by the POS system.
        /// By executing a ZeroReceipt, the cashier can read the message or instr
        /// Related to local regulations, this receipt may be stored/ar-chived with/for bookkeeping purposes; if this is the case, this is also visualized.
        /// </summary>
        NotificationPending = 0x0000_0040,
        /// <summary>
        /// DailyClosing due When the first cbReceiptMoment used since the last DailyClosing and the current/latest cbReceiptMoment in the ReceiptRequest have a date-gap of more than two days
        /// (for example, the first since the last daily closing is 24/08 and the current is 26/08), then this state indicates, a Daily Closing should be done.
        /// DailyClosing is an essential part of the security mechanism and also executes additional market-specific cleanup tasks.
        /// Therefore, each queue should do a DailyClosing to clear persistent changes in business data and also changes in the business period.
        /// </summary>
        DailyClosingDue = 0x0000_0100,
        /// <summary>
        /// MonthlyClosing due
        /// When the first cbReceiptMoment used since last MonthlyClosing and the current/latest cbReceiptMoment in the ReceiptRequest are different, then this state indicates, a MonthlyClosing should be done
        /// </summary>
        MonthlyClosingDue = 0x0000_0200,
        /// <summary>
        /// Something went wrong while processing the last request. QueueItem exists but didn’t reach the state of a ReceiptItem and didn’t consume a ftReceiptNumber within the chain.
        /// Error reason is shown within the responded ftSignatureItems. This happens, for example, if the ReceiptCase is not recognized or is wrong.
        /// </summary>
        Error = 0xEEEE_EEEE,
        /// <summary>
        /// Something went wrong while processing the last request, and nothing persisted within the Queue. Fail reason is shown within the responded ftSignatureItems.
        /// This happens, for example, when the flag ReceiptRequest is used after a communication outage, and no properly processed item is found.
        /// </summary>
        Fail = 0xFFFF_FFFF,
    }

    public enum FtStateLocalSCUFlagsAT : uint
    {
        SSUOK = 0,
        /// <summary>
        /// SCU permanent out of service. 48h FinanzOnline timeout reached.
        /// </summary>
        SSUPermanentlyOutOfService = 1,
        BackupSCUInUse = 2,
    }

    public enum FtStateLocalSCUFlagsDE : uint
    {
        SCUDOK = 0,
        /// <summary>
        /// SCU in switching state The queue is in the process of switching SCUs.
        /// This state is returned in case any receipts are processed between the initialize-switch receipt and the finish-switch receipt.
        /// These receipts are protected by the fiskaltrust.SecurityMechanism, but not sent to any TSE, as no SCU is con-nected at this point.
        /// </summary>
        SCUInSwitchingState = 1,
    }

    public enum FtStateLocalSCUFlagsIT : uint
    {
        SCUOK = 0,
        /// <summary>
        /// [RT-Printer|RT-Server|Government Service] not reachable Responded in case of a zero-receipt and other hard dependencies to the service.
        /// </summary>
        RTNotReachable = 1,
    }

    /// <summary>
    /// JSON converter that serializes FtState as a JSON number (ulong).
    /// </summary>
    public class FtStateJsonConverter : JsonConverter<FtState>
    {
        public override FtState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return new FtState(reader.GetUInt64());
            }
            throw new JsonException($"Expected a number for FtState, got {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, FtState value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}
