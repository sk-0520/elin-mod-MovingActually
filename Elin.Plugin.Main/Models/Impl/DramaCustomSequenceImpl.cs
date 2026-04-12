using Elin.Plugin.Generated;
using Elin.Plugin.Main.PluginHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Elin.Plugin.Main.Models.Impl
{

    public static class DramaCustomSequenceImpl
    {
        #region define

        private static class LanguageId
        {
            /// <summary>
            /// もう待機する必要はない
            /// </summary>
            /// <remarks>Lang!General: enableMove</remarks>
            public const string EnableMove = "enableMove";

            /// <summary>
            /// 仲間に誘う
            /// </summary>
            /// <remarks>Lang!General: daJoinParty</remarks>
            public const string JoinParty = "daJoinParty";
            /// <summary>
            /// ホームで待機しろ
            /// </summary>
            /// <remarks>Lang!General: daLeaveParty</remarks>
            public const string LeaveParty = "daLeaveParty";
            /// <summary>
            /// この土地に移住して欲しい
            /// </summary>
            /// <remarks>Lang!General: daMakeHome</remarks>
            public const string MakeHome = "daMakeHome";
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
            /// 仲間に誘う のジャンプID。
            /// </summary>
            public const string JoinParty = "_joinParty";
            /// <summary>
            /// <see cref="JoinParty"/> に対して差し込むMod用のジャンプID。
            /// </summary>
            /// <remarks>このID自体は Mod 内で使用を完結させ、表示用に Elin を経由することにはなるが最終的には <see cref="JoinParty"/> を指すようにすること。</remarks>
            public const string HookJoinParty = JoinParty + "@" + Package.Id;

            /// <summary>
            /// ホームで待機しろ のジャンプID。
            /// </summary>
            public const string LeaveParty = "_leaveParty";
            /// <summary>
            /// <see cref="LeaveParty"/> に対して差し込むMod用のジャンプID。
            /// </summary>
            /// <remarks>このID自体は Mod 内で使用を完結させ、表示用に Elin を経由することにはなるが最終的には <see cref="LeaveParty"/> を指すようにすること。</remarks>
            public const string HookLeaveParty = LeaveParty + "@" + Package.Id;

            /// <summary>
            /// この土地に移住して欲しい のジャンプID。
            /// </summary>
            public const string MakeHome = "_makeHome";
            /// <summary>
            /// <see cref="MakeHome"/> に対して差し込むMod用のジャンプID。
            /// </summary>
            /// <remarks>このID自体は Mod 内で使用を完結させ、表示用に Elin を経由することにはなるが最終的には <see cref="MakeHome"/> を指すようにすること。</remarks>
            public const string HookMakeHome = MakeHome + "@" + Package.Id;

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
            Start,
        }

        #endregion

        #region property

        private static Chara? BuildArgumentCharacter { get; set; }
        private static OtherSequence CurrentOtherSequence { get; set; } = OtherSequence.None;

        private static IReadOnlyDictionary<string, LanguageHookJumpId> LanguageHookJumpIdMap { get; } = new[]
        {
            new LanguageHookJumpId(LanguageId.EnableMove, JumpId.EnableMove, JumpId.HookEnableMove),
            new LanguageHookJumpId(LanguageId.MakeHome, JumpId.MakeHome, JumpId.HookMakeHome),
            new LanguageHookJumpId(LanguageId.JoinParty, JumpId.JoinParty, JumpId.HookJoinParty),
        }.ToDictionary(k => k.Language, v => v)
        ;

        #endregion

        #region function

        private static bool CanEnableMove(Chara c)
        {
            // [ELIN:DramaCustomSequence.Build]
            // -> if (c.IsPCParty) ... else if (!c.noMove)
            var result =
                !c.IsPCParty
                &&
                !c.noMove
            ;
            return result;
        }

        private static bool CanJoinParty(Chara c)
        {
            // [ELIN:DramaCustomSequence.Build]
            // -> if (c.IsHomeMember())
            // -> -> if (!c.IsPCParty && c.memberType != FactionMemberType.Livestock && c.trait.CanJoinParty)
            var result =
                c.IsHomeMember()
                &&
                !c.IsPCParty && c.memberType != FactionMemberType.Livestock && c.trait.CanJoinParty
            ;
            return result;
        }

        private static bool CanLeaveParty(Chara c)
        {
            // [ELIN:DramaCustomSequence.Build]
            // -> if (c.IsPCParty && !c.isSummon)
            // -> -> if (c.host == null && c.homeZone != null)
            var result =
                (c.IsPCParty && !c.isSummon)
                &&
                (c.host == null && c.homeZone != null)
            ;
            return result;
        }

        private static bool CanMakeHome(Chara c)
        {
            // [ELIN:DramaCustomSequence.Build]
            // -> if (c.IsPCParty && !c.isSummon)
            // -> -> if (EClass._zone.IsPCFaction && c.homeBranch != EClass._zone.branch)
            var result =
                (c.IsPCParty && !c.isSummon)
                &&
                (EClass._zone.IsPCFaction && c.homeBranch != EClass._zone.branch)
            ;
            return result;
        }

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

            if (LanguageHookJumpIdMap.TryGetValue(lang, out var languageHookJumpId))
            {
                // フックしたジャンプIDが指定されている場合は Elin 側の正式なジャンプIDに差し替え
                if (idJump == languageHookJumpId.HookJumpId)
                {
                    idJump = languageHookJumpId.OriginJumpId;
                    ModHelper.LogDev($"[hook] {nameof(idJump)}: {languageHookJumpId.HookJumpId} -> {idJump}");
                    return true;
                }

                ModHelper.LogDev($"[ignore] {lang}, {idJump}");
                return false;
            }

            // ホームで待機しろ、はすでに変換済みの言語が渡されるのでジャンプIDで判断する
            if (idJump == JumpId.LeaveParty)
            {
                ModHelper.LogDev($"[ignore] {lang}, {idJump}");
                return false;
            }
            if (idJump == JumpId.HookLeaveParty)
            {
                idJump = JumpId.LeaveParty;
                ModHelper.LogDev($"[hook] {nameof(idJump)}: {JumpId.HookLeaveParty} -> {idJump}");
                return true;
            }

            return true;
        }

        public static bool ChoicePrefix(DramaCustomSequence instance, string lang, string idJump, bool cancel)
        {
            var c = BuildArgumentCharacter;
            if (c == null)
            {
                ModHelper.LogNotExpected($"{nameof(BuildArgumentCharacter)} is null");
                return true;
            }

            ModHelper.LogDev($"ChoicePrefix: lang={lang}, idJump={idJump}, cancel={cancel}, c={c.Name}");

            // 添い寝の選択肢の直前に、移動許可の選択肢を差し込む
            // ここで待機してほしい Choice("disableMove", "_disableMove"); の選択肢と位置を合わせるための無理やり感
            if (idJump == JumpId.SleepBeside)
            {
                if (CanEnableMove(c))
                {
                    ModHelper.LogDev("[add] LanguageId.EnableMove, JumpId.HookEnableMove");
                    instance.Choice2(LanguageId.EnableMove, JumpId.HookEnableMove);
                }
            }

            // 選択肢最上部にメッセージを差し込む
            // Talk は内部メソッドなので正攻法でパッチあてられず、_Talk の言語は変換済みなのでここで無理やり差し込む
            if (CurrentOtherSequence == OtherSequence.Start)
            {
                // 実は… からの選択肢構築の開始時点で実施する
                // これは 
                // 1. Step("_factionOther");
                // 2. Talk("what", StepDefault);
                // の順序で呼ばれることに完全に依存している

                try
                {
                    if (CanJoinParty(c))
                    {
                        // 仲間に誘う
                        ModHelper.LogDev("[add] LanguageId.JoinParty, JumpId.HookJoinParty");
                        instance.Choice2(LanguageId.JoinParty, JumpId.HookJoinParty);
                    }
                    else // 特に Elin 側で分けてはい無さそうな気はするが一応 else で条件追加しておく
                    {
                        if (CanMakeHome(c))
                        {
                            // この土地に移住して欲しい
                            ModHelper.LogDev("[add] LanguageId.LeaveParty, JumpId.HookLeaveParty");
                            instance.Choice2(LanguageId.MakeHome, JumpId.HookMakeHome);
                        }
                        if (CanLeaveParty(c))
                        {
                            // ホームで待機しろ
                            ModHelper.LogDev("[add] LanguageId.LeaveParty, JumpId.HookLeaveParty");
                            instance.Choice2(LanguageId.LeaveParty.lang(c.homeZone.Name), JumpId.HookLeaveParty);
                        }
                    }
                }
                finally
                {
                    CurrentOtherSequence = OtherSequence.None;
                }
            }

            return true;
        }

        public static void StepPostfix(DramaCustomSequence instance, string step)
        {
            CurrentOtherSequence = step == JumpId.FactionOther
                ? OtherSequence.Start
                : OtherSequence.None
            ;
            ModHelper.LogDev($"StepPostfix: step={step}, CurrentOtherSequence={CurrentOtherSequence}");
        }


        #endregion
    }
}
