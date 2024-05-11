# UPMKits

项目地址：[https://github.com/Chino66/UPM\_Kits\_Develop](https://github.com/Chino66/UPM_Kits_Develop)

UPMKits是一个基于Github托管packag的发布和管理工具。

## 关于UPM

Unity的package（简称upm）其实是[npm](https://www.npmjs.com/)（存在一些差异），所以关于upm的发布，管理等很大程度上合npm是一样的。

## UPM包的来源

打开PackagerManager可以看到4种添加包的方式，当然也可以直接在manifest.json中直接添加包，这4种方式表述的是包的4种来源。

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/0a9c8432-df44-4560-a80f-599602d7b0b0.png)

**from disk**：来源是本地磁盘，它的好处是可以直接对包进行开发和修改，通常是包的开发者在生产阶段使用的方式。

**from tarball**：也是本地的，不过是以压缩包的形式。

**from git URL**：来自git上的托管仓库上的包，要求package.json文件必须在仓库的跟目录下，比如[UEC的upm分支](https://github.com/Chino66/UEC/tree/upm)的目录结构。

**by name**：通过如`com.aa.bb`名称形式添加包，这种方式是最好用的，但相应的有较为复杂的配置要求。

:::
本地安装不在本文的讨论范围内。
:::

### from git URL的缺点

首先使用git添加包是一个很棒的想法，通常开发者开发一个项目都会用git去进行项目管理，所以只要将项目稍加处理就能变成一个upm包去使用。并且我还开发过一个基于git URL的upm开发工具[UPM-Tool-Develop](https://github.com/Chino66/UPM-Tool-Develop)，这是一个开源项目，可以帮助理解git URL方式添加包。

当git URL有一个巨大的缺点那就是它无法支持依赖项的引入，因为git URL不是包依赖项的标准语法，同样它不能进行版本控制。这个缺点将直接导致使用git URL的包必须没有依赖，这不符合代码重用的期望。

### by name添加包

这是添加包的主要方式，因为它是标准的npm语法，所以可以自动引入依赖项。这种方式需要配置[注册表](https://docs.unity3d.com/Manual/upm-scoped.html)。

## 包的托管

通过注册表添加包，这包一定是被托管的，目前主流有几种托管方式：

**npm托管**：前面说过upm本质上是npm，所以包可以发布在[npm](https://www.npmjs.com/)上进行托管，但npm发布的包必须是public的，如果要想私有则要收费。

**openupm托管**：[openupm](https://openupm.com/)是开源的upm包托管平台，上面有大量开发者发布的upm包，它和npm托管一样必须是public的，如果私有则要[收费](https://openupm.com/docs/host-private-upm-registry-15-minutes.html)。

**部署个人的托管服务**：使用[Verdaccio](https://verdaccio.org/)部署npm的托管服务（openupm就使用Verdaccio），但它有一个缺点就是在私域内无法方便依赖外部包。以及外部访问不便。

## 基于Github的包托管

Github有Package功能，可以发布如NuGet，npm等包，而upm就是npm！所以我们可以将我们的upm包发布在Github上托管。

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/1cd88d0b-d392-4dc3-8fb0-bd30d6236663.png)

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/b21b3346-cbf6-4211-b6e0-62b70620ca91.png)

### Github托管npm的好处

基于仓库发布package它们在同一个空间下更好维护

Github托管可以支持私有包，完全免费

只要Github仓库稍作处理就能发布package非常方便

只需要添加注册表就能访问其他用户的pacakge非常灵活

### 发布包到Github

首先Unity项目远程仓库必须在Github，在项目创建一个目录\_package\_，要发布的内容都要放在目录里面，在\_package\_添加package.json文件并配置内容，示例如下：

    {
      "name": "com.chino.lab.package",
      "displayName": "Lab Package",
      "version": "0.0.2",
      "unity": "2023.1",
      "description": "upm实验包",
      "type": "lib",
      "author": "chino66",
      "license": "MIT",
      "repository": {
        "type": "git",
        "url": "git+https://github.com/chino66/Lab_Package.git"
      },
      "bugs": {
        "url": "https://github.com/chino66/Lab_Package/issues"
      },
      "homepage": "https://github.com/chino66/Lab_Package#readme",
      "publishConfig": {
        "registry": "https://npm.pkg.github.com/@chino66"
      },
      "dependencies": {}
    }

:::
因为发布的是[npm](https://nodejs.org/en)所以必须安装nodejs
:::

**配置.npmrc文件**，这个文件配置了哪个用户发布包，内容如下：

    @chino66:registry=https://npm.pkg.github.com
    //npm.pkg.github.com/:_authToken={your token}

其中\_authToken在[Github上获取](https://docs.github.com/zh/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)

.npmrc一般放在`C:\Users\{user name}\.npmrc`下全局生效，当然也可以放在\_pacakge\_同级目录下给每个项目单独配置，**但不建议这么做**，因为token是重要信息，需要谨慎保存，不要提交到Github上（Github会检测token，如果识别到提交了Github，这token会作废，是一个保护机制）

在\_package\_目录下使用cmd输入：

> npm.cmd publish {your path}\\_package\_

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/e6545ded-bd13-4297-a4e6-b162e14cac0a.png)

发布成功的提示，等待1-2分钟就可以在Github上显示了。

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/9a731a2c-c53e-40e4-a01c-a93916d45784.png)

## UPMKits的使用

使用UPMKits工具可以更加方便地发布和管理upm包，上面的发布等管理操作在UPMKits上进行了封装。

### 添加pacakge包

打开UPMKits工具Tools/UPM Kits/Develop Tool

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/ef3bd2f3-929a-451b-a964-be290d3daa29.png)

可以看到chino66是在[UEC](https://github.com/Chino66/UEC)中配置的开发者，这个包会以chino66的身份发布（[需要token](https://docs.github.com/zh/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)），点击create package将创建一个基本包结构：

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/88ae4d26-a2d3-402f-9e74-6cdf500671d9.png)

下图是基本结构：

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/1c5f85f0-eecf-425a-bc15-dc6be732ce2b.png)

（目前需要重开界面..）完善package.json的信息，点击Apply

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/11ee0ffe-ad59-40c1-b792-f6f2195b8efc.png)

可以看到package.json已经修改完成

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/566015e1-c1bb-4e6b-af8f-0d0485193d20.png)

### 发布package

:::
因为我们是基于Github发布npm所以发布前必须确认这个Unity项目已经在托管到Github的远程仓库中。
:::

1.  点击Publish，工具会将package目录下的内容发布到Github上
    

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/8e6774b7-6611-462c-90fc-680bc63093de.png)

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/587738e6-81b5-41b4-a21a-51137902702d.png)

:::
因为是npm发布所以需要安装Nodejs
:::

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/856229c0-56f7-479d-9538-12032e5a55ec.png)

等待1-2分钟后可以看到Github上已经显示发布的包了。

### 其他

工具还提供了包的删除和列表功能。

![image](https://alidocs.oss-cn-zhangjiakou.aliyuncs.com/res/Q35O8LWkL8oyO9Vb/img/a75bbc0d-f9e5-46be-9bbe-c1ab99532fc9.png)

发布成功后，可以在面板右边管理已有的包。