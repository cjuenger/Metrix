using System.ComponentModel;
using Io.Juenger.Scrum.GitLab.Contracts.Aggregates;

namespace Io.Juenger.Scrum.Metrix.WebUI;

public interface IContext
{
    event PropertyChangedEventHandler? PropertyChanged;
    IProductAggregate? SelectedProduct { get; set; }
    IList<IProductAggregate>? Products { get; set; }
}