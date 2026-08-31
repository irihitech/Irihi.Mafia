using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Irihi.Lingua;

namespace Irihi.Mafia.Demo.Models;

public class ControlItem
{
    public string Name { get; }

    public IObservable<string?> Title { get; }

    public IObservable<string?> Description { get; }

    public ICommand Command { get; }

    private readonly string[] _searchable;

    public ControlItem(ILinguaManager manager, string name, string titleKey, string descriptionKey, ICommand command)
    {
        Name = name;
        Title = manager.GetObservable(titleKey)!;
        Description = manager.GetObservable(descriptionKey)!;
        Command = command;
        
        var searchable = new List<string> { name };
        if (Title is LinguaObservableString title)
            searchable.AddRange(manager.GetTranslations(title).Values);
        if (Description is LinguaObservableString description)
            searchable.AddRange(manager.GetTranslations(description).Values);
        
        _searchable = searchable.ToArray();
    }

    public bool Matches(string query)
    {
        query = query.Trim();
        return string.IsNullOrEmpty(query) ||
               _searchable.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase));
    }
}
