namespace KingForms;

public static class Messages
{
    public static Lazy<int> RestoreInstance { get; } = new Lazy<int>(() => MessageHelper.GetMessageId("WM_KINGFORMS_RESTORE"), true);
}
