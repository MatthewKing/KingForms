namespace KingForms;

/// <summary>
/// Represents an item in a combo box.
/// </summary>
public sealed class ComboBoxItem : IEquatable<ComboBoxItem>
{
    /// <summary>
    /// Gets the placeholder value to be used in place of null values.
    /// </summary>
    internal static object DefaultValuePlaceholder { get; } = new object();

    /// <summary>
    /// Gets the item value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the item display text.
    /// </summary>
    public string DisplayText { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComboBoxItem"/> class.
    /// </summary>
    /// <param name="value">The item value.</param>
    /// <param name="displayText">The item display text.</param>
    public ComboBoxItem(object value, string displayText = null)
    {
        Value = value;
        DisplayText = displayText ?? value?.ToString();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as ComboBoxItem);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(ComboBoxItem other)
    {
        if (other is null)
        {
            return false;
        }

        return Equals(Value, other.Value);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Returns a combo box item representing the specified value.
    /// </summary>
    /// <param name="value">The value to represent.</param>
    /// <param name="displayText">The display text.</param>
    /// <returns>A combo box item.</returns>
    public static ComboBoxItem ForValue(object value, string displayText = null)
    {
        return new ComboBoxItem(value, displayText);
    }

    /// <summary>
    /// Returns a combo box item representing a default/null value.
    /// </summary>
    /// <param name="displayText">The display text.</param>
    /// <returns>A combo box item.</returns>
    public static ComboBoxItem Default(string displayText = null)
    {
        return new ComboBoxItem(DefaultValuePlaceholder, displayText);
    }
}
