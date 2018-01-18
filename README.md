# VipAnalyser

[VipAnalyser](https://github.com/niubileme/VipAnalyser) 是一个能够解析视频原始地址的服务。


##介绍

这是一个简单的调试工具，你可以像这样很容易的解析腾讯视频的真实地址

![](https://raw.githubusercontent.com/niubileme/VipAnalyser/master/img/debug.png)

+ 参数
	Url：腾讯视频的页面地址

```console
{
    "Url": "https://v.qq.com/x/cover/kds9l8b75jvb6y6.html"
}
```

## 支持

| Site | URL | Videos? | 
| :--: | :-- | :-----: | 
| **腾讯视频** | <https://v.qq.com/>    |✓| 


##调试

+ 修改**VipAnalyser.Service**配置文件，端口，账号密码。

```config
  <appSettings>
    <!--绑定端口-->
    <add key="Port" value="11234" />
    <!--版本号-->
    <add key="ClientVersion" value="1.0.0.1" />
    <!--QQ账号-->
    <add key="QQUserName" value="" />
    <!--QQ密码-->
    <add key="QQPassword" value="" />
  </appSettings>
```
+ 启动**VipAnalyser.Service**服务
+ 启动**VipAnalyser.TestClient**调试客户端

>目前只支持通过http方式


##使用

**VipAnalyser.Service**下有一个`install.cmd`脚本，能够直接安装成windows服务，需要以管理员方式运行。
然后就能愉快的调用该服务了。

