# ファイル翻訳APIマニュアル

## 1 ファイル翻訳APIの記述
[ファイル翻訳API](https://app.swaggerhub.com/apis-docs/classiii/file-translate/1.0.0-oas3)
## 2 T4OOユーザーの場合
### <a name="authcode"></a>&nbsp;&nbsp;1) . authcode獲得する
&nbsp;&nbsp;&nbsp;&nbsp;
T4OOのorgId，userIdを使って、ファイル翻訳API(/auth-code)にauthcodeを獲得
### &nbsp;&nbsp;2) . T4OO認証行い、CLASSIIIユーザーを作成する
&nbsp;&nbsp;&nbsp;&nbsp;
サーバ側から[authcode](#authcode)を使って、T4OOのパスワードを暗号化する。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
T4OOのorgId、userId、暗号化のパスワードを使って、ファイル翻訳API(/authenticate)にT4OOの認証を行う。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
認証成功した後、CLASSIIIユーザー情報（AccessKey, SecretKey）を入手する。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
その後、CLASSIIIユーザー（下記）と同じ流れ。

## 3 CLASSIIIユーザーの場合
### &nbsp;&nbsp;1) . ファイルアップロードする
&nbsp;&nbsp;&nbsp;&nbsp;
複数のファイルを一遍にアップロード可能です。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロードの時、Httpヘッダに必要な項目は下記のように：<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
accessKey<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
nonce<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
signature<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
[説明はここだ。](https://translate.classiii.io/doc/ja/authentication)<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロードの時、Http bodyに必要な項目は下記のように：<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
fieldId：分野Id<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
targetLangs：配列、複数可<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
files：選択したファイル内容、複数可<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
[説明はここだ。](https://app.swaggerhub.com/apis/classiii/file-translate/1.0.0-oas3#/file-translate/post_file_translate)<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
アップロード成功した後、translateIdを入手する。<br/>
### &nbsp;&nbsp;2) . 翻訳結果を問い合わせる
&nbsp;&nbsp;&nbsp;&nbsp;
/translate-result/{translateId}に翻訳結果を問い合わせる。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
TranslateItemとは一つのファイル、一つの言語を一つのTranslateItemに作る。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
翻訳結果のTranslateItemのdoneフィールドがtrueになる場合、当TranslateItemの翻訳完了という意味だ。<br/>

### &nbsp;&nbsp;3) . 翻訳結果をダウンロードする
&nbsp;&nbsp;&nbsp;&nbsp;
既にdoneのTranslateItemはダウンロード可能です。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
それらのTranslateItemは/downloadsにダウンロードすると、翻訳結果になる。<br/>
&nbsp;&nbsp;&nbsp;&nbsp;
翻訳結果は圧縮ファイルですので、解凍すれば、すべての翻訳結果を見れる。<br/>
