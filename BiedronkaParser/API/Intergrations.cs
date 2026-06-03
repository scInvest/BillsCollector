using CostAnalizerApp.Interfaces;
using Integrations.Biedronka;
using Integrations.Biedronka.BiedronkaImport.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Integrations.API
{
    public class Intergrations
    {

        public static ISpendingCase ReadBiedronkaJson(string json)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var dto = JsonSerializer.Deserialize<ReceiptDto>(json, options);

            BiedronkaToBLConverter biedronkaToBLConverter = new BiedronkaToBLConverter();

            ISpendingCase receipt = biedronkaToBLConverter.ConvertToStandardFromat(dto);
            return receipt;
        }
    }
}
