using Elin.Plugin.Main.Models.Settings;

namespace Elin.Plugin.Main
{
    partial class Plugin
    {
        #region function

        /// <summary>
        /// 起動時のプラグイン独自処理。
        /// </summary>
        protected override void AwakePlugin()
        {
            Setting.Instance = Setting.Bind(Config, new Setting());
        }

        #endregion
    }
}
