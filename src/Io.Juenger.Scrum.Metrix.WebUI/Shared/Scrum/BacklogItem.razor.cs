using Io.Juenger.Scrum.GitLab.Contracts.Entities;
using Microsoft.AspNetCore.Components;

namespace Io.Juenger.Scrum.Metrix.WebUI.Shared.Scrum
{
    public partial class BacklogItem
    {
        [Parameter] 
        public ItemEntity Item { get; set; }
    }
}