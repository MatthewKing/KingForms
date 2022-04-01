namespace KingForms;

/// <summary>
/// Combo box extension methods.
/// </summary>
public static class ComboBoxExtensions
{
    /// <summary>
    /// Populates the combo box with the specified items.
    /// This sets the DataSource, ValueMember, and DisplayMember properties.
    /// It also retains any existing selection, as long as that selection is still valid.
    /// The <see cref="GetSelectedValue{T}(ComboBox, T)"/> method should be used to retreive the selected value.
    /// </summary>
    /// <param name="comboBox">The combo box.</param>
    /// <param name="items">The combo box items.</param>
    public static void SetDataSource(this ComboBox comboBox, params ComboBoxItem[] items)
    {
        SetDataSource(comboBox, (items?.ToArray() ?? Array.Empty<ComboBoxItem>()) as IEnumerable<ComboBoxItem>);
    }

    /// <summary>
    /// Populates the combo box with the specified items.
    /// This sets the DataSource, ValueMember, and DisplayMember properties.
    /// It also retains any existing selection, as long as that selection is still valid.
    /// The <see cref="GetSelectedValue{T}(ComboBox, T)"/> method should be used to retreive the selected value.
    /// </summary>
    /// <param name="comboBox">The combo box.</param>
    /// <param name="items">The combo box items.</param>
    public static void SetDataSource(this ComboBox comboBox, IEnumerable<ComboBoxItem> items)
    {
        // Track the previously selected item.
        var previouslySelectedItem = comboBox.SelectedItem as ComboBoxItem;

        // Snapshot the items enumerable as an array.
        var itemArray = items.ToArray();

        // Update the ValueMember and DisplayMember properties.
        comboBox.ValueMember = nameof(ComboBoxItem.Value);
        comboBox.DisplayMember = nameof(ComboBoxItem.DisplayText);

        // Bind the data source.
        comboBox.DataSource = itemArray;

        // If the previously selected item is also present in the new item list,
        // make sure we re-select it afterwards so that the user's selection is preserved.
        if (itemArray.Contains(previouslySelectedItem))
        {
            comboBox.SelectedItem = previouslySelectedItem;
        }
    }

    /// <summary>
    /// Returns the currently selected value from the specified combo box.
    /// </summary>
    /// <typeparam name="T">The type of the value to return.</typeparam>
    /// <param name="comboBox">The combo box.</param>
    /// <param name="defaultValue">The default value to return if no matching selection is found.</param>
    /// <returns>The currently selected value, or the specified default value if no matching selection is found.</returns>
    public static T GetSelectedValue<T>(this ComboBox comboBox, T defaultValue = default)
    {
        if (comboBox.SelectedValue is T value)
        {
            return value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Resets the currently selected item to the default value.
    /// </summary>
    /// <param name="comboBox">The combo box.</param>
    public static void ResetSelectedValue(this ComboBox comboBox)
    {
        comboBox.SelectedValue = ComboBoxItem.DefaultValuePlaceholder;
    }

    /// <summary>
    /// Sets the currently selected item to the specified value.
    /// </summary>
    /// <param name="comboBox">The combo box.</param>
    /// <param name="value">The value to set.</param>
    public static void SetSelectedValue(this ComboBox comboBox, object value)
    {
        comboBox.SelectedValue = value ?? ComboBoxItem.DefaultValuePlaceholder;
    }
}
