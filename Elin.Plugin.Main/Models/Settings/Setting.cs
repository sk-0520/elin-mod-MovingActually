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
        [GeneratePluginConfigDescription(nameof(PluginLocalizationConfig.MoveHoromeMommy))]
        public virtual bool MoveHoromeMommy { get; set; } = false;

        [GeneratePluginConfigDescription(nameof(PluginLocalizationGeneral.Abc), PluginConfigDescriptionTarget.General)]
        public virtual bool Abc { get; set; } = false;

        [GeneratePluginConfigDescription(nameof(PluginLocalizationConfig.Xyz), PluginConfigDescriptionTarget.Config)]
        public virtual bool Xyz { get; set; } = false;

        public virtual bool None { get; set; } = false;

        #endregion
    }
}
