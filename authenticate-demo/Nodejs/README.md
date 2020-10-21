# Rozetta API コードサンプル (Node.js)

このディレクトリには、Node.jsでRozetta APIを利用する簡単なサンプルコードを置いています。

## Requirements

* Node.js 12

## Run

コードを実行する前に、下記コマンドで依存関係をインストールしてください。

```
npm install
```

## Examples

現在、下記のサンプルコードをご利用いただくことができます。

* auth-by-token.js - 認証用トークンで認証してRozetta APIを利用する。
* auth-signature.js - 認証用署名を生成するサンプルコード。
* jwt.js - 認証用トークンを生成するサンプルコード。

### 認証用署名を生成する

認証用署名を生成したい場合は、`auth-signature.js`で下記変数を変更します：

* `authConfig.accessKey` - アクセスキー。
* `authConfig.secretKey` - シークレットキー。
* `path` - APIのパス。

Node.jsで`auth-signature.js`を実行します：

```
npm run auth-signature
```

下記のような結果が得られます：

```
Access key: YOUR_ACCESSKEY
Signature: 8526f55b026b8dabb90e63182a13d4359e95715bff11792ccc55fb43e88f9a88
Nonce: 1603173327657
Path: /api/v1/translate
```

### 認証用トークンで認証してRozetta APIを利用する

認証用署名の代わりに、認証用トークンで認証してRozetta APIを利用したい場合は、
`auth-by-token.js`で下記変数を変更します：

* `token` - 取得した又は生成したトークン。

Node.jsで`auth-by-token.js`を実行します：

```
npm run auth-by-token
```

下記のような結果が得られます：

```
{ status: 'success', message: 'hello, world!' }
```

このサンプルは`/api/v1/hello`APIを利用しますが、他のRozetta APIもトークンで認証できます。

トークンの生成方法について、「認証用トークンを生成する」と「認証用トークンをRozetta APIから
取得する」を参照してください。

### 認証用トークンを生成する

認証用トークンを生成したい場合は、`jwt-generate.js`で下記変数を変更します：

* `userId` - Rozetta APIユーザID。
* `accessKey` - アクセスキー。
* `secretKey` - シークレットキー。
* `validDuration` - トークンの有効期間（単位：秒）。

Node.jsで`jwt.js`を実行します：

```
npm run jwt
```

下記のような結果が得られます：

```
JWT: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MDMxNzUzODQsImlzcyI6Ik15VXNlcklEIiwiYWNjZXNzS2V5IjoibXktYWNjZXNzLWtleSIsImlhdCI6MTYwMzE3MzU4NH0.8TV7YC10OT8p4gAZ0zvowb3nyPcfznGivXpZfFseO0w
```
