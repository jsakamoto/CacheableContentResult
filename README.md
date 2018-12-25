CacheableContentResult [![NuGet Package](https://img.shields.io/nuget/v/CacheableContentResult.svg)](https://www.nuget.org/packages/CacheableContentResult/)
======================

## What's this? / これは何か?

This is the class library that is ActionResult of ASP.NET MVC .

This ActionResult class for ASP.NET MVC allows you to send a binary contents response that can cache based on Etag, and/or last modified date, to web browser. 

If the cache hit, then this class send "HTTP 304 Not Modified" without calling contents retrieving callback, and witout content body.

これは ASP.NET MVC の ActonResult であるクラスライブラリです。

この ActionResult は Etag および最終更新日によるキャッシュ制御された任意のバイナリコンテンツをWebブラウザに応答することができます。

要求ヘッダの If-Modified-Since および If-Match-None からキャッシュヒットが判断できた場合は、コンテンツ本体を返すことなく、HTTP 304 Not Modified をブラウザに返します。

## How to install? / インストール方法

You can install this libray as a NuGet package into your ASP.NET MVC Web Appliction project on Visual Studio via NuGet.org.

Visual Studio 上の ASP.NET MVC Web アプリケーションに NuGet パッケージとして NuGet.org 経由でインストールできます。

```
PM> Install-Package CacheableContentResult
```

## How to use? / 使い方

```csharp
public ActionResult Picture(int id)
{
  var imagePath = HttpContext.Server.MapPath("~/App_Data/user/{id}/picture.png");
  var lastWriteTime = System.IO.File.GetLastWriteTimeUtc(imagePath);

  return new CacheableContentResult(
    contentType: "image/png",
    lastModified: lastWriteTime,
    getContent: () =>
    {
      return System.IO.File.ReadAllBytes(imagePath);
    }
  );
}
```

![movie](https://raw.githubusercontent.com/jsakamoto/CacheableContentResult/master/.asset/movie001.gif)

Return the instance of CachebleContentResult class in MVC action method specified with ETag, and/or the date time of last modified, and the function which return the binary content as byte[] to responding you want.

The function which return the content you specified does not call if not needed, then you can implement "lazy loading".

See the [example](https://github.com/jsakamoto/CacheableContentResult/blob/master/SampleSite/Controllers/UserprofileCOntroller.cs#L21).

MVC アクションメソッド内で、ETag、および/あるいは最終更新日、そして応答に返したいバイナリコンテンツを byte[] で返す関数を指定した CachebleContentResult クラスのインスタンスを返します。

必要となるまでコンテンツを返す関数は呼び出されませんので、遅延読み込みが実装できます。

例は [こちら](https://github.com/jsakamoto/CacheableContentResult/blob/master/SampleSite/Controllers/UserprofileCOntroller.cs#L21) を参照ください。

## License / ライセンス

[Mozilla Public License Version 2.0](LICENSE) / [Microsoft Public License (MS-PL)](https://opensource.org/licenses/MS-PL)

(* dual license )



