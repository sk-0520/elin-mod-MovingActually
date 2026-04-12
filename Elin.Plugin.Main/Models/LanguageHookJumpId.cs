namespace Elin.Plugin.Main.Models
{
    public record class LanguageHookJumpId
    {
        public LanguageHookJumpId(string lang, string originJumpId, string hookJumpId)
        {
            Language = lang;
            OriginJumpId = originJumpId;
            HookJumpId = hookJumpId;
        }

        #region property

        public string Language { get; }
        public string OriginJumpId { get; }
        public string HookJumpId { get; }

        #endregion
    }
}
