﻿namespace AI.Sample.Providers.Commands;

public class ProvidersCommand
    : Command<ProvidersCommand> {
    public ProvidersCommand(IHasChildren parent)
        : base(parent, "Providers", []) {
        Description = "Manage AI Providers.";

        AddCommand<ProviderListCommand>();
        AddCommand<ProviderAddCommand>();
        AddCommand<ProviderUpdateCommand>();
        AddCommand<ProviderRemoveCommand>();
        AddCommand<HelpCommand>();
        AddCommand<ExitCommand>();
    }

    protected override Task<Result> ExecuteAsync(CancellationToken ct = default) {
        var choice = Input.BuildSelectionPrompt<string>("What would you like to do?")
                          .ConvertWith(MapTo)
                          .AddChoices("List",
                                      "Create",
                                      "Update",
                                      "Remove",
                                      "Help",
                                      "Back",
                                      "Exit").Show();

        var providerHandler = Application.Services.GetRequiredService<IProviderHandler>();
        var modelHandler = Application.Services.GetRequiredService<IModelHandler>();
        var command = choice switch {
            "List" => new ProviderListCommand(this, providerHandler),
            "Create" => new ProviderAddCommand(this, providerHandler),
            "Update" => new ProviderUpdateCommand(this, providerHandler),
            "Remove" => new ProviderRemoveCommand(this, providerHandler, modelHandler),
            "Help" => new HelpCommand(this),
            "Exit" => new ExitCommand(this),
            _ => (ICommand?)null,
        };
        return command?.Execute([], ct) ?? Result.SuccessTask();

        static string MapTo(string choice) => choice switch {
            "List" => "List Providers",
            "Create" => "Add a New Provider",
            "Update" => "Update a Provider",
            "Remove" => "Exclude a Provider",
            "Help" => "Help",
            "Back" => "Back",
            "Exit" => "Exit",
            _ => string.Empty,
        };
    }
}