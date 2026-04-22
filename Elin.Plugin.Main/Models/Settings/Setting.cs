using Elin.Plugin.Generated;

namespace Elin.Plugin.Main.Models.Settings
{
    [GeneratePluginConfig]
    public partial class Setting
    {
        #region property

        public static Setting Instance { get; set; } = new Setting();

        /// <summary>
        /// ホロメ信仰時のママーを「実は…」に引っ越すか。
        /// </summary>
        [GeneratePluginConfigDescription(nameof(PluginLocalizationConfig.MoveHoromeMommy), AllLanguage = true)]
        public virtual bool MoveHoromeMommy { get; set; } = false;

        #endregion
    }
}
