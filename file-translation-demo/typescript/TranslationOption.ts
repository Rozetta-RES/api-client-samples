export default interface TranslationOption {
    accessKey: string;
    secretKey: string;
    nonce: string;
    signature: string;
    langs: string[];
    fieldId: number;
}
