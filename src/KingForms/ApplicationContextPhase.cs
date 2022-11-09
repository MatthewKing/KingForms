namespace KingForms;

public sealed class ApplicationContextPhase
{
    public event EventHandler Completed;

    private readonly IList<Form> _forms = new List<Form>();

    public void AddForm(Form form)
    {
        AddForm(form, true);
    }

    public void AddForm(Form form, bool visible)
    {
        if (form is not null)
        {
            if (!visible)
            {
                form.Opacity = 0;
            }

            _forms.Add(form);
            form.HandleDestroyed += OnFormDestroy;
            form.Visible = true;

            if (!visible)
            {
                form.Visible = false;
                form.Opacity = 1;
            }
        }
    }

    private void OnFormDestroy(object sender, EventArgs e)
    {
        if (sender is Form form)
        {
            if (!form.RecreatingHandle)
            {
                form.HandleDestroyed -= OnFormDestroy;

                _forms.Remove(form);
                if (_forms.Count == 0)
                {
                    Completed?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
