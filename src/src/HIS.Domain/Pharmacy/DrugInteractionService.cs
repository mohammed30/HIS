using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace HIS.Pharmacy;

public class DrugInteractionService : DomainService
{
    // Hardcoded interaction list for demonstration
    // In production, this would query a real database or external API
    private readonly List<(string DrugA, string DrugB, string Severity, string Description)> _interactions = new()
    {
        ("Aspirin", "Warfarin", "High", "Increased risk of bleeding."),
        ("Simvastatin", "Amlodipine", "Moderate", "Increased risk of myopathy."),
        ("Lisinopril", "Potassium", "High", "Risk of hyperkalemia."),
        ("Paracetamol", "Alcohol", "High", "Increased risk of liver damage.")
    };

    public Task<List<string>> CheckInteractionsAsync(string newDrugName, List<string> activeDrugNames)
    {
        var warnings = new List<string>();

        foreach (var activeDrug in activeDrugNames)
        {
            var interaction = _interactions.Find(x => 
                (x.DrugA.Equals(newDrugName, StringComparison.OrdinalIgnoreCase) && x.DrugB.Equals(activeDrug, StringComparison.OrdinalIgnoreCase)) ||
                (x.DrugB.Equals(newDrugName, StringComparison.OrdinalIgnoreCase) && x.DrugA.Equals(activeDrug, StringComparison.OrdinalIgnoreCase))
            );

            if (interaction != default)
            {
                warnings.Add($"[{interaction.Severity}] {interaction.Description} ({newDrugName} + {activeDrug})");
            }
        }

        return Task.FromResult(warnings);
    }
}
