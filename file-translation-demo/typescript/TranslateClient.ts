import * as CryptoJS from 'crypto-js';
import axios from 'axios';
import {
    AxiosInstance,
    AxiosRequestConfig,
    AxiosResponse,
} from 'axios';
import TranslationOption from "./TranslationOption";
import {resolve} from "path";
import {existsSync, unlinkSync, createReadStream} from "fs";
import {v4} from "uuid"
// @ts-ignore
import * as AdmZip from "adm-zip"
import {ensureDirSync, outputFileSync} from "fs-extra";
import ClassiiiUserInfo from "./ClassiiiUserInfo";
// @ts-ignore
import * as RC4 from "rc4.js";
import * as  FormData from "form-data"


export class TranslateClient {
    private readonly http: AxiosInstance;
    private baseUrl:string="https://test.classiii.io/api/v1";
    constructor() {
        this.http = axios.create({
            baseURL: this.baseUrl,
        });
    }
    public async getAuthCode(orgId:string, userId:string, url:string):Promise<string>{
        const params = {
            orgId: orgId,
            userId: userId,
        };

        const timeoutSeconds: number = 60;
        const  {data} = await this.post(url, params, timeoutSeconds);
        if(data.status!="success"){
            throw new Error('server error');
        }

        return data.data.id;
    }
    public async authenticate(orgId:string, userId:string, password:string, authCode:string, url:string):Promise<ClassiiiUserInfo>{
        const params = {
            orgId: orgId,
            userId: userId,
            password:password
        };
        let rc4=new RC4(authCode);
        let encryptedPasswd = rc4.encrypt(params.password);
        params.password = encryptedPasswd;

        const timeoutSeconds: number = 60;
        const  {data} = await this.post(url, params, timeoutSeconds);
        if(data.status!="success"){
            throw new Error('server error');
        }

        return data.data;
    }

    public getSignature(nonce:string, path:string, secretKey:string):string{
        return CryptoJS.HmacSHA256(nonce+path, secretKey).toString(CryptoJS.enc.Hex);
    }

    public async translate(files: string[], option: TranslationOption, translateUrl:string):Promise<AxiosResponse>{
        const url = new URL(this.getBaseUrl()+translateUrl).pathname;
        const signature = this.getSignature(option.nonce, url, option.secretKey);
        option.signature=signature;
        let bodyFormData = new FormData();
        bodyFormData.append('fieldId', option.fieldId);
        bodyFormData.append('targetLangs', JSON.stringify(option.langs));
        for(let i=0;i<files.length;i++){
            bodyFormData.append(`files`, createReadStream(files[i]));
        }
        const signatureHeader={
            'accessKey':option.accessKey,
            'nonce':option.nonce,
            'signature':option.signature,
        };
        return await this.http.post(translateUrl,　bodyFormData,{
            headers:{
                'Content-Type': `multipart/form-data; boundary=${bodyFormData.getBoundary()}`,
                ...signatureHeader
            }
        });
    }

    public async getAllHistories(accessKey:string, secretKey: string, url:string,timeoutSeconds?: number): Promise<AxiosResponse> {
        if (!timeoutSeconds) {
            timeoutSeconds = 15;
        }
        let urlToSignature = new URL(this.http.defaults.baseURL+url).pathname;
        let nonce = Date.now().toString();
        let signature = this.getSignature(nonce, urlToSignature, secretKey as string);

        const config: AxiosRequestConfig = {
            timeout: timeoutSeconds * 1000,
            headers: {
                accessKey,
                nonce,
                signature
            },
        };

        return await this.http.get(url, config);
    }
    public async getSomeHistories(accessKey:string, secretKey: string, url:string, translateId:string, timeoutSeconds?: number): Promise<AxiosResponse> {
        if (!timeoutSeconds) {
            timeoutSeconds = 60;
        }
        url+="/"+translateId;
        let urlToSignature = new URL(this.http.defaults.baseURL+url).pathname;
        let nonce = Date.now().toString();
        let signature = this.getSignature(nonce, urlToSignature, secretKey as string);

        const config: AxiosRequestConfig = {
            timeout: timeoutSeconds * 1000,
            headers: {
                accessKey,
                nonce,
                signature
            },
        };

        return await this.http.get(url, config);
    }

    public async downloadZip( ids: string, accessKey: string, secretKey: string, savePath:string, url:string, timeoutSeconds?: number): Promise<boolean> {
        if (!timeoutSeconds) {
            timeoutSeconds = 60;
        }
        url = url+"?ids="+ encodeURIComponent(ids);
        let fullUrl=new URL(this.http.defaults.baseURL+url);
        let pathname=decodeURIComponent(fullUrl.pathname + fullUrl.search);
        let nonce = Date.now().toString();
        let signature = this.getSignature(nonce, pathname, secretKey as string);
        const config: AxiosRequestConfig = {
            timeout: timeoutSeconds * 1000,
            headers: {
                accessKey,
                nonce,
                signature,
            },
            responseType: 'arraybuffer',
        };

        let {data} = await this.http.get(fullUrl.href, config);
        this.save(savePath, data);
        return true;
    }

    private save(savePath:string, data: ArrayBuffer){
        ensureDirSync(savePath);

        const zipFileName: string = resolve(savePath, v4());

        const byteArray = new Uint8Array(data);
        var buffer = new Buffer(byteArray.length);
        for (var i = 0; i < byteArray.length; i++) {
            buffer.writeUInt8(byteArray[i], i);
        }
        outputFileSync(zipFileName, buffer);

        try {
            const zip = new AdmZip(zipFileName);
            zip.extractAllTo(savePath, true);

        } finally {
            if (existsSync(zipFileName)) {
                unlinkSync(zipFileName);
            }
        }
    };

    public getBaseUrl():string{
        return this.baseUrl;
    }

    public async post(url: string, data?: any, timeoutSeconds?: number): Promise<AxiosResponse> {
        if (!timeoutSeconds) {
            timeoutSeconds = 60;
        }

        const config: AxiosRequestConfig = {
            timeout: timeoutSeconds * 1000,
            headers: {'Access-Control-Allow-Origin': '*'},
        };
        return await this.http.post(url, data, config);
    }

}
