# DontDoAnything

共に暮らさないか、もう待機する必要はない

これを抑止したい

ので、解析してから簡単なら実装する

---

| ja | Book | id |
| --- | --- | --- |
| 共に暮らさないか | Lang | daInvite |
| もう待機する必要はない | Lang | enableMove |

## 共に暮らさないか

```csharp
//DramaCustomSequence.cs
Choice2("daInvite", "_invite"); 

Step("_invite");
```

## もう待機する必要はない

```csharp
//DramaCustomSequence.cs
Choice2("enableMove", "_enableMove");

Step("_enableMove");
```

## memo

* 割と地獄感ある
  * `Step`, `Method` がくっついてる？感じだけど実際の挙動は分からぬ
  * delegate だと名前無くてつらつら
* 気が向いたら・・・
