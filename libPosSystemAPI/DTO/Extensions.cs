using System;
using System.Net.Http.Json;
using System.Text.Json;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public static class Extensions
    {
        public static PayItemCaseData? GetPayItemCaseData(this PayItem pi)
        {
            if (pi == null || pi.ftPayItemCaseData == null) return null;
            var ftPayItemCaseData = pi.ftPayItemCaseData;
            if (ftPayItemCaseData is PayItemCaseData picd) return picd;

            if (ftPayItemCaseData is JsonElement je)
            {
                try
                {
                    if (je.ValueKind == JsonValueKind.String)
                    {
                        string? strValue = je.GetString();
                        if (string.IsNullOrEmpty(strValue))
                        {
                            Console.WriteLine("ERROR ftPayItemCaseData string is null or empty");
                            return null;
                        }
                        return JsonSerializer.Deserialize<PayItemCaseData>(strValue);
                    }
                    else if (je.ValueKind == JsonValueKind.Object)
                    {
                        return je.Deserialize<PayItemCaseData>();
                    }
                    else
                    {
                        Console.WriteLine("ERROR ftPayItemCaseData JsonElement is of unexpected ValueKind: " + je.ValueKind);
                        Console.WriteLine("ERROR    data=" + je);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR deserializing ftPayItemCaseData to PayItemCaseData: {ex}");
                    Console.WriteLine("ERROR    data =" + je);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("ERROR ftPayItemCaseData is of unexpected type: " + ftPayItemCaseData.GetType());
            }
            return null;
        }
    }
}
