import {Translate} from "./Translate";
import {TranslateClient} from "./TranslateClient";
export class ApiFlow {

    // メインの流れ
    public async main() {
        const client = new TranslateClient();
        //  CLASSIIIのユーザー情報（accessKey,secretKey）のお客様、下記のステップ１と２は必要ないので、直接ステップ３に行く。

        //1. T4OOのユーザー情報（accesssKey,secretKey）の発行を申請する
        const authCode = await client.getAuthCode("お客様に持っているt4ooの会社id", "お客様に持っているt4ooのユーザーid","/auth-code");
        console.log(`authCode:${authCode}`);

        //2. CLASSIIIのユーザー情報（accesssKey,secretKey）の発行を申請する
        //　　毎回請求すると、secretKeyは変わる、前のsecretKeyは使えなくなる。
        const userInfo = await client.authenticate("お客様に持っているt4ooの会社id", "お客様に持っているt4ooのユーザーid", "お客様に持っているt4ooのパスワード", authCode, '/authenticate');
        console.log(`userInfo:${JSON.stringify(userInfo)}`);
        const translateFileApiUrl="/file-translate";
        const option={
            accessKey: userInfo.accessKey,
            secretKey: userInfo.secretKey,
            nonce: Date.now().toString(),
            signature:"",
            langs: ["en"],
            fieldId: 1
        };
        console.log(`option:${JSON.stringify(option)}`);


        //3. 翻訳したいファイル絶対パスを配列に入れる、翻訳のレクエストを投げる
        const {data}= await client.translate(["c:\\documents\\テスト.docx","c:\\documents\\テスト2.docx"], option, translateFileApiUrl);
        if(!data.data || !data.data.translateId)
            return;
        console.log(`translateId:${data.data.translateId}`);

        //４． あるタイミングで翻訳の結果を問い合わせる
        let internalId=setInterval(async () => {
            // 4.1 全部の翻訳した項目Idを取得する
            // const historyUrl="/translate-result/all";
            // let ids:string[]= await getAllDoneItemIds(client, userInfo.accessKey, userInfo.secretKey, historyUrl);

            // 4.2 前の翻訳項目を完成したかどうかを確認する
            let historyUrl="/translate-result";
            let ids:string[]= await this.getSomeDoneItemIds(client, userInfo.accessKey, userInfo.secretKey, data.data.translateId, historyUrl);

            if(ids.length>0){
                const downloadUrl="/downloads";
                const savePath="c:\\documents\\outputs";

                clearInterval(internalId);
                //５． 翻訳の結果を圧縮ファイルとしてダウンロードする。
                await client.downloadZip( JSON.stringify(ids), userInfo.accessKey,userInfo.secretKey, savePath, downloadUrl);
            }


        },15000);
    }

    // 全部の翻訳の結果をクエリーする
    private async queryAllResult(client:TranslateClient, accessKey:string, secretKey:string, url:string):Promise<Translate[]>{
        const {data}= await client.getAllHistories(accessKey, secretKey, url);
        return data.data;
    }

    // ある翻訳の結果をクエリーする
    private async querySomeResults(client:TranslateClient, accessKey:string, secretKey:string, translateId:string, url:string):Promise<Translate>{
        const {data}= await client.getSomeHistories(accessKey, secretKey, url, translateId);
        return data.data;
    }

    // 全部完成した翻訳の項目IDを取得する
    private async getAllDoneItemIds(client:TranslateClient, accessKey:string, secretKey:string, url:string){
        let translates = await this.queryAllResult(client, accessKey, secretKey, url);
        // 翻訳のIDをまとめて
        let ids:string[]=[]
        for(let translate of translates){
            for(let item of translate.items){
                if(item.done)
                    ids.push(item.translateItemId);
            }
        }
        return ids;
    }

    // ある翻訳の完成した項目IDを取得する
    private async getSomeDoneItemIds(client:TranslateClient, accessKey:string, secretKey:string, translateId:string, url:string){
        let translate = await this.querySomeResults(client, accessKey, secretKey, translateId, url);
        // 翻訳のIDをまとめて
        let ids:string[]=[]
        for(let item of translate.items){
            if(item.done)
                ids.push(item.translateItemId);
        }
        return ids;
    }
}
