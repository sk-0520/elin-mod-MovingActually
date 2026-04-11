using Elin.Plugin.Generated;
using Elin.Plugin.Main.PluginHelpers;

namespace Elin.Plugin.Main.Models.Impl
{

    public static class DramaCustomSequenceImpl
    {
        #region define

        private static class LanguageId
        {
            /// <summary>
            /// 共に暮らさないか
            /// </summary>
            /// <remarks>Lang!General: daInvite</remarks>
            public const string InviteHome = "daInvite";

            /// <summary>
            /// もう待機する必要はない
            /// </summary>
            /// <remarks>Lang!General: enableMove</remarks>
            public const string EnableMove = "enableMove";

            /// <summary>
            /// なんだ？ 的なの。
            /// </summary>
            /// <remarks>
            /// <para>どこで定義されてるんだこれ。</para>
            /// <para>とりあえずこのクラスに突っ込んでおく。</para>
            /// </remarks>
            public const string What = "what";
        }

        private static class JumpId
        {
            /// <summary>
            /// 実は… のジャンプID。
            /// </summary>
            public const string FactionOther = "_factionOther";

            /// <summary>
            /// 添い寝して欲しい/添い寝をやめたい のジャンプID。
            /// </summary>
            public const string SleepBeside = "_sleepBeside";

            /// <summary>
            /// もう待機する必要はない のジャンプID。
            /// </summary>
            public const string EnableMove = "_enableMove";
            /// <summary>
            /// <see cref="EnableMove"/> に対して差し込むMod用のジャンプID。
            /// </summary>
            /// <remarks>このID自体は Mod 内で使用を完結させ、表示用に Elin を経由することにはなるが最終的には <see cref="EnableMove"/> を指すようにすること。</remarks>
            public const string HookEnableMove = EnableMove + "@" + Package.Id;

            /// <summary>
            /// 共に暮らさないか のジャンプID。
            /// </summary>
            public const string InviteHome = "_invite";
            /// <summary>
            /// <see cref="InviteHome"/> に対して差し込むMod用のジャンプID。
            /// </summary>
            /// <remarks>このID自体は Mod 内で使用を完結させ、表示用に Elin を経由することにはなるが最終的には <see cref="InviteHome"/> を指すようにすること。</remarks>
            public const string HookInviteHome = InviteHome + "@" + Package.Id;

        }

        /// <summary>
        /// what が他の場面でも使用される可能性があるので、実は… の選択肢の状態を管理するための列挙体。
        /// </summary>
        /// <remarks>自分でも何かいてるのかわからんなこれ。</remarks>
        private enum OtherSequence
        {
            /// <summary>
            /// 何もしなくていい。
            /// </summary>
            None,
            /// <summary>
            /// 実は… が選ばれた状態。
            /// </summary>
            Prepare,
        }

        #endregion

        #region property

        private static Chara? BuildArgumentCharacter { get; set; }
        private static OtherSequence CurrentOtherSequence { get; set; } = OtherSequence.None;

        #endregion

        #region function

        #endregion

        #region DramaCustomSequence

        internal static bool BuildPrefix(DramaCustomSequence instance, Chara c)
        {
            BuildArgumentCharacter = c;
            return true;
        }

        internal static void BuildPostfix(DramaCustomSequence instance, Chara c)
        {
            BuildArgumentCharacter = null;
        }

        public static bool Choice2Prefix(DramaCustomSequence instance, string lang, ref string idJump)
        {
            // 特定のセリフを選択肢に表示させないようにしたりジャンプ先加工したり、忙しい子
            ModHelper.LogDev($"Choice2Prefix: lang={lang}, idJump={idJump}");

            if (lang == LanguageId.InviteHome)
            {
                // フックした「共に暮らさないか」が指定されている場合は Elin 側の正式なジャンプIDに差し替え
                if (idJump == JumpId.HookInviteHome)
                {
                    idJump = JumpId.InviteHome;
                    ModHelper.LogDev($"[hook] {nameof(idJump)}: {JumpId.HookInviteHome} -> {idJump}");
                    return true;
                }

                ModHelper.LogDev($"[ignore] {lang}, {idJump}");
                return false;
            }

            if (lang == LanguageId.EnableMove)
            {
                // フックした「もう待機する必要はない」が指定されている場合は Elin 側の正式なジャンプIDに差し替え
                if (idJump == JumpId.HookEnableMove)
                {
                    idJump = JumpId.EnableMove;
                    ModHelper.LogDev($"[hook] {nameof(idJump)}: {JumpId.HookEnableMove} -> {idJump}");
                    return true;
                }

                ModHelper.LogDev($"[ignore] {lang}, {idJump}");
                return false;
            }

            return true;
        }

        public static bool ChoicePrefix(DramaCustomSequence instance, string lang, string idJump, bool cancel)
        {
            var currentCharacter = BuildArgumentCharacter;
            if (currentCharacter == null)
            {
                ModHelper.LogNotExpected($"{nameof(BuildArgumentCharacter)} is null");
                return true;
            }

            ModHelper.LogDev($"ChoicePrefix: lang={lang}, idJump={idJump}, cancel={cancel}, currentCharacter={currentCharacter.Name}");

            // 添い寝の選択肢の直前に、移動許可の選択肢を差し込む
            // ここで待機してほしい Choice("disableMove", "_disableMove"); の選択肢と位置を合わせるための無理やり感
            if (idJump == JumpId.SleepBeside)
            {
                // [ELIN:DramaCustomSequence.Build]
                // -> if (c.IsPCParty) ... else if (!c.noMove)
                if (!currentCharacter.IsPCParty)
                {
                    if (currentCharacter.noMove)
                    {
                        ModHelper.LogDev("[add] LanguageId.EnableMove, JumpId.HookEnableMove");
                        instance.Choice2(LanguageId.EnableMove, JumpId.HookEnableMove);
                    }
                }
            }

            return true;
        }

        public static void StepPostfix(DramaCustomSequence instance, string step)
        {
            CurrentOtherSequence = step == JumpId.FactionOther
                ? OtherSequence.Prepare
                : OtherSequence.None
            ;
        }

        public static void TalkPostfix(DramaCustomSequence instance, string idTalk, string idJump)
        {
            if (CurrentOtherSequence == OtherSequence.Prepare && idTalk == LanguageId.What)
            {
                var currentCharacter = BuildArgumentCharacter;
                if (currentCharacter == null)
                {
                    ModHelper.LogNotExpected($"{nameof(BuildArgumentCharacter)} is null");
                    return;
                }

                // 選択肢最上部に勧誘メッセージを差し込む
                // 実は… から表示される項目は色々条件があるので待機用の適当差し込みだと怪しいので
                // 実は… からの選択肢構築の開始時点で実施する

                // [Elin]
                // -> if (!c.IsPCFaction && c.affinity.CanInvite() && !EClass._zone.IsInstance && c.c_bossType == BossType.none)
                // if ((c.trait.IsUnique || c.IsGlobal) && c.GetInt(111) == 0 && !c.IsPCFaction) ... else


                try
                {
                    instance.Choice2(LanguageId.InviteHome, JumpId.HookInviteHome);
                }
                finally
                {
                    CurrentOtherSequence = OtherSequence.None;
                }
            }
        }


        #endregion
    }
}
