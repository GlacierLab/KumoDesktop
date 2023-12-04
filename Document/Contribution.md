# 为云酱添砖加瓦
云酱桌面端分为运行时和扩展包两部分。  
## 改进运行时
运行时包含了云酱的底层功能支撑，是支撑扩展包运行的关键。  
尽管你可以自行fork并维护分支，但是我们更希望你能通过PR将改进提交回主线。  
请根据改进需求的不同参阅说明：  
### 问题修复和优化
如果你遇到了Bug（功能没有按照预期执行），请先新开一个Issue简单说明。如果你有能力修复，再将修复作为PR提交并注明对应的Issue编号，在确认后我们会将其合并到主线。  
如果你遇到了体验或性能问题，你可以直接开一个PR，若你无法修复请开一个Issue。  
### 运行时功能添加
如果你在开发扩展包时发现云酱提供的运行时能力不能满足你的需求，我们建议新开一个Issue提出你的需求，在参考社区意见后再决定是否要添加一个功能到运行时能力，这将避免添加能力PR被拒绝的情况。  
所有添加到运行时的能力，必须是存在潜在广泛需求的，而不是为了某个特定扩展包（除了云酱核心功能包以外）而添加的。例如，`解压7Z格式`是一个可能被多个扩展包使用的潜在广泛需求，可以添加到运行时，而`解压一款游戏的资源包格式`是一个特定于某个扩展包的需求，不会添加到运行时，我们将在后续推出原生扩展包支持来解决此类需求。  
如果你只需要对运行时现有能力进行增强例如增加overload参数，你可以直接提交PR，但是请注意，我们不会接受对运行时现有能力进行破坏性修改的PR，云酱运行时所有功能都需要100%向前兼容（除了Bug等特殊情况）。  
### 国际化和本地化
现阶段，云酱只计划提供简体中文一种语言，我们的开发重心暂时放在功能上，待功能相对完善之后，可能会考虑添加繁体中文支持和英文支持。  
## 开发扩展包
每个扩展包都是一个独立的项目，任何人都可以根据文档自己开发任何功能的扩展包。云酱提供了自定义侧载扩展包的能力，你可以使用本地侧载或自建托管仓库的办法来分发你的扩展包。  
你可以自由选择是否开源你的扩展包，扩展包的开源协议不受云酱运行时协议所限制。  
### 提交到官方托管仓库
云酱提供了一个官方的托管仓库，你可以将你的扩展包提交到这个仓库，但提交到该仓库的扩展包有一定限制，我们会在审核通过后将其添加到仓库中，限制规则如下：  
1. 扩展包必须完全开源，开源协议不限，但我们推荐`MPL2.0`  
2. 扩展包必须是和游戏有关的，云酱是一个为游戏玩家而生的项目，我们不收录和游戏无关的扩展包，例如`手机数据备份`不会被收录  
3. 我们重视游戏开发者和其他创作者的利益，我们不收录任何可能侵犯游戏开发者和其他创作者利益的扩展包，例如`最新最热3A大作免费下载`不会被收录  
    3.1 我们对于可能侵犯开发者利益的定义是：提供仍可正常获取的游戏内容的资源。即当游戏仍然可以通过开发者认可的渠道获取全新副本且游戏所支持的设备尚未停产时，提供未经授权的游戏资源视为侵犯游戏开发者利益。例如，`战地5`正在通过EA官方销售，因此不可提供未经授权的游戏资源；`世嘉土星`是已经停产的主机，因此提供该主机上的游戏ROM不视为侵犯游戏开发者利益  
    3.2 模拟器软件本身不侵犯开发者利益，因此我们不会限制模拟器软件的扩展包。同理，破解主机是主机所有者的合法权利，因此我们不会限制此类扩展包  
4. 扩展包不得提供任何社交功能和账号系统，亦不可收集任何非匿名信息。云酱的定位是工具，我们不接受非工具向的扩展包。匿名数据例如使用统计和错误报告可以在征得用户同意后收集  
5. 扩展包不得包含任何广告，亦不得包含任何收费功能。云酱的定位是免费开源的工具，我们不接受任何收费向的扩展包  
6. 如果扩展包需要连接到非功能直接所需的服务器，服务端需要一并开源  
    6.1 功能所需的服务器的定义是：功能直接需要，且不归扩展包开发者所管理的服务器。例如，`Steam头像获取`可以连接到Steam API服务器  
7. 扩展包不应包含NSFW内容。我们理解确实有NSFW游戏也需要制作扩展包，我们后续会推出NSFW专用扩展包仓库  
8. 偶有一些特殊情况，我们可能会因为个人喜好或恩怨拒绝收录符合其他规则的扩展包（但我们不会通过任何不符合其他规则的扩展包），托管仓库只是云酱默认的扩展包检索来源，we can do whatever we want  