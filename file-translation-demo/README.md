# ファイル翻訳APIマニュアル

## 1 ファイル翻訳APIの記述
[ファイル翻訳API（Swagger）](https://app.swaggerhub.com/apis-docs/classiii/file-translate/1.0.0-oas3)
## 2 T4OOユーザーの場合
### <a name="authcode"></a>&nbsp;&nbsp;1) . authcode獲得する
&nbsp;&nbsp;&nbsp;&nbsp;
T4OOのorgId，userIdを使って、ファイル翻訳API(/auth-code)からauthcodeを取得します。
### &nbsp;&nbsp;2) . T4OO認証行い、CLASSIIIユーザーを作成する
&nbsp;&nbsp;&nbsp;&nbsp;
サーバ側から[authcode](#authcode)を使って、T4OOのパスワードを暗号化します。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
T4OOのorgId、userId、暗号化のパスワードを使って、ファイル翻訳API(/authenticate)でT4OOの認証を行います。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
認証成功した後、CLASSIIIユーザー情報（AccessKey, SecretKey）を入手します。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
その後は、CLASSIIIユーザーと同じ下記のような流れです。

## 3 CLASSIIIユーザーの場合
### &nbsp;&nbsp;1) . ファイルアップロードする
&nbsp;&nbsp;&nbsp;&nbsp;
複数のファイルを一度にアップロードすることが可能です。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロードの時、HTTPヘッダに必要な項目は下記のようにしてください。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
accessKey: アクセスキー<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
nonce: nonce<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
signature: signature<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
[各項目の説明はこちら](https://translate.classiii.io/doc/ja/authentication)<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロードの時、HTTP bodyに必要な項目は下記のようにしてください。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
fieldId：分野Id<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
targetLangs：配列、複数可<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
files：選択するファイル、複数可<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
[各項目の説明はこちら](https://app.swaggerhub.com/apis/classiii/file-translate/1.0.0-oas3#/file-translate/post_file_translate)<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロードに成功すると、translateIdを入手できます。<br/>
### &nbsp;&nbsp;2) . 翻訳結果を問い合わせる
&nbsp;&nbsp;&nbsp;&nbsp;
/translate-result/{translateId} に翻訳結果を問い合わせます。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
TranslateItemは一つのファイルで、一つの言語に対してそれぞれ一つのTranslateItemにが作られます。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
翻訳結果のTranslateItemのdoneフィールドがtrueになると、このアイテムの翻訳が完了したということを意味します。<br/>

### &nbsp;&nbsp;3) . 翻訳結果をダウンロードする
&nbsp;&nbsp;&nbsp;&nbsp;
翻訳が完了したTranslateItemをダウンロードすることができます。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
/downloadsから翻訳結果をダウンロードできます。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
翻訳結果は圧縮ファイルなので、解凍するとすべての翻訳結果を見ることができます。<br/>
