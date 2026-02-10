namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    // from fiskaltrust.ifPOS.v2.Cases.ReceiptCase
    public enum ReceiptCase : ulong
    {
        UnknownReceipt0x0000 = 0uL,
        PointOfSaleReceipt0x0001 = 1uL,
        PaymentTransfer0x0002 = 2uL,
        PointOfSaleReceiptWithoutObligation0x0003 = 3uL,
        ECommerce0x0004 = 4uL,
        DeliveryNote0x0005 = 5uL,
        InvoiceUnknown0x1000 = 4096uL,
        InvoiceB2C0x1001 = 4097uL,
        InvoiceB2B0x1002 = 4098uL,
        InvoiceB2G0x1003 = 4099uL,
        ZeroReceipt0x2000 = 8192uL,
        OneReceipt0x2001 = 8193uL,
        ShiftClosing0x2010 = 8208uL,
        DailyClosing0x2011 = 8209uL,
        MonthlyClosing0x2012 = 8210uL,
        YearlyClosing0x2013 = 8211uL,
        ProtocolUnspecified0x3000 = 12288uL,
        ProtocolTechnicalEvent0x3001 = 12289uL,
        ProtocolAccountingEvent0x3002 = 12290uL,
        InternalUsageMaterialConsumption0x3003 = 12291uL,
        Order0x3004 = 12292uL,
        Pay0x3005 = 12293uL,
        CopyReceiptPrintExistingReceipt0x3010 = 12304uL,
        ArchiveReceiptx3011 = 12305uL,
        InitialOperationReceipt0x4001 = 16385uL,
        OutOfOperationReceipt0x4002 = 16386uL,
        InitSCUSwitch0x4011 = 16401uL,
        FinishSCUSwitch0x4012 = 16402uL,
        StartMigrationLifecycle0x4021 = 16417uL,
        StopMigrationLifecycle0x4022 = 16418uL
    }
}