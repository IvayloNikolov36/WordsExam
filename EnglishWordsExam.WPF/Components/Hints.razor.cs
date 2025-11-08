using Microsoft.AspNetCore.Components;
using System;

namespace WpfBlazor.Components;

public partial class Hints
{
    [Parameter]
    public string[]? HintsTokens { get; set; }

    [Parameter]
    public EventCallback HintsNeeded { get; set; }

    private string? hintsText;
    private int hintsTextAreaRows = 1;
    private bool showHintsTextArea = false;

    protected override void OnParametersSet()
    {
        if (this.HintsTokens != null)
        {
            this.hintsText = string.Join(Environment.NewLine, this.HintsTokens);
            this.hintsTextAreaRows = this.HintsTokens.Length;
            this.showHintsTextArea = true;
        }
        else
        {
            this.showHintsTextArea = false;
        }
    }

    private async Task OnHintRequired()
    {
        await this.HintsNeeded.InvokeAsync();
    }
}
