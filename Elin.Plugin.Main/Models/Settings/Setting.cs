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
        public virtual bool MovingActuallyHoromeMommy { get; set; } = false;

        #endregion
    }
}
