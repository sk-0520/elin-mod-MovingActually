# DontDoAnything

共に暮らさないか、もう待機する必要はない

これを抑止したい

ので、解析してから簡単なら実装する

---

| ja | Book | Sheet | id |
| --- | --- | --- | --- |
| ~~共に暮らさないか~~ | Lang | General | daInvite |
| もう待機する必要はない | General | Lang | enableMove |
| 実は… | Lang | General | daFactionOther |
| ここで待機して欲しい | Lang | General | disableMove |
| 仲間に誘う | Lang | General | daJoinParty |
| ホームで待機しろ (#1) | Lang | General | daLeaveParty |
| この土地に移住して欲しい | Lang | General | daMakeHome |

## ~~共に暮らさないか~~

```csharp
//DramaCustomSequence.cs
Choice2("daInvite", "_invite"); 

Step("_invite");
```

ともくらは無理。実は…の道は険しい。

設定でこれを抑制することは可能だが、この Mod では設定を作りたくない

## もう待機する必要はない

```csharp
//DramaCustomSequence.cs
Choice2("enableMove", "_enableMove");

Step("_enableMove");
```

## 実は…

```csharp
//DramaCustomSequence.cs
Choice2("daFactionOther", "_factionOther");

Step("_factionOther");
```

### ここで待機して欲しい

```csharp
//DramaCustomSequence.cs
else if (!c.noMove)
{
 Choice("disableMove", "_disableMove");
}

Step("_disableMove");
```

## memo

* 割と地獄感ある
  * `Step`, `Method` がくっついてる？感じだけど実際の挙動は分からぬ
  * delegate だと名前無くてつらつら
* 気が向いたら・・・
* 実は・・・
  * `Build` なんだ、これは、なんだ
