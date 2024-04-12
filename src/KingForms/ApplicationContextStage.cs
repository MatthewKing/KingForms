namespace KingForms;

public class ApplicationContextStage
{
    /// <summary>
    /// Raised when the stage has been completed (i.e. all forms have been closed).
    /// </summary>
    public event EventHandler Completed;

    /// <summary>
    /// Gets or sets the state object for the stage. This is used for passing data between stages.
    /// </summary>
    public object State { get; private set; }

    private readonly IList<Form> _forms = new List<Form>();

    public bool HasForms => _forms.Count > 0;

    public void AddForm(Form form)
    {
        AddForm(form, true);
    }

    public void AddForm(Form form, bool visible)
    {
        if (form is not null)
        {
            _forms.Add(form);

            if (!visible)
            {
                form.Opacity = 0;
            }

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

    public void SetCompletedState(object state) => State = state;
}
