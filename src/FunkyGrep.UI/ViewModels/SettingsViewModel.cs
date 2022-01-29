using System.Collections.ObjectModel;
using FunkyGrep.UI.Validation.DataAnnotations;
using Prism.Validation;

namespace FunkyGrep.UI.ViewModels;

public class SettingsViewModel : ValidatableBindableBase
{
    ObservableCollection<EditorInfo> _editors = null!;
    int _selectedEditorIndex;

    public ObservableCollection<EditorInfo> Editors
    {
        get => this._editors;
        set => this.SetProperty(ref this._editors, value);
    }

    [ValidIndexInListMember(nameof(Editors))]
    public int DefaultEditorIndex
    {
        get => this._selectedEditorIndex;
        set => this.SetProperty(ref this._selectedEditorIndex, value);
    }

    public SettingsViewModel()
    {
        this.Editors = new ObservableCollection<EditorInfo>
        {
            EditorInfo.GetDefaultEditor()
        };
    }
}
